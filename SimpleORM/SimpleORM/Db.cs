using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORM
{
	// TODO setup IDb interface
	// TODO should this be a generic class???
	public class Db
	{
		private string _connectionString = null;

		public Db(string connectionString)
		{
			_connectionString = connectionString;
		}

		public T ExecuteReaderToSingle<T>(QueryStatement<T> queryCommand)
		{
			var item = default(T);
			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (var cmd = new SqlCommand(queryCommand.SqlQuery, conn))
				{
					var reader = cmd.ExecuteReader();
					var columnNames = (from row in reader.GetSchemaTable().AsEnumerable()
									   select row.Field<string>("ColumnName")).ToList();
					var itemType = typeof(T);
					var count = 0;
					while (reader.Read())
					{
						if (count > 0)
							throw new ApplicationException("More than one item was returned for the query: " + 
								queryCommand.SqlQuery);
						item = Activator.CreateInstance<T>();
						(from columnName in columnNames select columnName).
							ToList().ForEach(columnName =>
							{
								var property = itemType.GetProperty(columnName);
								if (property != null)
									property.SetValue(item, reader[columnName]);
							});
						count++;
					}
				}
			}
			return item;
		}
		public List<T> ExecuteReaderToList<T>(QueryStatement<T> queryCommand)
		{
			var list = new List<T>();
			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (var cmd = new SqlCommand(queryCommand.SqlQuery, conn))
				{
					var reader = cmd.ExecuteReader();
					var columnNames = (from row in reader.GetSchemaTable().AsEnumerable()
									   select row.Field<string>("ColumnName")).ToList();
					var itemType = typeof(T);
					while (reader.Read())
					{
						var item = Activator.CreateInstance<T>();
						(from columnName in columnNames select columnName).
							ToList().ForEach(columnName => 
						{
							var property = itemType.GetProperty(columnName);
							if (property != null)
								property.SetValue(item, reader[columnName]);
						});
						list.Add(item);
					}
				}
			}
			return list;
		}
	}
}

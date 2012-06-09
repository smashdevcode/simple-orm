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

		public List<T> ExecuteReaderToList<T>(string sqlQuery)
		{
			var list = new List<T>();
			var itemType = typeof(T);
			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (var cmd = new SqlCommand(sqlQuery, conn))
				{
					var reader = cmd.ExecuteReader();
					var columnNames = (from row in reader.GetSchemaTable().AsEnumerable()
									   select row.Field<string>("ColumnName")).ToList();
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

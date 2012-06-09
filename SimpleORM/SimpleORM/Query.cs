using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORM
{
    public class Query<T> : IDisposable where T : class
    {
		private string _connectionString = null;
		private Db _db = null;

		public string SqlQuery { get; private set; }

		// TODO setup default constructor that looks for a "SimpleORM" connection string in the configuration file

		public Query(string connectionString)
		{
			_connectionString = connectionString;
			_db = new Db(connectionString);
		}

		//public Query Select<T>() where T : class
		//{
		//	return this;
		//}
		public List<T> SelectToList()
		{
			return _db.ExecuteReaderToList<T>(this.BuildQuery());
		}

		private string BuildQuery()
		{
			var genericType = this.GetType().GetGenericArguments()[0];

			// TODO cache the entity property type information???
			// TODO need to handle collections and reference properties

			// use the properties of the generic type as the select list
			var properties = genericType.GetProperties();
			var propertyNames = (from property in properties
								 where property.CanWrite
								 select property.Name).ToList();
			var selectList = string.Join(", ", propertyNames);
			// use the generic type name as the "from"
			var fromClause = genericType.Name;

			// build the query
			var select = new StringBuilder();
			select.AppendFormat("select {0}{1}", selectList, Environment.NewLine);
			select.AppendFormat("from {0}", fromClause);
			this.SqlQuery = select.ToString();
			return this.SqlQuery;
		}

		public void Dispose()
		{
			// TODO dispose of resources
		}
	}
}

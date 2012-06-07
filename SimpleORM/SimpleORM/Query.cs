using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORM
{
    public class Query<T> : IDisposable where T : class
    {
		public string SqlQuery { get; private set; }

		//public Query Select<T>() where T : class
		//{
		//	return this;
		//}
		public List<T> SelectToList()
		{
			BuildQuery();

			// TODO run query against the database
			// TODO populate a generic collection of materialized entities

			return new List<T>();
		}

		private void BuildQuery()
		{
			var genericType = this.GetType().GetGenericArguments()[0];

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
		}

		public void Dispose()
		{
			// TODO dispose of resources
		}
	}
}

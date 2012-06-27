using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SimpleORM.Enums;

namespace SimpleORM
{
    public class Query<T> : IDisposable where T : class
    {
		private string _connectionString = null;
		private Db _db = null;
		private List<Expression<Func<T, bool>>> _whereExpressions = null;

		public QueryStatement<T> QueryStatement { get; private set; }

		// TODO setup default constructor that looks for a "SimpleORM" connection string in the configuration file

		public Query(string connectionString)
		{
			_connectionString = connectionString;
			_db = new Db(connectionString);
			_whereExpressions = new List<Expression<Func<T, bool>>>();
		}

		//public Query Select<T>() where T : class
		//{
		//	return this;
		//}
		public T SelectToSingle()
		{
			return _db.ExecuteReaderToSingle<T>(this.GetQueryStatement());
		}
		public List<T> SelectToList()
		{
			return _db.ExecuteReaderToList<T>(this.GetQueryStatement());
		}
		public Query<T> Where(Expression<Func<T, bool>> whereExpression)
		{
			_whereExpressions.Add(whereExpression);
			return this;
		}

		// TODO move all query statement building into the QueryStatement class

		private QueryStatement<T> GetQueryStatement()
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
			// convert the where predicates to a where clause
			var whereClause = string.Empty;
			if (_whereExpressions.Count > 0)
				whereClause = BuildWhereClause(_whereExpressions);

			// build the query
			var select = new StringBuilder();
			select.AppendFormat("select {0}{1}", selectList, Environment.NewLine);
			select.AppendFormat("from {0}", fromClause);
			if (!string.IsNullOrWhiteSpace(whereClause))
			{
				select.Append(Environment.NewLine);
				select.AppendFormat("where {0}", whereClause);
			}

			// setup the query command
			var queryStatement = new QueryStatement();
			queryStatement.SqlQuery = select.ToString();

			// TODO setup the query command parameters
	



			
			this.QueryStatement = queryStatement;
			return queryStatement;
		}

		public void Dispose()
		{
			// TODO dispose of resources
		}
	}
}

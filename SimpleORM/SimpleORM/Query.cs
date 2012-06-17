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

		public string SqlQuery { get; private set; }

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
		public List<T> SelectToList()
		{
			return _db.ExecuteReaderToList<T>(this.BuildQuery());
		}
		public Query<T> Where(Expression<Func<T, bool>> whereExpression)
		{
			_whereExpressions.Add(whereExpression);
			return this;
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
			// convert the where predicates to a where clause
			var whereClause = string.Empty;
			if (_whereExpressions.Count > 0)
				whereClause = BuildWhereClause(_whereExpressions);

			// build the query
			var select = new StringBuilder();
			select.AppendFormat("select {0}{1}", selectList, Environment.NewLine);
			select.AppendFormat("from {0}", fromClause);
			if (!string.IsNullOrWhiteSpace(whereClause))
				select.AppendFormat("where {0}", whereClause);
			this.SqlQuery = select.ToString();
			return this.SqlQuery;
		}
		private static string BuildWhereClause(List<Expression<Func<T, bool>>> whereExpressions)
		{
			var whereClause = new StringBuilder();
			foreach (var whereExpression in whereExpressions)
			{
				// TODO need to handle different setups
				// TODO combine where clauses that have the same column name and operator
				// i.e. "(CustomerID = 1 or CustomerID = 2) and (CustomerID != 1 and CustomerID != 2)"

				// TODO read chapter from Microsoft LINQ book about expression trees
				// need to learn how to handle different types of expression types

				var body = (BinaryExpression)whereExpression.Body;
				var left = (ParameterExpression)body.Left;
				var nodeType = body.NodeType;
				var right = (ConstantExpression)body.Right;

				string operatorString = null;
				switch (nodeType)
				{
					case ExpressionType.Equal:
						operatorString = "=";
						break;
					default:
						throw new ApplicationException("Unexpected Expression NodeType: " + nodeType.ToString());
				}

			}
			return whereClause.ToString();
		}

		private class WhereClauseSegment
		{
			public string ColumnName { get; set; }
			public OperatorType Operator { get; set; }
			public object Value { get; set; }
		}

		public void Dispose()
		{
			// TODO dispose of resources
		}
	}
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SimpleORM.Enums;

namespace SimpleORM
{
	public class QueryStatement<T> where T : class
	{
		public QueryStatement()
		{
			this.Parameters = new List<SqlParameter>();
		}

		public string SqlQuery { get; set; }
		public string WhereClause { get; set; }
		public List<SqlParameter> Parameters { get; set; }

		private void BuildWhereClause(List<Expression<Func<T, bool>>> whereExpressions)
		{
			var whereClauseSegments = new List<WhereClauseSegment>();
			foreach (var whereExpression in whereExpressions)
			{
				// TODO need to handle different setups
				// TODO combine where clauses that have the same column name and operator
				// i.e. "(CustomerID = 1 or CustomerID = 2) and (CustomerID != 1 and CustomerID != 2)"

				// TODO read chapter from Microsoft LINQ book about expression trees
				// need to learn how to handle different types of expression types

				var body = (BinaryExpression)whereExpression.Body;
				var left = (MemberExpression)body.Left;
				var nodeType = body.NodeType;
				var right = (ConstantExpression)body.Right;

				OperatorType? operatorType = null;
				switch (nodeType)
				{
					case ExpressionType.Equal:
						operatorType = OperatorType.Equals;
						break;
					default:
						throw new ApplicationException("Unexpected Expression NodeType: " + nodeType.ToString());
				}

				var whereClauseSegment = new WhereClauseSegment()
				{
					ColumnName = left.Member.Name,
					Operator = operatorType.Value,
					Value = right.Value,
					CommandParameterType = GetSqlDbType(right.Type)
				};
				whereClauseSegments.Add(whereClauseSegment);
			}

			this.WhereClause = WhereClauseSegment.GetWhereClauseSql(whereClauseSegments);

			// TODO setup command parameters
		}
		private SqlDbType GetSqlDbType(Type type)
		{
			var typeCode = Type.GetTypeCode(type);
			switch (typeCode)
			{
				case TypeCode.Boolean:
					return SqlDbType.Bit;
				case TypeCode.Char:
					return SqlDbType.NChar;
				case TypeCode.DateTime:
					return SqlDbType.DateTime;
				case TypeCode.Decimal:
					return SqlDbType.Decimal;
				case TypeCode.Double:
					return SqlDbType.Float;
				case TypeCode.Int16:
					return SqlDbType.SmallInt;
				case TypeCode.Int32:
					return SqlDbType.Int;
				case TypeCode.Int64:
					return SqlDbType.BigInt;
				case TypeCode.Single:
					return SqlDbType.Real;
				case TypeCode.String:
					return SqlDbType.NVarChar;
				default:
					throw new ApplicationException("Unexpected TypeCode enum value: " + 
						typeCode.ToString());
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORM.Enums
{
	public enum OperatorType
	{
		Equals,
		NotEquals,
		LessThan,
		LessThanOrEqual,
		GreaterThan,
		GreaterThanOrEqual
	}

	public static class OperatorTypeHelper
	{
		public static OperatorType GetEnumFromString(string value)
		{
			return (OperatorType)Enum.Parse(typeof(OperatorType), value);
		}
		public static string GetWhereClauseSqlOperator(OperatorType enumValue)
		{
			switch (enumValue)
			{
				case OperatorType.Equals:
					return "=";
				case OperatorType.GreaterThan:
					return ">";
				case OperatorType.GreaterThanOrEqual:
					return ">=";
				case OperatorType.LessThan:
					return "<";
				case OperatorType.LessThanOrEqual:
					return "<=";
				case OperatorType.NotEquals:
					return "!=";
				default:
					throw new ApplicationException("Unexpected OperatorType enum value: " + enumValue.ToString());
			}
		}
	}
}

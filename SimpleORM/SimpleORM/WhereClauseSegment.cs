using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleORM.Enums;

namespace SimpleORM
{
	internal class WhereClauseSegment
	{
		public string ColumnName { get; set; }
		public OperatorType Operator { get; set; }
		public object Value { get; set; }
		public int Index { get; set; }
		public string CommandParameterName
		{
			get
			{
				return string.Format("@{0}{1}", this.ColumnName, this.Index);
			}
		}
		public SqlDbType CommandParameterType { get; set; }

		public override string ToString()
		{
			return string.Format("({0} {1} {2})",
				this.ColumnName,
				OperatorTypeHelper.GetWhereClauseSqlOperator(this.Operator),
				this.CommandParameterName);
		}

		public static string GetWhereClauseSql(List<WhereClauseSegment> whereClauseSegments)
		{
			var segmentStrings = new List<string>();

			// group by column name and operator
			(from s in whereClauseSegments
			 group s by new { s.ColumnName, s.Operator }).ToList().ForEach(s =>
			 {
				 var segments = s.ToList();
				 segments.ForEach(s2 => s2.Index = segments.IndexOf(s2) + 1);
				 var segmentString = string.Format("({0})", string.Join(" or ", segments));
				 segmentStrings.Add(segmentString);
			 });

			return string.Join(" and ", segmentStrings);
		}
	}
}

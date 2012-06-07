using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleORM;
using SimpleORMTests.Entities;

namespace SimpleORMTests
{
	[TestClass]
	public class QueryTests
	{
		private const string DATABASE_PATH = @"..\\..\\Database\Database.mdf";
		private const string CONNECTION_STRING = @"Data Source=(LocalDB)\v11.0;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30";
		private const string QUERIES_DIRECTORY = "..\\..\\Queries";

		private static string GetConnectionString()
		{
			return string.Format(CONNECTION_STRING, Path.GetFullPath(DATABASE_PATH));
		}
		private static string GetQueryText(string methodName)
		{
			var fileNameWithPath = Path.GetFullPath(QUERIES_DIRECTORY + "\\" + methodName + ".txt");
			return File.ReadAllText(fileNameWithPath);
		}
		private static bool CompareText(string expected, string actual, string methodName)
		{
			// compare strings and write to text file if not equals
			if (expected != actual)
			{
				var date = DateTime.Now;
				var expectedFileName = string.Format("{0}_Expected_{1:yyyyMMddhhmmss}.txt", methodName, date);
				var actualFileName = string.Format("{0}_Actual_{1:yyyyMMddhhmmss}.txt", methodName, date);
				var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				File.WriteAllText(desktopPath + "\\" + expectedFileName, expected);
				File.WriteAllText(desktopPath + "\\" + actualFileName, actual);
				return false;
			}
			return true;
		}

		#region DatabaseAccessTest
		[TestMethod]
		public void DatabaseAccessTest()
		{
			var rowValues = new List<string>();
			using (var conn = new SqlConnection(GetConnectionString()))
			{
				conn.Open();
				using (var cmd = new SqlCommand("select * from Customer", conn))
				{
					var reader = cmd.ExecuteReader();
					var columnNames = (from row in reader.GetSchemaTable().AsEnumerable()
									   select row.Field<string>("ColumnName")).ToList();
					while (reader.Read())
					{
						var values = (from columnName in columnNames
									  select string.Format("{0}: {1}", columnName, reader[columnName]));
						rowValues.Add(string.Join(", ", values));
					}
				}
			}
			Assert.AreEqual(2, rowValues.Count);
		}
		#endregion

		[TestMethod]
		public void SelectTest_Simple()
		{
			using (var qry = new Query<Customer>())
			{
				var methodName = MethodBase.GetCurrentMethod().Name;
				var result = qry.SelectToList();
				var expectedQuery = GetQueryText(methodName);
				Assert.IsTrue(CompareText(expectedQuery, qry.SqlQuery, methodName));
				Assert.AreEqual(2, result.Count);
				// TODO need a helper method to compare collections
			}
		}
		[TestMethod]
		public void SelectTest_WithWhere()
		{
			// TODO
		}
		[TestMethod]
		public void SelectTest_WithOrderBy()
		{
			// TODO
		}
		[TestMethod]
		public void InsertOrUpdateTest()
		{
			// TODO
		}
		[TestMethod]
		public void DeleteTest()
		{
			// TODO
		}
	}
}

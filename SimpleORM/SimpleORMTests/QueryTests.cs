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
using SimpleORM.Extensions;
using SimpleORMTests.Entities;
using SimpleORMTests.Helper;

namespace SimpleORMTests
{
	[TestClass]
	public class QueryTests
	{
		#region Constants
		private const string DATABASE_PATH = @"..\\..\\Database\Database.mdf";
		private const string CONNECTION_STRING = @"Data Source=(LocalDB)\v11.0;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30";
		private const string QUERIES_DIRECTORY = "..\\..\\Queries";		
		#endregion

		#region Helpers
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
		#endregion

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

		// TODO after insert, edit, delete each tests... truncate each database table and reinsert data
		// use the TestData class methods to get the data for the tables

		// TODO setup tests that exercise specific exception messages or types
		// i.e. can't find a column in an entity when populating an entity
		// or can't find a column in a database table when executing a query

		[TestMethod]
		public void SelectTest_Simple()
		{
			using (var qry = new Query<Customer>(GetConnectionString()))
			{
				var result = qry.SelectToList();
				var methodName = MethodBase.GetCurrentMethod().Name;
				var expectedQuery = GetQueryText(methodName);
				Assert.IsTrue(CompareText(expectedQuery, qry.QueryStatement.SqlQuery, methodName));
				Assert.IsTrue(TestData.GetCustomers().ItemsAreEqualTo(result));
			}
		}
		[TestMethod]
		public void SelectTest_WithWhere()
		{
			using (var qry = new Query<Customer>(GetConnectionString()))
			{
				var result = qry.Where(c => c.CustomerID == 1).SelectToSingle();
				var methodName = MethodBase.GetCurrentMethod().Name;
				var expectedQuery = GetQueryText(methodName);
				Assert.IsTrue(CompareText(expectedQuery, qry.QueryStatement.SqlQuery, methodName));
				Assert.IsTrue(result is Customer);
				Assert.AreEqual(1, result.CustomerID);
				Assert.AreEqual("Test Customer 1", result.Name);
				Assert.AreEqual("555-555-5555", result.Phone);
			}

			// c.CustomerID == 1
			// c.CustomerID != 1
			// c.CustomerID > 1
			// c.CustomerID < 1
			// c.CustomerID >= 1
			// c.CustomerID <= 1

			// TODO string where operators

			// contains
			// ==
			// !=
			// starts with
			// ends with

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

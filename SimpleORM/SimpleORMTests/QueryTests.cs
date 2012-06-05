using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleORM;
using SimpleORMTests.Entities;

namespace SimpleORMTests
{
	[TestClass]
	public class QueryTests
	{
		[TestMethod]
		public void SelectTest()
		{
			// TODO put the expected queries into a set of text files???
			var expectedQuery = @"
select CustomerID, Name, Phone
from Customer;
";
			using (var qry = new Query())
			{
				qry.Select<Customer>();
				var result = qry.ExecuteToList();
				Assert.AreEqual(expectedQuery, qry.SqlQuery);
				Assert.AreEqual(2, result.Count);
				// TODO need a helper method to compare collections
			}
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

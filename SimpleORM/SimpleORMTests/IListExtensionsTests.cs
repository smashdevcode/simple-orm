using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleORM.Extensions;
using SimpleORMTests.Entities;
using SimpleORMTests.Helper;

namespace SimpleORMTests
{
	[TestClass]
	public class IListExtensionsTests
	{
		[TestMethod]
		public void ItemsAreEqualToTest_Success()
		{
			var listOne = TestData.GetCustomers();
			var listTwo = TestData.GetCustomers();
			Assert.IsTrue(listOne.ItemsAreEqualTo(listTwo));
		}
		[TestMethod]
		public void ItemsAreEqualToTest_ItemCountFailure()
		{
			var listOne = TestData.GetCustomers();
			var listTwo = TestData.GetCustomers();
			listTwo.RemoveAt(0);
			Assert.IsFalse(listOne.ItemsAreEqualTo(listTwo));
		}
		[TestMethod]
		public void ItemsAreEqualToTest_PropertyValueMismatchFailure()
		{
			var listOne = TestData.GetCustomers();
			var listTwo = TestData.GetCustomers();
			listTwo[0].CustomerID = 3;
			Assert.IsFalse(listOne.ItemsAreEqualTo(listTwo));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleORMTests.Entities;

namespace SimpleORMTests.Helper
{
	public static class TestData
	{
		internal static List<Customer> GetCustomers()
		{
			return new List<Customer>()
			{
				new Customer() { CustomerID = 1, Name = "Test Customer 1", Phone = "555-555-5555" },
				new Customer() { CustomerID = 2, Name = "Test Customer 2", Phone = "666-666-6666" },
			};
		}
	}
}

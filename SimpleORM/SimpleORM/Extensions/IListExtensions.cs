using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORM.Extensions
{
	public static class IListExtensions
	{
		public static bool ItemsAreEqualTo<T>(this IList<T> list, 
			IList<T> listToCompare) where T : class
		{
			// if the list counts are not equal, return false
			if (list.Count != listToCompare.Count)
				return false;

			// get the type
			var itemType = typeof(T);
			// get the properties
			var itemProperties = itemType.GetProperties().Where(p => p.CanRead).ToList();

			// loop through the list items...
			for (int itemIndex = 0; itemIndex < list.Count; itemIndex++)
			{
				// loop through the item properties...
				foreach (var itemProperty in itemProperties)
				{
					// get the property values
					var propertyValue = itemProperty.GetValue(list[itemIndex], null);
					var propertyValueToCompare = itemProperty.GetValue(listToCompare[itemIndex], null);
					// if the properties are not equal, then return false
					if ((propertyValue != null && propertyValueToCompare == null) ||
						(propertyValue == null && propertyValueToCompare != null) ||
						(!propertyValue.Equals(propertyValueToCompare)))
					{
						return false;
					}
				}
			}

			// if we got this far, then return true
			return true;
		}
	}
}

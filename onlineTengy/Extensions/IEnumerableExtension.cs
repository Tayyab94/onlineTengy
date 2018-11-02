using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectlistItem<T>(this IEnumerable<T> items,int selectdValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyVale("Name"),  //add another method to Get GetPropertyValue
                       Value = item.GetPropertyVale("Id"),
                       Selected = item.GetPropertyVale("Id").Equals(selectdValue.ToString())
                   };
        }
    }
}

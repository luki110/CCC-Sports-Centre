﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Extensions
{
    // extensions method class should be static 
    public static class IEnumerableExtensions
    {
        // all methods should also be static for extension methods


        // we are converting IEnumerable of class/activity category to selectListItem
        // first argument of extention method should be of extended class proceded by "this" keyword
        // second argument since it is a drop down list we will have an integer "selectedValue"
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            // passing the collection that we have in IEnumerable;  using linq 
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Extensions
{
    public static class ReflectionExtension
    {
        public static string GetPropertyValue<T>(this T item, string PropertName)
        {
            return item.GetType().GetProperty(PropertName).GetValue(item, null).ToString();
        }
    }
}

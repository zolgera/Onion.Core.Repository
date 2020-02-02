using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repository.Extensions
{
    public static class StringHelpers
    {
        public static string ToPascalCase(this string soruce)
        {
            if (!string.IsNullOrEmpty(soruce) && soruce.Length > 1)
            {
                return Char.ToUpperInvariant(soruce[0]) + soruce.Substring(1);
            }
            return soruce;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Utility
{
    //This static class is used as a common point to default roles, default dir storing location and magic strings i.e. "SD.ssCartCount "
    public static class SD
    {
        public const string DefaultFoodImage = "default_food.png";
        public const string ManageUser = "Manager";
        public const string KitchenUser = "Chef";
        public const string FrontDeskUser = "Cashier";
        public const string CustomerUser = "Customer";
        public const string ssCartCount  = "ssCartCount ";

        //StackOverFlow code snippit
        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}

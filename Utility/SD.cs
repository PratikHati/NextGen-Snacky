using NextGen_Snacky.Models;
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
        public const string ssCartCount  = "ssCartCount";
        public const string ssCouponCode = "ssCouponCode";

        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready for Pickup ";
        public const string StatusSubmitted = "Submitted";


        public const string PaymentStatusCompleted = "Approved";
        public const string PaymentStatusCancelled = "Rejected";
        public const string PaymentStatusPending = "Pending";


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

        public static double DiscountedPrice(Coupon couponfromDB, double originaltotalorder)
        {
            if(couponfromDB == null)
            {
                return originaltotalorder;      // no coupon code
            }
            else
            {
                if(couponfromDB.MinimumAmount < originaltotalorder)
                {
                    //return discounted price
                    if (couponfromDB.CouponType.Contains("0"))                                     // % case
                    {
                        originaltotalorder = Math.Round(originaltotalorder - (originaltotalorder * (couponfromDB.Discount / 100)),2);

                        return originaltotalorder;
                    }
                    else
                    {
                        originaltotalorder = Math.Round(originaltotalorder - couponfromDB.Discount,2);                          //$ case

                        return originaltotalorder; 
                    }
                }
                else
                {
                    return originaltotalorder;      //minimum has not meet
                }
            }
        }
    }
}

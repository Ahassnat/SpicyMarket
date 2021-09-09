using SpicyMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpicyMarket.Utility
{
    //Static Defention 
    // User Role 
    public static class SD
    {
        #region Role Name
        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "Front Desk";
        public const string CustomerEndUser = "Customer";
        #endregion

        #region Sessions Name 
        public const string ShoppingCartCount = "ShoppingCartCount";
        public const string ssCouponCode = "CouponCode";
        #endregion

        #region Order Status
        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Begin Prepared";
        public const string StatusReady = "Ready to Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        #endregion

        #region Payment Status
        public const string PaymetStatuPending = "Pending";
        public const string PaymetStatuApproved = "Approved";
        public const string PaymetStatuRejected = "Rejected";
        #endregion


        #region Areas Name 
        public const string Admin = "Admin";
        public const string Customer = "Customer";
        #endregion

        #region Controllers Name 
        public const string Categories = "Admin";
        public const string Coupons = "Customer";
        public const string MenuItems = "MenuItems";
        public const string SubCategories = "SubCategories";
        #endregion

        #region Actions Name 
        public const string Index = "Index";
        #endregion

        #region Discount Price
        public static double DiscountPrice(Coupon coupon, double orderTotalOrginal)
        {
            if (coupon == null)
            {
                return Math.Round(orderTotalOrginal, 2);
            }
            else
            {
                if (coupon.MinimumAmount > orderTotalOrginal)
                {
                    return Math.Round(orderTotalOrginal, 2);
                }
                else
                {
                    if (int.Parse(coupon.CouponType) == (int)Coupon.ECouponType.Dollar)
                    {
                        return Math.Round((orderTotalOrginal - coupon.Discount), 2);
                    }
                    else
                    {
                        return Math.Round(orderTotalOrginal - (orderTotalOrginal * (coupon.Discount / 100)), 2);
                    }
                }
               

            }
        }
        #endregion

    }
}

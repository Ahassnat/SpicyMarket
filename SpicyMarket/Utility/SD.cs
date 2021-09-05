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




    }
}

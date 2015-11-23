using GaryBookstore.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GaryBookstore.Web.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; } 
    }
}
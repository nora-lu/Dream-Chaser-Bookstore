using GaryBookstore.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GaryBookstore.Web.Models
{
    public partial class ShoppingCart
    {
        GaryBookstoreEntities dbc = new GaryBookstoreEntities();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Book book, int qty)
        {
            // Get the matching cart and book instances
            var cartItem = dbc.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.BookId == book.BookId);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    BookId = book.BookId,
                    CartId = ShoppingCartId,
                    Count = qty,
                    DateCreated = DateTime.Now
                };
                dbc.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count += qty;
            }
            // Save changes
            dbc.SaveChanges();
        }

        public int UpdateCart(int id, int qty)
        {
            var cartItem = dbc.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.BookId == id);
            int itemCount = 0;
            if (cartItem != null)
            {
                cartItem.Count = qty;
                itemCount = cartItem.Count;
                dbc.SaveChanges();
            }
            return itemCount;
        }

        public bool RemoveFromCart(int id)
        {
            var cartItem = dbc.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.BookId == id);
            //int itemCount = 0;
            //if (cartItem != null)
            //{
            //    if (cartItem.Count > 1)
            //    {
            //        cartItem.Count--;
            //        itemCount = cartItem.Count;
            //    }
            //    else
            //    {
            //        dbc.Carts.Remove(cartItem);
            //    }
            //    dbc.SaveChanges();
            //}
            if (cartItem != null)
            {
                dbc.Carts.Remove(cartItem);
                dbc.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void EmptyCart()
        {
            var cartItems = dbc.Carts.Where(
                cart => cart.CartId == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                dbc.Carts.Remove(cartItem);
            }
            dbc.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return dbc.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            int? count = (from cartItems in dbc.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            decimal? total = (from cartItems in dbc.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Book.Inventory.RetailPrice).Sum();
            return total ?? decimal.Zero;
        }

        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;
            var cartItems = GetCartItems();
            foreach (var item in cartItems)
            {

                var orderDetail = new OrderDetail
                {
                    BookId = item.BookId,
                    OrderId = order.Id,
                    UnitPrice = item.Book.ListPrice,
                    Quantity = item.Count
                };
                dbc.OrderDetails.Add(orderDetail);
                orderTotal += (item.Count * item.Book.ListPrice);
            }
            order.Total = orderTotal;
            dbc.Orders.Add(order);
            dbc.SaveChanges();
            // dbc.SaveChanges();
            EmptyCart();
            return order.Id;
        }

        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                dbc.ClearAbandonedCart();
                dbc.SaveChanges();
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            } 
            return context.Session[CartSessionKey].ToString();
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = dbc.Carts.Where(
                c => c.CartId == ShoppingCartId);
            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            dbc.SaveChanges();
        }
    }
}
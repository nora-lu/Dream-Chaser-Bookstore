using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GaryBookstore.Web.Models;
using GaryBookstore.Web.Entities;
using GaryBookstore.Web.ViewModels;

namespace GaryBookstore.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private GaryBookstoreEntities dbc = new GaryBookstoreEntities();

        
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            return View(viewModel);
        }

        // TODO: Add stock checking before adding to cart.
        public ActionResult AddToCart(int id, int qty)
        {
            var singleBook = dbc.Books.SingleOrDefault(b => b.BookId == id);

            // For now we allow adding up to 30 books each item, regardless of real inventory
            if (singleBook == null || qty > 30)
            //if (singleBook == null || qty > singleBook.Inventory.Quantity)
            {
                return View("Error");
            }
            else
            {
                var cart = ShoppingCart.GetCart(this.HttpContext);
                cart.AddToCart(singleBook, qty);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            string bookName = dbc.Carts.Single(item => item.BookId == id).Book.Title;
            bool removed = cart.RemoveFromCart(id);

            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(bookName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                Removed = removed,
                DeleteId = id
            };
            return Json(results);
        }

        [HttpPost]
        public ActionResult UpdateCart(int id, int qty)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            string bookName = dbc.Carts.Single(item => item.BookId == id).Book.Title;
            int itemCount = cart.UpdateCart(id, qty);

            var results = new ShoppingCartUpdateViewModel
            {
                Message = "The quantity of " + Server.HtmlEncode(bookName) + " has been updated.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                ItemId = id
            };
            return Json(results);
        }

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}
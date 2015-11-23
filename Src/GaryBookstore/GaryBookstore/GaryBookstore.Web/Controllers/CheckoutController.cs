using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GaryBookstore.Web.Entities;
using GaryBookstore.Web.Models;

namespace GaryBookstore.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private GaryBookstoreEntities dbc = new GaryBookstoreEntities();
        // GET: Checkout
        public ActionResult AddressAndPayment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new Order();
            TryUpdateModel(order);

            order.UserName = User.Identity.Name;
            order.Date = DateTime.Now;
            order.Status = "Submitted";

            var cart = ShoppingCart.GetCart(this.HttpContext);
            cart.CreateOrder(order);

            // Save order
            //dbc.Orders.Add(order);
            //dbc.SaveChanges();
           
            

            return RedirectToAction("Complete", new { id = order.Id });
        }

        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = dbc.Orders.Any(
                o => o.Id == id &&
                o.UserName == User.Identity.Name);
            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
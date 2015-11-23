using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GaryBookstore.Web.Entities;

namespace GaryBookstore.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Book> featuredBooks = new List<Book>();
            List<int> ids = new List<int>(new int[] { 96, 110, 515 });
            using (GaryBookstoreEntities dbc = new GaryBookstoreEntities())
            {
                foreach (var id in ids)
                {
                    Book book = dbc.Books.SingleOrDefault(b => b.BookId == id);
                    featuredBooks.Add(book);
                }
            }
            return View(featuredBooks);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GaryBookstore.Web.Models;
using GaryBookstore.Web.Entities;

namespace GaryBookstore.Web.Controllers
{
    public class BookController : Controller
    {
        private const int bookPerPage = 32;
        public ActionResult ViewBooks(int pageNumber)
        {
            List<Book> allBooks = null;
            int size = 0;
            using (GaryBookstoreEntities dbc = new GaryBookstoreEntities())
            {
                size = dbc.Books.Count();
                if (size <= (pageNumber - 1) * bookPerPage)
                {
                    return View("Error");
                }
                int max = Math.Min(size, pageNumber * bookPerPage);
                allBooks = dbc.Books.Include("Inventory").Where(b => b.BookId >= (pageNumber - 1) * bookPerPage + 1 &&
                    b.BookId <= max).ToList();
            }
            ViewData["maxPage"] = (size - 1) / bookPerPage + 1;
            ViewData["curPage"] = pageNumber;
            return View("BookList", allBooks);
        }

        [HttpGet]
        public ActionResult ViewSingleBook(int id)
        {
            Book targetBook = null;
            string authorNames = null;
            using (GaryBookstoreEntities dbc = new GaryBookstoreEntities())
            {
                targetBook = dbc.Books.Include("Inventory").SingleOrDefault(b => b.BookId == id);
                List<BookAuthor> authors = dbc.BookAuthors.Where(b => b.BookId == id).ToList();
                for (int i = 0; i < authors.Count; i++)
                {
                    BookAuthor author = authors[i];
                    authorNames += dbc.Authors.SingleOrDefault(a => a.Id == author.AuthorId).Name;
                    if (i < authors.Count() - 1)
                    {
                        authorNames += ", ";
                    }
                }
            }
            if (targetBook == null)
            {
                return View("Error");
            }
            else
            {
                ViewData["authors"] = authorNames;
                return View("BookDetails", targetBook);
            }
        }

    }
}
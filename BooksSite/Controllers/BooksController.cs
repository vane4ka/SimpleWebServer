using BooksSite.Services;
using SimpleWebServer.Attributes;
using System.Text;

namespace BooksSite.Controllers
{
    public class BooksController
    {
        private IBookService bookService;

        public BooksController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        [HttpMethod("GET")]
        public string All()
        {
            StringBuilder str = new StringBuilder();
            str.Append("<h3>Books:</h3>");
            str.Append("<ul>");
            foreach(Book b in bookService.GetBooks())
            {
                str.Append($"<li>{b.Id}) {b.Title} {b.Year} <a href='/books/get?id={b.Id}'>more</a></li>");
            }
            str.Append("</ul>");
            return str.ToString();
        }

        [HttpMethod("GET")]
        public string Get(int id)
        {
            Book book = bookService.GetBook(id);
            if (book == null) return "<p>no book</p>";
            return $"<h3>Book info:</h3>" +
                $"<p>ID: {book.Id}</p>" +
                $"<p>TITLE: {book.Title}</p>" +
                $"<p>YEAR: {book.Year}</p>";
        }
    }
}

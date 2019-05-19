using BooksSite.Services;
using SimpleWebServer.Attributes;
using SimpleWebServer.Controllers;
using SimpleWebServer.Exceptions;
using System.Text;

namespace BooksSite.Controllers
{
    public class BooksController : BaseController
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
            if (book == null) throw new NotFoundException();
            return $"<h3>Book info:</h3>" +
                $"<p>ID: {book.Id}</p>" +
                $"<p>TITLE: {book.Title}</p>" +
                $"<p>YEAR: {book.Year}</p>" +
                $"<br/><a href='/books/all'>back to all books</a>";
        }

        [Auth]
        [HttpMethod("GET")]
        public string Add()
        {
            return "<h3>New book</h3>" +
                "<form method='POST' action='/books/addbook'>" +
                "<label>Title: </label><input type='text' name='title'/><br/>" +
                "<label>Year: </label><input type='number' name='year'/><br/>" +
                "<input type='submit' value='Add'/>" +
                "</form>";
        }

        [Auth]
        [HttpMethod("POST")]
        public string AddBook(string title, int year)
        {
            bookService.AddBook(title, year);
            StringBuilder str = new StringBuilder();
            str.Append("<h3>Books:</h3>");
            str.Append("<ul>");
            foreach (Book b in bookService.GetBooks())
            {
                str.Append($"<li>{b.Id}) {b.Title} {b.Year} <a href='/books/get?id={b.Id}'>more</a></li>");
            }
            str.Append("</ul>");
            str.Append("<a href='/books/add'>add new book</a>");
            return str.ToString();
        }
    }
}

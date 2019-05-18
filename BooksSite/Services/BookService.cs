using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksSite.Services
{
    public class BookService : IBookService
    {
        private static List<Book> books = new List<Book>()
        {
            new Book() { Id = 1, Title = "Book 1", Year = 1996 },
            new Book() { Id = 2, Title = "Book 2", Year = 1997 },
            new Book() { Id = 3, Title = "Book 3", Year = 1998 },
            new Book() { Id = 4, Title = "Book 4", Year = 1999 },
            new Book() { Id = 5, Title = "Book 5", Year = 2000 },
        };

        public List<Book> GetBooks()
        {
            return books;
        }

        public Book GetBook(int id)
        {
            return books.Find(b => b.Id == id);
        }

        public Book AddBook(string title, int year)
        {
            Book book = new Book()
            {
                Id = books.Max(b => b.Id) + 1,
                Title = title,
                Year = year
            };
            books.Add(book);
            return book;
        }
    }
}

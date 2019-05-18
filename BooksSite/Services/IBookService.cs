using System.Collections.Generic;

namespace BooksSite.Services
{
    public interface IBookService
    {
        List<Book> GetBooks();
        Book GetBook(int id);
        Book AddBook(string title, int year);
    }
}
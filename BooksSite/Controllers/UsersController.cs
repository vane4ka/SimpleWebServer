using SimpleWebServer.Attributes;
using SimpleWebServer.Controllers;
using System.Net;

namespace BooksSite.Controllers
{
    public class UsersController : BaseController
    {
        [HttpMethod("GET")]
        public string Login()
        {
            return "<h3>Login form</h3>" +
                "<form method='POST' action='/users/loginprocess'>" +
                "<label>Username: </label><input type='text' name='username'/>" +
                "<input type='submit' value='Enter'/>" +
                "</form>";
        }

        [HttpMethod("POST")]
        public string LoginProcess(string username)
        {
            Response.SetCookie(new Cookie("username", username));
            return $"<a href='/users/page'>user page</a><br/>" +
                $"<a href='/books/all'>all books</a><br/>" +
                $"<a href='/users/logout'>logout</a><br/>";
        }
        [Auth]
        [HttpMethod("GET")]
        public string Page()
        {
            string username = Request.Cookies["username"]?.Value ?? "anonim";
            return $"<h3>Hello, {username}!</h3>" +
                $"<a href='/books/all'>all books</a><br/>" +
                $"<a href='/users/logout'>logout</a><br/>"; 
        }

        [Auth]
        [HttpMethod("GET")]
        public string Logout()
        {
            Response.SetCookie(new Cookie("username", "") { Expired = true });
            return $"<a href='/users/login'>login</a><br/>";
        }
    }
}

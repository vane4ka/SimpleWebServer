using BooksSite.Configures;
using SimpleWebServer.Server;
using System;
using System.Diagnostics;

namespace BooksSite
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer webServer = new WebServer("http://127.0.0.1:8084/");
            webServer.Configure<BooksSiteConfigurator>();
            try
            {
                Console.WriteLine("server start...");
                Process.Start("chrome.exe", " http://127.0.0.1:8084/index.html");
                webServer.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine("server stop");
                Console.WriteLine(ex.Message);
            }
        }
    }
}

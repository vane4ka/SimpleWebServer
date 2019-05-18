using System;

namespace SimpleWebServer.Attributes
{
    public class HttpMethodAttribute : Attribute
    {
        public string Method { get; set; }

        public HttpMethodAttribute(string method)
        {
            Method = method.ToUpper();
        }
    }
}

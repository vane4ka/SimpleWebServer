using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using Autofac;
using SimpleWebServer.Exceptions;

namespace SimpleWebServer.Middlewares
{
    public class MvcMiddleware : IMiddleware
    {
        private MiddlewareDelegate next;

        public void SetNext(MiddlewareDelegate next)
        {
            this.next = next;
        }

        public void RequestProcessing(HttpListenerContext context, IContainer container, Dictionary<string, object> data)
        {
            try
            {
                string res = FindAction(context, container, data);
                context.Response.StatusCode = 200;
                using (StreamWriter sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.Write(res);
                }
            }
            catch (MvcException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.Close();
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }

        private string FindAction(HttpListenerContext context, IContainer container, Dictionary<string, object> data)
        {
            HttpListenerRequest request = context.Request;

            string[] segments = request.Url.LocalPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2) throw new NotFoundException();

            string controllerName = segments[0];
            string actionName = segments[1];

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type controllerType = assembly.GetType($"MyWebServer.Controllers.{controllerName}Controller", false, true);
            if (controllerType == null) throw new NotFoundException();

            MethodInfo actionMethod = controllerType.GetMethod(actionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (actionMethod == null) throw new NotFoundException();

            ParameterInfo[] methodParams = actionMethod.GetParameters();
            if (methodParams.Length == 0)
            {
                return actionMethod.Invoke(Activator.CreateInstance(controllerType), null).ToString();
            }

            NameValueCollection queryParams = null;
            if (request.HttpMethod == "GET")
            {
                string query = request.Url.Query;
                if (String.IsNullOrEmpty(query) && actionMethod.GetParameters().Length != 0)
                    throw new BadRequestException();

                if (!String.IsNullOrEmpty(query))
                    queryParams = HttpUtility.ParseQueryString(query);
            }
            else if (request.HttpMethod == "POST")
            {
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    string res = reader.ReadToEnd();
                    queryParams = HttpUtility.ParseQueryString(res);
                }
            }

            if (queryParams == null || queryParams.Count < methodParams.Length)
                throw new BadRequestException();

            List<object> paramToMethod = new List<object>();

            foreach (var p in methodParams)
            {
                paramToMethod.Add(Convert.ChangeType(queryParams[p.Name], p.ParameterType));
            }

            if (paramToMethod.Count != methodParams.Length)
                throw new BadRequestException();

            return actionMethod.Invoke(container.Resolve(controllerType), paramToMethod.ToArray()).ToString();
        }
    }
}

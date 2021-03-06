﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using Autofac;
using SimpleWebServer.Attributes;
using SimpleWebServer.Controllers;
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
                context.Response.ContentType = "text/html";
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
            catch (Exception ex)
            {
                if (ex.InnerException is MvcException innerEx)
                {
                    context.Response.StatusCode = innerEx.StatusCode;
                    context.Response.Close();
                }
                else
                {
                    context.Response.StatusCode = 500;
                    context.Response.Close();
                }
            }
        }

        private string FindAction(HttpListenerContext context, IContainer container, Dictionary<string, object> data)
        {
            HttpListenerRequest request = context.Request;

            string[] segments = request.Url.LocalPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2) throw new NotFoundException();

            string controllerName = segments[0];
            string actionName = segments[1];

            Assembly assembly = Assembly.GetEntryAssembly();
            
            Type controllerType = assembly.GetType($"{data["controllerNS"]}.{controllerName}Controller", false, true);
            if (controllerType == null) throw new NotFoundException();

            MethodInfo actionMethod = controllerType.GetMethod(actionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (actionMethod == null) throw new NotFoundException();

            ParameterInfo[] methodParams = actionMethod.GetParameters();

            HttpMethodAttribute methoAttr = actionMethod.GetCustomAttribute<HttpMethodAttribute>();
            if (methoAttr == null) methoAttr = new HttpMethodAttribute("GET");

            if (methoAttr.Method != request.HttpMethod)
                throw new MethodNotAllowedException();

            List<object> paramToMethod = new List<object>();
            if (methodParams.Length != 0)
            {
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

                foreach (var p in methodParams)
                {
                    paramToMethod.Add(Convert.ChangeType(queryParams[p.Name], p.ParameterType));
                }

                if (paramToMethod.Count != methodParams.Length)
                    throw new BadRequestException();
            }

            AuthAttribute authAttr = actionMethod.GetCustomAttribute<AuthAttribute>();
            if (authAttr != null && (data["isAuth"] == null || Convert.ToBoolean(data["isAuth"]) == false))
                throw new UnauthorizedException();

            BaseController controller = container.Resolve(controllerType) as BaseController;
            controller.Request = context.Request;
            controller.Response = context.Response;
            return actionMethod.Invoke(controller, paramToMethod.ToArray()).ToString();
        }
    }
}

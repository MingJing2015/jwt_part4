using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


namespace jwt.Controllers
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // For CORS stands for cross-origin resource sharing. 
            // In other words, enabling CORS on your server allows you to call a service 
            //from a different domain using JavaScript. 

            config.EnableCors();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

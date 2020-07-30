using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PayLater
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api" + "/{controller}/{param1}/{param2}/{param3}/{param4}/{param5}/{param6}/{param7}",
            defaults: new { param1 = RouteParameter.Optional, param2 = RouteParameter.Optional, param3 = RouteParameter.Optional, param4 = RouteParameter.Optional, param5 = RouteParameter.Optional, param6 = RouteParameter.Optional, param7 = RouteParameter.Optional }
            );
        }
    }
}

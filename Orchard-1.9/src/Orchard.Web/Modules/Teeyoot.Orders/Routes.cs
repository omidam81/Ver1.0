using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Teeyoot.Orders
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }


        public IEnumerable<RouteDescriptor> GetRoutes()
        {

            return new[] {

                new RouteDescriptor {
                    Route = new Route(
                        "Admin/Orders",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Orders"},
                            {"controller", "Home"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Orders"}
                        },
                        new MvcRouteHandler())
                }
      
            };
        }
    }
}
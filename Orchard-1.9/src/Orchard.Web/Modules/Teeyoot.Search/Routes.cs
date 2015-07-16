using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Teeyoot.Search
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
                                "Teeyoot/Search",
                                new RouteValueDictionary {
                                    {"area", "Teeyoot.Search"},
                                    {"controller", "Search"},
                                    {"action", "Index"}                           
                                },
                                new RouteValueDictionary(),
                                new RouteValueDictionary {
                                    {"area", "Teeyoot.Search"}
                                },
                                new MvcRouteHandler())
                        }
                        ,
                        new RouteDescriptor {
                            Route = new Route(
                                "Teeyoot/CategoriesSearch",
                                new RouteValueDictionary {
                                    {"area", "Teeyoot.Search"},
                                    {"controller", "Search"},
                                    {"action", "CategoriesSearch"}                           
                                },
                                new RouteValueDictionary(),
                                new RouteValueDictionary {
                                    {"area", "Teeyoot.Search"}
                                },
                                new MvcRouteHandler())
                }
            };
        }
    }
}
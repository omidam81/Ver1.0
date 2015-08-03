using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Teeyoot.Account
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "Login",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"},
                            {"controller", "Account"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"}
                        },
                        new MvcRouteHandler()
                        )
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GoogleAuth",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"},
                            {"controller", "Account"},
                            {"action", "GoogleAuth"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"}
                        },
                        new MvcRouteHandler()
                        )
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "FacebookAuth",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"},
                            {"controller", "Account"},
                            {"action", "FacebookAuth"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"}
                        },
                        new MvcRouteHandler()
                        )
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Recover/Request",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"},
                            {"controller", "Account"},
                            {"action", "ResetPassword"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"}
                        },
                        new MvcRouteHandler()
                        )
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Recover",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"},
                            {"controller", "Account"},
                            {"action", "Recover"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Account"}
                        },
                        new MvcRouteHandler()
                        )
                }
            };
        }
    }
}
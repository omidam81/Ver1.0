using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Teeyoot.Messaging
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
                        "UpdateSetting",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Messaging"},
                            {"controller", "AdminMessage"},
                            {"action", "UpdateSetting"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Messaging"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/MailChimpSettings",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Messaging"},
                            {"controller", "AdminMessage"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Messaging"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/MailChimpSettings/Add",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Messaging"},
                            {"controller", "AdminMessage"},
                            {"action", "AddSetting"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Messaging"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/Messages",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Messaging"},
                            {"controller", "AdminMessageContent"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Messaging"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
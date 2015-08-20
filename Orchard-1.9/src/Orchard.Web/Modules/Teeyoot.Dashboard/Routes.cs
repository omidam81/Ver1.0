using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Teeyoot.Module.Dashboard
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
                        "Stores/{url}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Dashboard"},
                            {"controller", "Dashboard"},
                            {"action", "ViewStorefront"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Dashboard"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Dashboard/Shop/New",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Dashboard"},
                            {"controller", "Dashboard"},
                            {"action", "NewStorefront"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Dashboard"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Campaign/Edit/{id}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Dashboard"},
                            {"controller", "Dashboard"},
                            {"action", "EditCampaign"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Dashboard"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "GetStarted",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Dashboard/{action}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Dashboard"},
                            {"controller", "Dashboard"},
                            {"action", "Campaigns"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Dashboard"}
                        },
                        new MvcRouteHandler())
                }
                
            };
        }
    }
}
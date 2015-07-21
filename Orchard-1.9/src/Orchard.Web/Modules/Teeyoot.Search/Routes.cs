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
                                "TSearch",
                                new RouteValueDictionary {
                                    {"area", "Teeyoot.Search"},
                                    {"controller", "Search"},
                                    {"action", "Search"}                           
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
                                "CategoriesSearch",
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
                ,
                        new RouteDescriptor {
                            Route = new Route(
                                "Scroll",
                                new RouteValueDictionary {
                                    {"area", "Teeyoot.Search"},
                                    {"controller", "InfiniteScrollDemo"},
                                    {"action", "Index"}                           
                                },
                                new RouteValueDictionary(),
                                new RouteValueDictionary {
                                    {"area", "Teeyoot.Search"}
                                },
                                new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/Categories",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Search"},
                            {"controller", "AdminSearch"},
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
                        "Admin/EditCategory",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Search"},
                            {"controller", "AdminSearch"},
                            {"action", "EditCategory"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Search"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/AddCampaign",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Search"},
                            {"controller", "AdminSearch"},
                            {"action", "AddCampaignForCategory"}                           
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
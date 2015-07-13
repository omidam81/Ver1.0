﻿using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Teeyoot.Module
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
                        "Admin/FAQ",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"},
                            {"controller", "FaqAdmin"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/FAQ/Add",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"},
                            {"controller", "FaqAdmin"},
                            {"action", "AddFaqEntry"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/FAQ/Edit/{id}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"},
                            {"controller", "FaqAdmin"},
                            {"action", "EditFaqEntry"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"}
                        },
                        new MvcRouteHandler())
                },
               
                new RouteDescriptor {
                    Route = new Route(
                        "FAQ",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"},
                            {"controller", "Home"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "FAQ/Topic/{topicId}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"},
                            {"controller", "Home"},
                            {"action", "ViewTopic"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"}
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Route = new Route(
                        "FAQ/Section/{sectionId}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"},
                            {"controller", "Home"},
                            {"action", "ViewSection"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Teeyoot",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                }
                ,
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
                }
                ,
                new RouteDescriptor {
                    Route = new Route(
                        "Dashboard/{controller}/{action}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Campaigns"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                }




                ,
                new RouteDescriptor {
                    Route = new Route(
                        "{campaignName}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Campaign"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
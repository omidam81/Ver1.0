using Orchard.Mvc.Routes;
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
                        "Buy",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "Payment"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
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
                }
                ,
                new RouteDescriptor {
                    Route = new Route(
                        "LaunchCampaign",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "LaunchCampaign"}                           
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
                        "UpoadArtFile",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "UpoadArtFile"}                           
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
                        "GetAllFonts",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "GetFonts"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                }
                ,

                
                new RouteDescriptor {
                    Priority = -11,
                    Route = new Route(
                        "{campaignName}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Campaign"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary {
                            {"campaignName", new ExpectedValuesConstraint("Admin")}
                        },
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                }
                ,
                new RouteDescriptor {
                    Priority = -11,
                    Route = new Route(
                        "Admin/Cost",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminCost"},
                            {"action", "Index"}                           
                        },
                        new RouteValueDictionary {
                            {"campaignName", new ExpectedValuesConstraint("Admin")}
                        },
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        public class ExpectedValuesConstraint : IRouteConstraint
        {
            private readonly string[] _values;

            public ExpectedValuesConstraint(params string[] values)
            {
                _values = values;
            }

            public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
            {
                return !_values.Contains(values[parameterName].ToString(), StringComparer.InvariantCultureIgnoreCase);
            }
        }
    }
}
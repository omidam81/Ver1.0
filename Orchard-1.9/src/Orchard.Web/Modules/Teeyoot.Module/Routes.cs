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
                        "Admin/MailChimpSettings",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminMessage"},
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
                        "Admin/MailChimpSettings/Add",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminMessage"},
                            {"action", "AddSetting"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
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
                        "FAQ/Search",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.FAQ"},
                            {"controller", "Home"},
                            {"action", "GetDetailSearch"}                           
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
                        "Dashboard/{action}",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Dashboard"},
                            {"action", "Campaigns"}                           
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
                        "Teeyoot/Search",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Search"},
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
                        "Teeyoot/CategoriesSearch",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Search"},
                            {"action", "CategoriesSearch"}                           
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
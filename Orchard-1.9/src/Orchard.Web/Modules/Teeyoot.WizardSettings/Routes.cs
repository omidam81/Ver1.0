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
                        "Admin/Fonts",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "AdminWizard"},
                            {"action", "FontList"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                }
                ,
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/Colors",
                        new RouteValueDictionary {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "AdminWizard"},
                            {"action", "ColorList"}                           
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                }
                       
            };
        }
    }
}
using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Teeyoot.FeaturedCampaigns
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
                        "Admin/FeaturedCampaigns",
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
            };
        }
    }
}
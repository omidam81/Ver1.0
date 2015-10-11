using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

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
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/FeaturedCampaigns",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"},
                            {"controller", "AdminFeaturedCampaigns"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/ExportPrints",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"},
                            {"controller", "AdminExportPrints"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/ApproveCampaigns",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"},
                            {"controller", "AdminApproveCampaigns"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/CampaignsSettings",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"},
                            {"controller", "AdminCampaignsSettings"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/GetCampaigns",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"},
                            {"controller", "AdminCampaignsSettings"},
                            {"action", "GetCampaigns"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.FeaturedCampaigns"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
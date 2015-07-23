using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace RM.QuickLogOn.OAuth 
{
    public abstract class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes()) routes.Add(routeDescriptor);
        }

        public abstract IEnumerable<RouteDescriptor> GetRoutes();
    }

    [OrchardFeature("RM.QuickLogOn.OAuth")]
    public class GoogleRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/GGAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"}, {"controller", "GoogleOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary (),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"} },
                        new MvcRouteHandler())
                }
            };
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Facebook")]
    public class FacebookRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/FBAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"}, {"controller", "FacebookOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary (),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"} },
                        new MvcRouteHandler())
                }
            };
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.LiveID")]
    public class LiveIDRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/LIDAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"}, {"controller", "LiveIDOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary (),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"} },
                        new MvcRouteHandler())
                }
            };
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/TWAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"}, {"controller", "TwitterOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary (),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"} },
                        new MvcRouteHandler())
                }
            };
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.LinkedIn")]
    public class LinkedInRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/LINAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"}, {"controller", "LinkedInOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary (),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth"} },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
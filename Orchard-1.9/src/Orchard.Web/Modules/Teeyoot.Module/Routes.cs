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
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Currencies/Edit/{id}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminCurrencies"},
                            {"action", "EditCurrency"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Currencies/Delete/{id}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminCurrencies"},
                            {"action", "DeleteCurrency"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Currencies/Add",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminCurrencies"},
                            {"action", "AddCurrency"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Users",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminUser"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Users/Edit/{userId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminUser"},
                            {"action", "EditUser"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Buy/{orderId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "Payment"},
                            {"promo", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "ReservationComplete/{campaignId}/{sellerId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "ReservationComplete"},
                            {"promo", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GetStarted",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Oops",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "Oops"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "TrackOrder",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "TrackOrder"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "TrackOrder/Recover",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "RecoverOrder"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "TrackOrder/{orderId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "OrderTracking"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "LaunchCampaign",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "LaunchCampaign"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "UpoadArtFile",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "UpoadArtFile"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GetAllFonts",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "GetFonts"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GetArts",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "GetArts"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GetRandomArts",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "GetRandomArts"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GetAllProducts",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "GetProducts"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GetAllProductsAsync",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "GetProductsAsync"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GetAllSwatches",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "GetSwatches"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/CommonSettings",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminCommonSettings"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Cost",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminCost"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/CostEdit",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminCost"},
                            {"action", "Edit"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Priority = -11,
                    Route = new Route(
                        "{campaignName}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Campaign"},
                            {"action", "Index"},
                            {"promo", UrlParameter.Optional}
                        },
                        new RouteValueDictionary
                        {
                            {"campaignName", new ExpectedValuesConstraint("Admin")}
                        },
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "GetSettings",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "GetSettings"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Promotions",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminPromotions"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/TextTranslation",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminTranslationText"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/EdtTextForTranslation",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminTranslationText"},
                            {"action", "EditTextForLocalization"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/SaveTextForTransaltion",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "AdminTranslationText"},
                            {"action", "SaveText"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "ChangeCountry",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.Module"},
                            {"controller", "Home"},
                            {"action", "ChangeCountryAndCulture"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
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

            public bool Match(
                HttpContextBase httpContext,
                Route route,
                string parameterName,
                RouteValueDictionary values,
                RouteDirection routeDirection)
            {
                return !_values.Contains(values[parameterName].ToString(), StringComparer.InvariantCultureIgnoreCase);
            }
        }
    }
}
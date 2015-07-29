using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Teeyoot.WizardSettings
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {

            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Fonts",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "AdminWizard"},
                            {"action", "FontList"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Colours/Product/Add",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Colour"},
                            {"action", "AddProductColour"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Colours/Product/Edit/{productColourId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Colour"},
                            {"action", "EditProductColour"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Colours/Swatch/Add",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Colour"},
                            {"action", "AddSwatchColour"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Colours/Swatch/Edit/{swatchColourId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Colour"},
                            {"action", "EditSwatchColour"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Colours/{chooseColourFor}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Colour"},
                            {"action", "Index"},
                            {"chooseColourFor", ""}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Products",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Product"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/Products/Edit/{productId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Product"},
                            {"action", "EditProduct"},
                            {"productId", ""}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
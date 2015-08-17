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
                        "Admin/ProductGroups",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductStyle"},
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
                        "Admin/ProductGroups/Add",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductStyle"},
                            {"action", "AddProductStyle"}
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
                        "Admin/ProductGroups/Edit/{productStyleId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductStyle"},
                            {"action", "EditProductStyle"}
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
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "Admin/ProductImage/Get/{productId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductImage"},
                            {"action", "GetImage"}
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
                        "Admin/ProductImage/Edit",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductImage"},
                            {"action", "EditImage"}
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
                        "Admin/ProductSizes",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductSize"},
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
                        "Admin/ProductSizes/Add",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductSize"},
                            {"action", "AddProductSize"}
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
                        "Admin/ProductSizes/Edit/{productSizeId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductSize"},
                            {"action", "EditProductSize"}
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
                        "Admin/ProductHeadlines",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductHeadline"},
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
                        "Admin/ProductHeadlines/Add",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductHeadline"},
                            {"action", "AddProductHeadline"}
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
                        "Admin/ProductHeadlines/Edit/{productHeadlineId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "ProductHeadline"},
                            {"action", "EditProductHeadline"}
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
                        "Admin/Artworks",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Artwork"},
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
                        "Admin/Artworks/Add",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Artwork"},
                            {"action", "AddArtwork"}
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
                        "Admin/Artworks/Edit/{artworkId}",
                        new RouteValueDictionary
                        {
                            {"area", "Teeyoot.WizardSettings"},
                            {"controller", "Artwork"},
                            {"action", "EditArtwork"}
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
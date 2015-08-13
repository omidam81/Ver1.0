using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.Alias.Implementation.Holder;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization.Services;
using Orchard.Mvc.Routes;
using Orchard.Alias.Implementation.Map;
using Orchard.ContentManagement;
using Orchard.Localization.Models;
using RM.Localization.Services;
using System.Globalization;

namespace RM.Localization
{
    public class LocalizedHomeRoutesProvider : IRouteProvider
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public LocalizedHomeRoutesProvider(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 100,
                    Route = new LocalizedHomeRoute(_workContextAccessor)
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes()) routes.Add(routeDescriptor);
        }
    }

    public class LocalizedHomeRoute : RouteBase {
        private readonly IWorkContextAccessor _workContextAccessor;

        public LocalizedHomeRoute(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // Check if this path is "home"
            if (!string.IsNullOrEmpty(httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2))) return null;

            using (var scope = _workContextAccessor.CreateWorkContextScope(httpContext)) {

                const string area = "Contents";
                var aliasHolder = scope.Resolve<IAliasHolder>();
                var aliasMap = aliasHolder.GetMap(area);

                if (!aliasMap.Any()) return null;

                IDictionary<string, string> routeValues;
                if (!aliasMap.TryGetAlias(string.Empty, out routeValues) || !routeValues.ContainsKey("Id")) return null;

                int contentItemId = int.TryParse(routeValues["Id"], out contentItemId) ? contentItemId : 0;

                var contentManager = scope.Resolve<IContentManager>();

                var contentItem = contentItemId > 0 ? contentManager.Get(contentItemId) : null;
                var localizationPart = contentItem != null ? contentItem.Get<LocalizationPart>() : null;

                if(localizationPart == null) return null;

                var cultureService = scope.Resolve<ICultureService>();
                var currentCulture = cultureService.GetCurrentCulture();

                if (localizationPart.Culture != null && string.Compare(localizationPart.Culture.Culture, currentCulture, StringComparison.OrdinalIgnoreCase) != 0) 
                {
                    var masterContentItem = localizationPart.MasterContentItem != null ? localizationPart.MasterContentItem.ContentItem : contentItem;

                    var localizationService = scope.Resolve<ILocalizationService>();

                    localizationPart = localizationService.GetLocalizedContentItem(masterContentItem, currentCulture);

                    contentItemId = localizationPart != null ? localizationPart.Id : masterContentItem.Id;

                    routeValues["Id"] = contentItemId.ToString(CultureInfo.InvariantCulture.NumberFormat);
                }

                var data = new RouteData(this, new MvcRouteHandler());
                foreach (var routeValue in routeValues) {
                    var key = routeValue.Key;
                    data.Values.Add(key.EndsWith("-") ? key.Substring(0, key.Length - 1) : key, routeValue.Value);
                }

                data.Values["area"] = area;
                data.DataTokens["area"] = area;

                return data;
            }
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
            return null;
        }
    }
}

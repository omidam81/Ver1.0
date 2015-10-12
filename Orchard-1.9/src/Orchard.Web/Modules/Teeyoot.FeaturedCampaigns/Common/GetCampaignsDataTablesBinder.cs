using System.Collections.Specialized;
using System.Web.Mvc;
using DataTables.Mvc;

namespace Teeyoot.FeaturedCampaigns.Common
{
    public class GetCampaignsDataTablesBinder : DataTablesBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return Bind(controllerContext, bindingContext, typeof (GetCampaignsDataTablesRequest));
        }

        protected override void MapAditionalProperties(
            IDataTablesRequest requestModel,
            NameValueCollection requestParameters)
        {
            var request = (GetCampaignsDataTablesRequest) requestModel;
            var filterCurrencyIdString = Get<string>(requestParameters, "FilterCurrencyId");

            int filterCurrencyId;
            var parseResult = int.TryParse(filterCurrencyIdString, out filterCurrencyId);

            if (parseResult)
            {
                request.FilterCurrencyId = filterCurrencyId;
            }
        }
    }
}
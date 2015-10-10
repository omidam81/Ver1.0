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

        protected override void MapAditionalProperties(IDataTablesRequest requestModel, NameValueCollection requestParameters)
        {
            base.MapAditionalProperties(requestModel, requestParameters);
        }
    }
}
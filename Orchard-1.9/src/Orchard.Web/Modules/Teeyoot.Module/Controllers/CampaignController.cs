using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class CampaignController : Controller
    {
        //
        // GET: /Campaign/
        public ActionResult Index(string campaignName)
        {
            //return new EmptyResult();
            return View((object)campaignName);
        }
	}
}
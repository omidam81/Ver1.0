using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Teeyoot.Module.Models;
using MailChimp;
using MailChimp.Campaigns;
using MailChimp.Helper;
using MailChimp.Lists;


namespace Teeyoot.Module.Dashboard.Controllers
{
    [Themed]
    public partial class DashboardController : Controller
    {
        public ActionResult Campaigns()
        {
            return View();
        }
    }
}
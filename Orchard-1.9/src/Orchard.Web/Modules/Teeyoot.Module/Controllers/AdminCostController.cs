using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;
using Teeyoot.Module.Services.Interfaces;
using Orchard.Localization;
using Orchard;
using System.Globalization;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminCostController : Controller
    {
        private readonly ITShirtCostService _costService;
        public Localizer T { get; set; }
        private IOrchardServices Services { get; set; }

        public AdminCostController(ITShirtCostService costService, IOrchardServices services)
        {
            _costService = costService;
            Services = services;
        }

        // GET: AdminCost
        public ActionResult Index()
        {
            TShirtCostRecord cost = _costService.GetCost();

            if (cost == null)
            {
                return View("Index", new AdminCostViewModel { });
            }
            else
            {
                AdminCostViewModel costViewModel = new AdminCostViewModel { AdditionalScreenCosts = cost.AdditionalScreenCosts.ToString(), DTGPrintPrice = cost.DTGPrintPrice.ToString(), FirstScreenCost = cost.FirstScreenCost.ToString(), InkCost = cost.InkCost.ToString(), LabourCost = cost.LabourCost.ToString(), LabourTimePerColourPerPrint = cost.LabourTimePerColourPerPrint, LabourTimePerSidePrintedPerPrint = cost.LabourTimePerSidePrintedPerPrint, PercentageMarkUpRequired = cost.PercentageMarkUpRequired.ToString(), PrintsPerLitre = cost.PrintsPerLitre, SalesGoal = cost.SalesGoal };

                return View("Index", costViewModel);
            }
        }

        public ActionResult Edit()
        {
            TShirtCostRecord cost = _costService.GetCost();
            if (cost == null)
            {
                return View("Edit");
            }
            else
            {
                AdminCostViewModel costViewModel = new AdminCostViewModel { AdditionalScreenCosts = cost.AdditionalScreenCosts.ToString(), DTGPrintPrice = cost.DTGPrintPrice.ToString(), FirstScreenCost = cost.FirstScreenCost.ToString(), InkCost = cost.InkCost.ToString(), LabourCost = cost.LabourCost.ToString(), LabourTimePerColourPerPrint = cost.LabourTimePerColourPerPrint, LabourTimePerSidePrintedPerPrint = cost.LabourTimePerSidePrintedPerPrint, PercentageMarkUpRequired = cost.PercentageMarkUpRequired.ToString(), PrintsPerLitre = cost.PrintsPerLitre, SalesGoal = cost.SalesGoal };

                return View("Edit", costViewModel);
            }
        }

        public ActionResult Save(AdminCostViewModel costViewModel)
        {
            costViewModel = ReplaceAllCost(costViewModel);

            bool dontUpdate = false;
            float flo;
            try
            {
                flo = Convert.ToSingle(costViewModel.AdditionalScreenCosts, new CultureInfo("en-US"));
            }
            catch
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Additional Screen Costs\""));
                dontUpdate = true;
            }
            try
            {
                flo = Convert.ToSingle(costViewModel.DTGPrintPrice, new CultureInfo("en-US"));
            }
            catch
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"DTG print price\""));
                dontUpdate = true;
            }
            try
            {
                flo = Convert.ToSingle(costViewModel.FirstScreenCost, new CultureInfo("en-US"));
            }
            catch
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"1st Screen Cost\""));
                dontUpdate = true;
            }
            try
            {
                flo = Convert.ToSingle(costViewModel.InkCost, new CultureInfo("en-US"));
            }
            catch
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Ink Cost\""));
                dontUpdate = true;
            }
            try
            {
                flo = Convert.ToSingle(costViewModel.LabourCost, new CultureInfo("en-US"));
            }
            catch
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Labour Cost\""));
                dontUpdate = true;
            }
            if (costViewModel.LabourTimePerColourPerPrint == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Labour time per colour per print\""));
                dontUpdate = true;
            }
            if (costViewModel.LabourTimePerSidePrintedPerPrint == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Labour time per side printed per print\""));
                dontUpdate = true;
            }
            try
            {
                flo = Convert.ToSingle(costViewModel.PercentageMarkUpRequired, new CultureInfo("en-US"));
            }
            catch
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Percentage Mark-Up required\""));
                dontUpdate = true;
            }
            if (costViewModel.PrintsPerLitre == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Prints per litre\""));
                dontUpdate = true;
            }
            if (dontUpdate)
            {
                return this.RedirectToAction("Edit");
            }

            TShirtCostRecord cost = _costService.GetCost();

            if (cost == null)
            {
                TShirtCostRecord newCost = new TShirtCostRecord
                {
                    AdditionalScreenCosts = Convert.ToSingle(costViewModel.AdditionalScreenCosts, new CultureInfo("en-US")),
                    DTGPrintPrice = Convert.ToSingle(costViewModel.DTGPrintPrice, new CultureInfo("en-US")),
                    FirstScreenCost = Convert.ToSingle(costViewModel.FirstScreenCost, new CultureInfo("en-US")),
                    InkCost = Convert.ToSingle(costViewModel.InkCost, new CultureInfo("en-US")),
                    LabourCost = Convert.ToSingle(costViewModel.LabourCost, new CultureInfo("en-US")),
                    LabourTimePerColourPerPrint = costViewModel.LabourTimePerColourPerPrint,
                    LabourTimePerSidePrintedPerPrint = costViewModel.LabourTimePerSidePrintedPerPrint,
                    PercentageMarkUpRequired = Convert.ToSingle(costViewModel.PercentageMarkUpRequired, new CultureInfo("en-US")),
                    PrintsPerLitre = costViewModel.PrintsPerLitre,
                    SalesGoal = costViewModel.SalesGoal
                };

                if (_costService.InsertCost(newCost))
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Data update was successfully"));
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("When you try to update the data error occurred. Please, try again later!"));
                }
            }
            else
            {
                cost.AdditionalScreenCosts = Convert.ToSingle(costViewModel.AdditionalScreenCosts, new CultureInfo("en-US"));
                cost.DTGPrintPrice = Convert.ToSingle(costViewModel.DTGPrintPrice, new CultureInfo("en-US"));
                cost.FirstScreenCost = Convert.ToSingle(costViewModel.FirstScreenCost, new CultureInfo("en-US"));
                cost.InkCost = Convert.ToSingle(costViewModel.InkCost, new CultureInfo("en-US"));
                cost.LabourCost = Convert.ToSingle(costViewModel.LabourCost, new CultureInfo("en-US"));
                cost.LabourTimePerColourPerPrint = costViewModel.LabourTimePerColourPerPrint;
                cost.LabourTimePerSidePrintedPerPrint = costViewModel.LabourTimePerSidePrintedPerPrint;
                cost.PercentageMarkUpRequired = Convert.ToSingle(costViewModel.PercentageMarkUpRequired, new CultureInfo("en-US"));
                cost.PrintsPerLitre = costViewModel.PrintsPerLitre;
                cost.SalesGoal = costViewModel.SalesGoal;

                if (_costService.UpdateCost(cost))
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Data update was successfully"));
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("When you try to update the data error occurred. Please, try again later!"));
                }
            }

            return this.RedirectToAction("Index");
        }

        public AdminCostViewModel ReplaceAllCost(AdminCostViewModel cost)
        {
            cost.AdditionalScreenCosts = cost.AdditionalScreenCosts.Replace(",", ".");
            cost.DTGPrintPrice = cost.DTGPrintPrice.Replace(",", ".");
            cost.FirstScreenCost = cost.FirstScreenCost.Replace(",", ".");
            cost.InkCost = cost.InkCost.Replace(",", ".");
            cost.LabourCost = cost.LabourCost.Replace(",", ".");
            cost.PercentageMarkUpRequired = cost.PercentageMarkUpRequired.Replace(",", ".");

            return cost;
        }
    }
}
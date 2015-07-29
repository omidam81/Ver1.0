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
                AdminCostViewModel costViewModel = new AdminCostViewModel { AdditionalScreenCosts = Convert.ToDecimal(cost.AdditionalScreenCosts), CostOfMaterial = Convert.ToDecimal(cost.CostOfMaterial), DTGPrintPrice = Convert.ToDecimal(cost.DTGPrintPrice), FirstScreenCost = Convert.ToDecimal(cost.FirstScreenCost), InkCost = Convert.ToDecimal(cost.InkCost), LabourCost = Convert.ToDecimal(cost.LabourCost), LabourTimePerColourPerPrint = cost.LabourTimePerColourPerPrint, LabourTimePerSidePrintedPerPrint = cost.LabourTimePerSidePrintedPerPrint, PercentageMarkUpRequired = Convert.ToDecimal(cost.PercentageMarkUpRequired), PrintsPerLitre = cost.PrintsPerLitre };

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
                AdminCostViewModel costViewModel = new AdminCostViewModel { AdditionalScreenCosts = Convert.ToDecimal(cost.AdditionalScreenCosts), CostOfMaterial = Convert.ToDecimal(cost.CostOfMaterial), DTGPrintPrice = Convert.ToDecimal(cost.DTGPrintPrice), FirstScreenCost = Convert.ToDecimal(cost.FirstScreenCost), InkCost = Convert.ToDecimal(cost.InkCost), LabourCost = Convert.ToDecimal(cost.LabourCost), LabourTimePerColourPerPrint = cost.LabourTimePerColourPerPrint, LabourTimePerSidePrintedPerPrint = cost.LabourTimePerSidePrintedPerPrint, PercentageMarkUpRequired = Convert.ToDecimal(cost.PercentageMarkUpRequired), PrintsPerLitre = cost.PrintsPerLitre };

                return View("Edit", costViewModel);
            }
        }

        public ActionResult Save(AdminCostViewModel costViewModel)
        {
            bool dontUpdate = false;
            if (costViewModel.AdditionalScreenCosts == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Additional Screen Costs\""));
                dontUpdate = true;
            }
            if (costViewModel.CostOfMaterial == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Cost of material\""));
                dontUpdate = true;
            }
            if (costViewModel.DTGPrintPrice == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"DTG print price\""));
                dontUpdate = true;
            }
            if (costViewModel.FirstScreenCost == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"1st Screen Cost\""));
                dontUpdate = true;
            }
            if (costViewModel.InkCost == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Ink Cost\""));
                dontUpdate = true;
            }
            if (costViewModel.LabourCost == 0)
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
            if (costViewModel.PercentageMarkUpRequired == 0)
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
                    AdditionalScreenCosts = (float)costViewModel.AdditionalScreenCosts,
                    CostOfMaterial = (float)costViewModel.CostOfMaterial,
                    DTGPrintPrice = (float)costViewModel.DTGPrintPrice,
                    FirstScreenCost = (float)costViewModel.FirstScreenCost,
                    InkCost = (float)costViewModel.InkCost,
                    LabourCost = (float)costViewModel.LabourCost,
                    LabourTimePerColourPerPrint = costViewModel.LabourTimePerColourPerPrint,
                    LabourTimePerSidePrintedPerPrint = costViewModel.LabourTimePerSidePrintedPerPrint,
                    PercentageMarkUpRequired = (float)costViewModel.PercentageMarkUpRequired,
                    PrintsPerLitre = costViewModel.PrintsPerLitre
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
                cost.AdditionalScreenCosts = (float)costViewModel.AdditionalScreenCosts;
                cost.CostOfMaterial = (float)costViewModel.CostOfMaterial;
                cost.DTGPrintPrice = (float)costViewModel.DTGPrintPrice;
                cost.FirstScreenCost = (float)costViewModel.FirstScreenCost;
                cost.InkCost = (float)costViewModel.InkCost;
                cost.LabourCost = (float)costViewModel.LabourCost;
                cost.LabourTimePerColourPerPrint = costViewModel.LabourTimePerColourPerPrint;
                cost.LabourTimePerSidePrintedPerPrint = costViewModel.LabourTimePerSidePrintedPerPrint;
                cost.PercentageMarkUpRequired = (float)costViewModel.PercentageMarkUpRequired;
                cost.PrintsPerLitre = costViewModel.PrintsPerLitre;

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

        public ActionResult ValidationForInput(AdminCostViewModel cost)
        {
            if (cost.AdditionalScreenCosts == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Additional Screen Costs\""));
            }
            if (cost.CostOfMaterial == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Cost of material\""));
            }
            if (cost.DTGPrintPrice == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"DTG print price\""));
            }
            if (cost.FirstScreenCost == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"1st Screen Cost\""));
            }
            if (cost.InkCost == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Ink Cost\""));
            }
            if (cost.LabourCost == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Labour Cost\""));
            }
            if (cost.LabourTimePerColourPerPrint == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Labour time per colour per print\""));
            }
            if (cost.LabourTimePerSidePrintedPerPrint == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Labour time per side printed per print\""));
            }
            if (cost.PercentageMarkUpRequired == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Percentage Mark-Up required\""));
            }
            if (cost.PrintsPerLitre == 0)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Incorrect entries in the box \"Prints per litre\""));
            }

            return this.RedirectToAction("Edit");
        }
    }
}
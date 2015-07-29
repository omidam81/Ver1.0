using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminCostController : Controller
    {
        private readonly ITShirtCostService _costService;

        public AdminCostController(ITShirtCostService costService)
        {
            _costService = costService;
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
                AdminCostViewModel costViewModel = new AdminCostViewModel { AdditionalScreenCosts = cost.AdditionalScreenCosts, CostOfMaterial = cost.CostOfMaterial, DTGPrintPrice = cost.DTGPrintPrice, FirstScreenCost = cost.FirstScreenCost, InkCost = cost.InkCost, LabourCost = cost.LabourCost, LabourTimePerColourPerPrint = cost.LabourTimePerColourPerPrint, LabourTimePerSidePrintedPerPrint = cost.LabourTimePerSidePrintedPerPrint, PercentageMarkUpRequired = cost.PercentageMarkUpRequired, PrintsPerLitre = cost.PrintsPerLitre };

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
                AdminCostViewModel costViewModel = new AdminCostViewModel { AdditionalScreenCosts = cost.AdditionalScreenCosts, CostOfMaterial = cost.CostOfMaterial, DTGPrintPrice = cost.DTGPrintPrice, FirstScreenCost = cost.FirstScreenCost, InkCost = cost.InkCost, LabourCost = cost.LabourCost, LabourTimePerColourPerPrint = cost.LabourTimePerColourPerPrint, LabourTimePerSidePrintedPerPrint = cost.LabourTimePerSidePrintedPerPrint, PercentageMarkUpRequired = cost.PercentageMarkUpRequired, PrintsPerLitre = cost.PrintsPerLitre };

                return View("Edit", costViewModel);
            }
        }

        public ActionResult Save(AdminCostViewModel costViewModel)
        {


            return this.RedirectToAction("Index");
        }
    }
}
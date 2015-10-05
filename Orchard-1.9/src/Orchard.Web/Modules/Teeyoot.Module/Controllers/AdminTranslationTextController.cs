using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.ViewModels;
using Teeyoot.Module.Services.Interfaces;
using RM.Localization.Services;
using Orchard.Data;
using Orchard.Localization.Records;
using Orchard;
using System.IO;
using System.Text;
using Orchard.UI.Navigation;
using Orchard.Settings;
using Orchard.DisplayManagement;
using Orchard.Localization;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminTranslationTextController : Controller
    {
        private readonly ICountryService _countryService;
        private readonly ICultureService _cultureService;
        private readonly IRepository<CultureRecord> _cultureRepository;
        private readonly IWorkContextAccessor _wca;
        private IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        private const string pathToTeeyootAccount = "Modules/Teeyoot.Account/App_Data/Localization/";
        private const string pathToTeeyootDashboard = "Modules/Teeyoot.Dashboard/App_Data/Localization/";
        private const string pathToTeeyootFAQ = "Modules/Teeyoot.FAQ/App_Data/Localization/";
        private const string pathToTeeyootFeaturedCampaigns = "Modules/Teeyoot.FeaturedCampaigns/App_Data/Localization/";
        private const string pathToTeeyootMessaging = "Modules/Teeyoot.Messaging/App_Data/Localization/";
        private const string pathToTeeyootModule = "Modules/Teeyoot.Module/App_Data/Localization/";
        private const string pathToTeeyootOrders = "Modules/Teeyoot.Orders/App_Data/Localization/";
        private const string pathToTeeyootPaymentSettings = "Modules/Teeyoot.PaymentSettings/App_Data/Localization/";
        private const string pathToTeeyootPayots = "Modules/Teeyoot.Payouts/App_Data/Localization/";
        private const string pathToTeeyootSearch = "Modules/Teeyoot.Search/App_Data/Localization/";
        private const string pathToTeeyootWizardSettings = "Modules/Teeyoot.WizardSettings/App_Data/Localization/";
        private const string pathToTeebayTheme = "Themes/Teebay.Theme/App_Data/Localization/";
        private const string nameFileToModule = "/orchard.module.po";
        private const string nameFileToTheme = "/orchard.theme.po";

        public AdminTranslationTextController(ICountryService countryService, ICultureService cultureService, IRepository<CultureRecord> cultureRepository, IWorkContextAccessor wca, IOrchardServices services)
        {
            _countryService = countryService;
            _cultureService = cultureService;
            _cultureRepository = cultureRepository;
            _wca = wca;
            Services = services;
        }

        // GET: AdminTranslationText
        public ActionResult Index(AdminTranslationTextViewModel attvm)
        {
            attvm.ActionCountry = _countryService.GetAllCountry().ToList();
            if (attvm.ActionCountryId > 0)
            {
                attvm.ActionCulture = _countryService.GetCultureByCountry(attvm.ActionCountryId).ToList();
                if (attvm.ActionCulture.Count == 0)
                {
                    attvm.NotFoundCulture = true;
                    return View(attvm);
                }
                else
                {
                    attvm.NotFoundCulture = false;
                }
            }
            else
            {
                attvm.ActionCulture = _countryService.GetCultureByCountry(attvm.ActionCountry.First().Id).ToList();
            }

            if (string.IsNullOrEmpty(attvm.SearchString))
            {
                attvm.NotFoundResult = true;
            }
            else
            {
                List<string> result = new List<string>();
                List<string> resultReplace = new List<string>();
                List<string> resultFilePath = new List<string>();

                var cult = _cultureRepository.Table.Where(c => c.Id == attvm.ActionCultureId).First().Culture;
                string tAccountPath = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootAccount + cult + nameFileToModule;
                string tDashboard = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootDashboard + cult + nameFileToModule;
                string tFAQ = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootFAQ + cult + nameFileToModule;
                string tFeaturedCampaigns = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootFeaturedCampaigns + cult + nameFileToModule;
                string tMessaging = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootMessaging + cult + nameFileToModule;
                string tModule = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootModule + cult + nameFileToModule;
                string tOrders = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootOrders + cult + nameFileToModule;
                string tPaymentSettings = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootPaymentSettings + cult + nameFileToModule;
                string tPayots = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootPayots + cult + nameFileToModule;
                string tSearch = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootSearch + cult + nameFileToModule;
                string tWizardSettings = AppDomain.CurrentDomain.BaseDirectory + pathToTeeyootWizardSettings + cult + nameFileToModule;
                string teebayTheme = AppDomain.CurrentDomain.BaseDirectory + pathToTeebayTheme + cult + nameFileToTheme;

                StreamReader strAc = new StreamReader(tAccountPath, Encoding.Default);
                while (!strAc.EndOfStream)
                {
                    string st = strAc.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strDach = new StreamReader(tDashboard, Encoding.Default);
                while (!strDach.EndOfStream)
                {
                    string st = strDach.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strFAQ = new StreamReader(tFAQ, Encoding.Default);
                while (!strFAQ.EndOfStream)
                {
                    string st = strFAQ.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strFeaturedCampaigns = new StreamReader(tFeaturedCampaigns, Encoding.Default);
                while (!strFeaturedCampaigns.EndOfStream)
                {
                    string st = strFeaturedCampaigns.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strMess = new StreamReader(tMessaging, Encoding.Default);
                while (!strMess.EndOfStream)
                {
                    string st = strMess.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strModule = new StreamReader(tModule, Encoding.Default);
                while (!strModule.EndOfStream)
                {
                    string st = strModule.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strOrders = new StreamReader(tOrders, Encoding.Default);
                while (!strOrders.EndOfStream)
                {
                    string st = strOrders.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strPaym = new StreamReader(tPaymentSettings, Encoding.Default);
                while (!strPaym.EndOfStream)
                {
                    string st = strPaym.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strPayots = new StreamReader(tPayots, Encoding.Default);
                while (!strPayots.EndOfStream)
                {
                    string st = strPayots.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strSearch = new StreamReader(tSearch, Encoding.Default);
                while (!strSearch.EndOfStream)
                {
                    string st = strSearch.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strWiz = new StreamReader(tWizardSettings, Encoding.Default);
                while (!strWiz.EndOfStream)
                {
                    string st = strWiz.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                StreamReader strTeeTheme = new StreamReader(teebayTheme, Encoding.Default);
                while (!strTeeTheme.EndOfStream)
                {
                    string st = strTeeTheme.ReadLine();
                    if (st.Contains(attvm.SearchString))
                    {
                        if (st.StartsWith("msgstr"))
                        {
                            result.Add(st.Substring(8, st.Length - 9));
                            resultReplace.Add(st);
                            resultFilePath.Add(tAccountPath);
                        }
                    }
                }
                strAc.Close();
                strDach.Close();
                strFAQ.Close();
                strFeaturedCampaigns.Close();
                strMess.Close();
                strModule.Close();
                strOrders.Close();
                strPaym.Close();
                strPayots.Close();
                strSearch.Close();
                strWiz.Close();
                strTeeTheme.Close();

                attvm.SearchResult = result;

                if (result == null || result.Count == 0)
                {
                    attvm.NotFoundResult = true;
                }
                else
                {
                    attvm.NotFoundResult = false;
                }

                attvm.SearchResultReplace = resultReplace;
                attvm.SearchResultFilePath = resultFilePath;
            }

            return View(attvm);
        }

        public JsonResult GetCultureByCountry(int countryId)
        {
            var model = new AdminTranslationTextViewModel();
            model.ActionCulture = _countryService.GetCultureByCountry(countryId).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditTextForLocalization(string changeText, string replaceText, string filePath, int actionCountry, int actionCulture, string search)
        {
            return View("EditTextForLocalization", new AdminEditTranslationTextViewModel { ChangeText = changeText, ReplaceText = replaceText, FilePath = filePath, ActionCountry = actionCountry, ActionCulture = actionCulture, Search = search });
        }

        public ActionResult SaveText(AdminEditTranslationTextViewModel aettvm)
        {
            try
            {
                StringBuilder sbText = new StringBuilder(System.IO.File.ReadAllText(aettvm.FilePath));
                sbText.Replace(aettvm.ReplaceText, "msgstr \"" + aettvm.ChangeText + "\"");
                System.IO.File.WriteAllText(aettvm.FilePath, sbText.ToString());
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Text upgraded successfully"));
            }
            catch {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("An error occurred while updating the text"));
            }

            return this.RedirectToAction("Index", new AdminTranslationTextViewModel { ActionCountryId = aettvm.ActionCountry, ActionCultureId = aettvm.ActionCulture, SearchString = aettvm.Search });
        }
    }
}
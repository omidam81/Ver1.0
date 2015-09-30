using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Orchard.Users.Services;
using Orchard.Users.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminUserController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly ISiteService _siteService;
        private readonly ShellSettings _shellSettings;
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        private readonly ICampaignService _campaignService;
        private readonly IOrderService _orderService;

        public IOrchardServices Services { get; set; }
        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public AdminUserController(
            IRepository<TeeyootUserPartRecord> teeyootUserRepository,
            IOrderService orderService,
            ICampaignService campaignService,
            IRepository<CurrencyRecord> currencyRepository,
            IMembershipService membershipService,
            ShellSettings shellSettings,
            IOrchardServices services,
            IUserService userService,
            ISiteService siteService,
            IShapeFactory shapeFactory)
        {
            _orderService = orderService;
            _campaignService = campaignService;
            _currencyRepository = currencyRepository;
            _membershipService = membershipService;
            _shellSettings = shellSettings;
            Services = services;
            _siteService = siteService;
            _userService = userService;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        public ActionResult Index(UserIndexOptions options, PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            const string userTotalQuery = " SELECT COUNT(*)" +
                                          " FROM Orchard_Framework_ContentItemVersionRecord ContentItemVersionRecord" +
                                          " JOIN Orchard_Framework_ContentItemRecord ContentItemRecord" +
                                          " ON ContentItemVersionRecord.ContentItemRecord_id = ContentItemRecord.Id" +
                                          " JOIN Orchard_Users_UserPartRecord UserPartRecord" +
                                          " ON ContentItemRecord.Id = UserPartRecord.Id" +
                                          " WHERE ContentItemVersionRecord.Published = 1";

            const string userQuery = " SELECT UserPartRecord.UserName UserName," +
                                     " UserPartRecord.Id UserId," +
                                     " CurrencyRecord.Name CurrencyName," +
                                     " CAST(CASE WHEN TeeyootUserPartRecord.Id IS NOT NULL THEN 1 ELSE 0 END AS BIT) IsTeeyootUser" +
                                     " FROM Orchard_Framework_ContentItemVersionRecord ContentItemVersionRecord" +
                                     " JOIN Orchard_Framework_ContentItemRecord ContentItemRecord" +
                                     " ON ContentItemVersionRecord.ContentItemRecord_id = ContentItemRecord.Id" +
                                     " JOIN Orchard_Users_UserPartRecord UserPartRecord" +
                                     " ON ContentItemRecord.Id = UserPartRecord.Id" +
                                     " LEFT JOIN Teeyoot_Module_TeeyootUserPartRecord TeeyootUserPartRecord" +
                                     " ON UserPartRecord.Id = TeeyootUserPartRecord.Id" +
                                     " LEFT JOIN Teeyoot_Module_CurrencyRecord CurrencyRecord" +
                                     " ON TeeyootUserPartRecord.CurrencyRecord_Id = CurrencyRecord.Id" +
                                     " WHERE ContentItemVersionRecord.Published = 1" +
                                     " ORDER BY UserPartRecord.Id" +
                                     " OFFSET @Offset ROWS" +
                                     " FETCH NEXT @Fetch ROWS ONLY";

            var userItems = new List<UserItemViewModel>();
            int userTotal;

            var offset = pager.Page > 0 ? (pager.Page - 1)*pager.PageSize : 0;
            var fetch = pager.PageSize == 0 ? int.MaxValue : pager.PageSize;

            using (var connection = new SqlConnection(_shellSettings.DataConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = userQuery;

                        var offsetParameter = new SqlParameter("@Offset", SqlDbType.Int) {Value = offset};
                        var fetchParameter = new SqlParameter("@Fetch", SqlDbType.Int) {Value = fetch};

                        command.Parameters.Add(offsetParameter);
                        command.Parameters.Add(fetchParameter);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userItemViewModel = new UserItemViewModel
                                {
                                    Email = (string) reader["UserName"],
                                    UserId = (int) reader["UserId"],
                                    IsTeeyootUser = (bool) reader["IsTeeyootUser"]
                                };

                                if (reader["CurrencyName"] != DBNull.Value)
                                    userItemViewModel.Currency = (string) reader["CurrencyName"];

                                userItems.Add(userItemViewModel);
                            }
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = userTotalQuery;

                        userTotal = (int) command.ExecuteScalar();
                    }

                    transaction.Commit();
                }
            }

            var pagerShape = Shape.Pager(pager).TotalItemCount(userTotal);

            var viewModel = new AdminUserIndexViewModel
            {
                Users = userItems,
                Pager = pagerShape
            };

            return View(viewModel);
        }

        public ActionResult EditUser(int userId)
        {
            var viewModel = new EditUserViewModel();

            var user = Services.ContentManager.Get<UserPart>(userId);
            var teeyootUser = user.As<TeeyootUserPart>();

            if (teeyootUser != null)
            {
                viewModel.IsTeeyootUser = true;
                viewModel.Currency = teeyootUser.CurrencyId;

                var campaignsQuery = _campaignService.GetCampaignsOfUser(teeyootUser.Id);

                if (!campaignsQuery.Any(c => c.IsActive) && GetUserPayoutBalance(campaignsQuery) <= 0)
                {
                    viewModel.IsUserCurrencyEditable = true;
                }
            }

            var currencies = _currencyRepository.Table
                .ToList()
                .Select(c => new CurrencyItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                });

            viewModel.UserId = user.Id;
            viewModel.Email = user.UserName;
            viewModel.Currencies = currencies;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditUser(EditUserViewModel model)
        {
            var user = Services.ContentManager.Get<UserPart>(model.UserId);

            if (!_userService.VerifyUserUnicity(model.UserId, model.Email, model.Email))
            {
                Services.Notifier.Error(T("User with that email already exists."));
            }
            else if (!Regex.IsMatch(model.Email ?? "", UserPart.EmailPattern, RegexOptions.IgnoreCase))
            {
                // http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx    
                Services.Notifier.Error(T("You must specify a valid email address."));
            }
            else
            {
                if (!(string.IsNullOrEmpty(model.Password) && string.IsNullOrEmpty(model.ConfirmPassword)))
                {
                    if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
                    {
                        Services.Notifier.Error(T("Password or Confirm Password field is empty."));
                    }
                    else
                    {
                        if (model.Password != model.ConfirmPassword)
                        {
                            Services.Notifier.Error(T("Password confirmation must match."));
                        }

                        var actUser = _membershipService.GetUser(user.UserName);
                        _membershipService.SetPassword(actUser, model.Password);
                    }
                }

                user.UserName = model.Email;
                user.Email = model.Email;
                user.NormalizedUserName = model.Email.ToLowerInvariant();
            }

            if (!ModelState.IsValid || Services.Notifier.List().Any())
            {
                Services.TransactionManager.Cancel();
                return RedirectToAction("EditUser", new {userId = user.Id});
            }

            var teeyootUser = user.As<TeeyootUserPart>();
            if (teeyootUser != null)
            {
                teeyootUser.CurrencyId = model.Currency;
            }

            Services.ContentManager.Publish(user.ContentItem);

            Services.Notifier.Information(T("User information updated"));
            return RedirectToAction("Index");
        }

        private double GetUserPayoutBalance(IQueryable<CampaignRecord> campaignsQuery)
        {
            var campaignIds = new List<int>();
            foreach (var campaign in campaignsQuery)
            {
                var campaignProducts = _orderService.GetProductsOrderedOfCampaign(campaign.Id)
                    .ToList();

                if (campaign.ProductMinimumGoal <= campaignProducts.Select(p => p.Count).Sum())
                {
                    campaignIds.Add(campaign.Id);
                }
            }

            var orderedProducts = _orderService.GetProductsOrderedOfCampaigns(campaignIds.ToArray());
            var payoutBalance = orderedProducts.Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled" &&
                                                           p.OrderRecord.OrderStatusRecord.Name != "Unapproved" &&
                                                           p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-MY")
                .Select(p => new
                {
                    Profit = p.Count*(p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost)
                })
                .Sum(entry => (double?) entry.Profit);

            return payoutBalance ?? 0;
        }
    }
}
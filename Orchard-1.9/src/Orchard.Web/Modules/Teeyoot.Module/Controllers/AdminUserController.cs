using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.SqlServer.Server;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Roles.Models;
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
        private readonly IRepository<RoleRecord> _roleRepository;

        public IOrchardServices Services { get; set; }
        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public AdminUserController(
            IRepository<RoleRecord> roleRepository,
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
            _roleRepository = roleRepository;
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

        public ActionResult Index(int? role, UserIndexOptions options, PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var userItems = new List<UserItemViewModel>();
            int userTotal;

            var skip = pager.Page > 0 ? (pager.Page - 1)*pager.PageSize : 0;
            var take = pager.PageSize == 0 ? int.MaxValue : pager.PageSize;

            using (var connection = new SqlConnection(_shellSettings.DataConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetUsers";

                        if (role.HasValue)
                        {
                            var roleIdParameter = new SqlParameter("@RoleId", SqlDbType.Int)
                            {
                                Value = role.Value
                            };
                            command.Parameters.Add(roleIdParameter);
                        }

                        var skipParameter = new SqlParameter("@Skip", SqlDbType.Int)
                        {
                            Value = skip
                        };
                        var takeParameter = new SqlParameter("@Take", SqlDbType.Int)
                        {
                            Value = take
                        };

                        command.Parameters.Add(skipParameter);
                        command.Parameters.Add(takeParameter);

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
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetUsersCount";

                        if (role.HasValue)
                        {
                            var roleIdParameter = new SqlParameter("@RoleId", SqlDbType.Int)
                            {
                                Value = role.Value
                            };
                            command.Parameters.Add(roleIdParameter);
                        }

                        userTotal = (int) command.ExecuteScalar();
                    }

                    FillUsersWithRoles(userItems, transaction);

                    transaction.Commit();
                }
            }

            var pagerShape = Shape.Pager(pager).TotalItemCount(userTotal);

            var roles = _roleRepository.Table
                .Select(r => new RoleItemViewModel
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();

            var viewModel = new AdminUserIndexViewModel
            {
                Roles = roles,
                SelectedRoleId = role,
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

        private double GetUserPayoutBalance(IEnumerable<CampaignRecord> campaigns)
        {
            var campaignIds = new List<int>();
            foreach (var campaign in campaigns)
            {
                var campaignProducts = _orderService.GetProductsOrderedOfCampaign(campaign.Id)
                    .ToList();

                if (campaign.ProductMinimumGoal <= campaignProducts.Select(p => p.Count).Sum())
                {
                    campaignIds.Add(campaign.Id);
                }
            }

            var orderedProducts = _orderService.GetProductsOrderedOfCampaigns(campaignIds.ToArray());
            var myProfit = orderedProducts
                .Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled" &&
                            p.OrderRecord.OrderStatusRecord.Name != "Unapproved")
                .Select(p => new
                {
                    Profit = p.Count*(p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost)
                })
                .Sum(entry => (double?) entry.Profit);

            if (myProfit < 0)
            {
                myProfit *= -1;
            }

            return myProfit ?? 0;
        }

        private static void FillUsersWithRoles(IList<UserItemViewModel> users, IDbTransaction transaction)
        {
            if (!users.Any())
                return;

            using (var command = transaction.Connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GetUsersRoles";

                var userIdsValue = new List<SqlDataRecord>();
                foreach (var user in users)
                {
                    var campaignIdValue = new SqlDataRecord(new SqlMetaData("N", SqlDbType.BigInt));
                    campaignIdValue.SetInt64(0, Convert.ToInt64(user.UserId));

                    userIdsValue.Add(campaignIdValue);
                }

                var userIdsParameter = new SqlParameter("@UserIds", SqlDbType.Structured)
                {
                    TypeName = "INTEGER_LIST_TABLE_TYPE",
                    Value = userIdsValue
                };
                command.Parameters.Add(userIdsParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var userId = (int) reader["UserId"];
                        var user = users.First(u => u.UserId == userId);

                        var role = (string) reader["RoleName"];
                        if (string.IsNullOrEmpty(user.Roles))
                        {
                            user.Roles = role;
                        }
                        else
                        {
                            user.Roles += ", " + role;
                        }
                    }
                }
            }
        }
    }
}
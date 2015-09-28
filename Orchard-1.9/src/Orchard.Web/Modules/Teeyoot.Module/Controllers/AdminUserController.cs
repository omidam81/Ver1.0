using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Configuration;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.Users.ViewModels;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminUserController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly ShellSettings _shellSettings;

        private dynamic Shape { get; set; }
        public IOrchardServices Services { get; set; }

        public AdminUserController(
            ShellSettings shellSettings,
            IOrchardServices services,
            ISiteService siteService,
            IShapeFactory shapeFactory)
        {
            _shellSettings = shellSettings;
            Services = services;
            _siteService = siteService;

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

            const string userQuery = " SELECT UserPartRecord.UserName" +
                                     " FROM Orchard_Framework_ContentItemVersionRecord ContentItemVersionRecord" +
                                     " JOIN Orchard_Framework_ContentItemRecord ContentItemRecord" +
                                     " ON ContentItemVersionRecord.ContentItemRecord_id = ContentItemRecord.Id" +
                                     " JOIN Orchard_Users_UserPartRecord UserPartRecord" +
                                     " ON ContentItemRecord.Id = UserPartRecord.Id" +
                                     " LEFT JOIN Teeyoot_Module_TeeyootUserPartRecord TeeyootUserPartRecord" +
                                     " ON UserPartRecord.Id = TeeyootUserPartRecord.Id" +
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
                                    Email = (string) reader["UserName"]
                                };

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
    }
}

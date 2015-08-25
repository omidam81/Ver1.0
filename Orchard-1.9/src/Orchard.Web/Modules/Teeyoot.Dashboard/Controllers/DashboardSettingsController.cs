using Orchard.ContentManagement;
using Orchard.Users.Models;
using Orchard.Users.ViewModels;
using System.Web.Mvc;
using Orchard.Mvc.Extensions;
using Teeyoot.Module.Models;


namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {

        public ActionResult Profile(UserSettingsViewModel viewModel)
        {
            if (viewModel.ErrorMessage == null)
            {
                var currentUser = Services.WorkContext.CurrentUser;
                var teeUser = _contentManager.Get<TeeyootUserPart>(currentUser.Id, VersionOptions.Latest);
                UserSettingsViewModel model = new UserSettingsViewModel() { };
                model.Id = teeUser.Id;
                model.PublicName = teeUser.PublicName;
                model.PhoneNumber = teeUser.PhoneNumber;
                model.City = teeUser.City;
                model.CurrentEmail = currentUser.Email;
                model.State = teeUser.State;
                model.Street1 = teeUser.Street;
                model.InfoMessage = viewModel.InfoMessage;
                model.Zip = teeUser.Zip;
                model.Country = teeUser.Country;
                return View(model);
            }
            else
            {
                var currentUser = Services.WorkContext.CurrentUser;
                var teeUser = _contentManager.Get<TeeyootUserPart>(currentUser.Id, VersionOptions.Latest);
                UserSettingsViewModel model = new UserSettingsViewModel() { };
                model.Id = teeUser.Id;
                model.PublicName = teeUser.PublicName;
                model.PhoneNumber = teeUser.PhoneNumber;
                model.City = teeUser.City;
                model.CurrentEmail = currentUser.Email;
                model.State = teeUser.State;
                model.Street1 = teeUser.Street;
                model.ErrorMessage = viewModel.ErrorMessage;
                model.Zip = teeUser.Zip;
                model.Country = teeUser.Country;
                return View(model);
            }
            
        }

        public ActionResult ChangeUserInfo(UserSettingsViewModel model)
        {
            if (model.PhoneNumber == null && model.City == null && model.PublicName == null && model.State == null && model.Street1 == null && model.Street2 == null && model.Zip == null)
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += T("There is nothing to update!").ToString();
                return RedirectToAction("Profile", viewModel);
            }
            if (TryValidateModel(model))
            {               
                var user = _contentManager.Get<TeeyootUserPart>(model.Id, VersionOptions.Latest);
                if (user != null)
                {
                    user.PhoneNumber = model.PhoneNumber;
                    user.City = model.City;
                    user.PublicName = model.PublicName;
                    user.State = model.State;
                    user.Street = model.Street1 + " " + model.Street2;
                    user.Zip = model.Zip;
                    user.Country = model.Country;
                    
                    model.InfoMessage = T("Your info has been changed sucessfully!").ToString();
                    return RedirectToAction("Profile", model);
                }
                else
                {
                    UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                    viewModel.Id = model.Id;
                    viewModel.ErrorMessage += T("Sorry, it is some error, try later!").ToString();
                    return RedirectToAction("Profile", viewModel);
                }
            }
            else
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += T("Please, input valid phone number!").ToString();
                return RedirectToAction("Profile", viewModel);
            }

        }

        public ActionResult ChangePassword(UserSettingsViewModel model)
        {

            string currentUser = Services.WorkContext.CurrentUser.Email;
            if ((model.CurrentPassword == null)||(model.NewPassword == null))
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += T("Password is required!").ToString();
                return RedirectToAction("Profile", viewModel);
            }
            if (!(model.NewPassword.Length > 7))
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += T("Password must be at least 8 characters!").ToString();
                return RedirectToAction("Profile", viewModel);
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += T("Confirmation did not match new password!").ToString();
                return RedirectToAction("Profile", viewModel);
            }
            var user = _membershipService.ValidateUser(currentUser, model.CurrentPassword);
            if (user != null)
            {
                _membershipService.SetPassword(user, model.NewPassword);
            }
            else
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += T("Password incorrect!").ToString();
                return RedirectToAction("Profile", viewModel);
            }
            UserSettingsViewModel infoModel = new UserSettingsViewModel() { };
            infoModel.InfoMessage = T("Your password has been changed sucessfully!").ToString();
            return RedirectToAction("Profile", infoModel);
        }

        public ActionResult ChangeEmail(UserSettingsViewModel model)
        {
            if (TryValidateModel(model))
            {
                if (model.NewEmailAddress == null)
                {
                    UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                    viewModel.Id = model.Id;
                    viewModel.ErrorMessage += T("New email address is required!").ToString();
                    return RedirectToAction("Profile", viewModel);
                }
                if (model.NewEmailAddress != model.ConfirmNewEmailAddress)
                {
                    UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                    viewModel.Id = model.Id;
                    viewModel.ErrorMessage += T("Confirmation did not match new email!").ToString();
                    return RedirectToAction("Profile", viewModel);

                }
                var user = Services.WorkContext.CurrentUser;
                try
                {
                    user.As<UserPart>().Email = model.NewEmailAddress;
                    user.As<UserPart>().UserName = model.NewEmailAddress;
                    user.As<UserPart>().NormalizedUserName = model.NewEmailAddress.ToLower();
                    model.InfoMessage = T("Your email has been changed sucessfully!").ToString();
                }
                catch
                {
                    UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                    viewModel.Id = model.Id;
                    viewModel.ErrorMessage += T("Sorry, it is some error, try later!").ToString();
                    return RedirectToAction("Profile", viewModel);
                }
                UserSettingsViewModel infoModel = new UserSettingsViewModel() { };
                infoModel.InfoMessage = T("Your email has been changed sucessfully!").ToString();
                return RedirectToAction("Profile", infoModel);
            }
            else
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += T("Please, input valid email address!").ToString();
                return RedirectToAction("Profile", viewModel);
            }
            
        }
    }
}
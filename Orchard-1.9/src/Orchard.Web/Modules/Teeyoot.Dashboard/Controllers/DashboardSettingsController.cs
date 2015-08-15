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
      
        public ActionResult Settings(UserSettingsViewModel viewModel)
        {
            if (viewModel.ErrorMessage == null)
            {
                int currentUser = Services.WorkContext.CurrentUser.Id;
                var teeUser = _contentManager.Get<TeeyootUserPart>(currentUser, VersionOptions.Latest);
                UserSettingsViewModel model = new UserSettingsViewModel() { };
                model.Id = teeUser.Id;
                model.PublicName = teeUser.PublicName;
                model.PhoneNumber = teeUser.PhoneNumber;
                model.City = teeUser.City;
                model.State = teeUser.State;
                model.Street = teeUser.Street;
                model.InfoMessage = viewModel.InfoMessage;
                model.Zip = teeUser.Zip;
                return View(model);
            }
            else
            {
                int currentUser = Services.WorkContext.CurrentUser.Id;
                var teeUser = _contentManager.Get<TeeyootUserPart>(currentUser, VersionOptions.Latest);
                UserSettingsViewModel model = new UserSettingsViewModel() { };
                model.Id = teeUser.Id;
                model.PublicName = teeUser.PublicName;
                model.PhoneNumber = teeUser.PhoneNumber;
                model.City = teeUser.City;
                model.State = teeUser.State;
                model.Street = teeUser.Street;
                model.ErrorMessage = viewModel.ErrorMessage;
                model.Zip = teeUser.Zip;
                return View(model);
            }
            
        }

        public ActionResult ChangeUserInfo(UserSettingsViewModel model)
        {
            if (model.PhoneNumber == null && model.City == null && model.PublicName == null && model.State == null && model.Street == null && model.Zip == null)
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += "There is nothing to update!";
                return RedirectToAction("Settings", viewModel);
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
                    user.Street = model.Street;
                    user.Zip = model.Zip;
                    
                    model.InfoMessage = "Your info has been changed sucessfully!";
                    return RedirectToAction("Settings", model);
                }
                else
                {
                    UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                    viewModel.Id = model.Id;
                    viewModel.ErrorMessage += "Sorry, it is some error, try later!";
                    return RedirectToAction("Settings", viewModel);
                }
            }
            else
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += "Please, input valid phone number!";
                return RedirectToAction("Settings", viewModel);
            }

        }

        public ActionResult ChangePassword(UserSettingsViewModel model)
        {

            string currentUser = Services.WorkContext.CurrentUser.Email;
            if ((model.CurrentPassword == null)||(model.NewPassword == null))
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += "Password is required!";
                return RedirectToAction("Settings", viewModel);
            }
            if (!(model.NewPassword.Length > 6))
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += "Password must be at least 7 characters!";
                return RedirectToAction("Settings", viewModel);
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += "Confirmation did not match new password!";
                return RedirectToAction("Settings", viewModel);
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
                viewModel.ErrorMessage += "Password incorrect!";
                return RedirectToAction("Settings", viewModel);
            }
            UserSettingsViewModel infoModel = new UserSettingsViewModel() { };
            infoModel.InfoMessage = "Your password has been changed sucessfully!";
            return RedirectToAction("Settings", infoModel);
        }

        public ActionResult ChangeEmail(UserSettingsViewModel model)
        {
            if (TryValidateModel(model))
            {
                if (model.NewEmailAddress == null)
                {
                    UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                    viewModel.Id = model.Id;
                    viewModel.ErrorMessage += "New email address is required!";
                    return RedirectToAction("Settings", viewModel);
                }
                if (model.NewEmailAddress != model.ConfirmNewEmailAddress)
                {
                    UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                    viewModel.Id = model.Id;
                    viewModel.ErrorMessage += "Confirmation did not match new email!";
                    return RedirectToAction("Settings", viewModel);

                }
                var user = Services.WorkContext.CurrentUser;
                try
                {
                    user.As<UserPart>().Email = model.NewEmailAddress;
                    user.As<UserPart>().UserName = model.NewEmailAddress;
                    user.As<UserPart>().NormalizedUserName = model.NewEmailAddress.ToLower();
                    model.InfoMessage = "Your email has been changed sucessfully!";
                }
                catch
                {
                    UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                    viewModel.Id = model.Id;
                    viewModel.ErrorMessage += "Sorry, it is some error, try later!";
                    return RedirectToAction("Settings", viewModel);
                }
                UserSettingsViewModel infoModel = new UserSettingsViewModel() { };
                infoModel.InfoMessage = "Your email has been changed sucessfully!";
                return RedirectToAction("Settings", infoModel);
            }
            else
            {
                UserSettingsViewModel viewModel = new UserSettingsViewModel() { };
                viewModel.Id = model.Id;
                viewModel.ErrorMessage += "Please, input valid email address!";
                return RedirectToAction("Settings", viewModel);
            }
            
        }
    }
}
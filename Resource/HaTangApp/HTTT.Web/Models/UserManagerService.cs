using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HTTT.HaTangApp.Models.ViewModel;
using HTTT.HaTangApp.Models.Validation;
using System.Web.Security;
using System.Web.Profile;

namespace HTTT.HaTangApp.Models
{
    public partial class HaTangAppServices
    {


        #region User management Services
        private bool ValidateUser(UserViewModel userToValidate)
        {
            if (String.IsNullOrEmpty(userToValidate.UserName))
            {
                _validationDictionary.AddError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(userToValidate.Email))
            {
                _validationDictionary.AddError("email", "You must specify an email address.");
            }
            if (userToValidate.Password == null)
            {
                _validationDictionary.AddError("password", "Phải nhập password");
            }
            if (!String.Equals(userToValidate.Password, userToValidate.ConfirmPassword, StringComparison.Ordinal))
            {
                _validationDictionary.AddError("_FORM", "The new password and confirmation password do not match.");
            }
            return _validationDictionary.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        public bool CreateUser(UserViewModel userToCreate)
        {
            // Validation logic
            if (!ValidateUser(userToCreate))
                return false;

            // Database logic
            try
            {
                MembershipCreateStatus createStatus;
                MembershipUser createdUser = Membership.CreateUser(
                        userToCreate.UserName
                        , userToCreate.Password
                        , userToCreate.Email
                        , null
                        , null
                        , true
                        , out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    // Create profile
                    ProfileBase curProfile = ProfileBase.Create(createdUser.UserName);
                    curProfile.SetPropertyValue("FirstName", userToCreate.FistName);
                    curProfile.SetPropertyValue("LastName", userToCreate.LastName);
                    curProfile.SetPropertyValue("MobilePhone", userToCreate.Mobile);
                    curProfile.SetPropertyValue("HomePhone", userToCreate.HomePhone);
                    curProfile.Save();
                }
                else
                {
                    _validationDictionary.AddError("_FORM", ErrorCodeToString(createStatus));
                    return false;
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool EditUser(UserViewModel userEdit)
        {
            //// Validation logic
            //if (!ValidateUser(userEdit))
            //    return false;

            //// Database logic
            //try
            //{
            //    MembershipUser editUser = new MembershipUser(
            //    Membership.UpdateUser(
            //}
            //catch
            //{
            //    return false;
            //}
            return true;
        }

        //public bool DeleteUser(UserViewModel userToDelete)
        //{
        //    try
        //    {
        //        _repository.DeleteBienBanGiaoCa(bienBanGiaoCaToDelete);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public aspnet_Users GetUser(Guid userId)
        {
            return _repository.GetUser(userId);
        }
        public IEnumerable<aspnet_Users> ListUsers()
        {
            return _repository.ListUsers();
        }
        #endregion
    }
}
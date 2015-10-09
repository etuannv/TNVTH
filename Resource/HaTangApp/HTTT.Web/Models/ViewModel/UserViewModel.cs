using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Profile;
using System.Web.Security;

namespace HTTT.HaTangApp.Models.ViewModel
{
    public class UserViewModel
    {

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "First name")]
        public string FistName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} phải ít nhất {2} kí tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Mật khẩu không trùng nhau.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Home phone")]
        public string HomePhone { get; set; }

        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        public UserViewModel() { }
        public UserViewModel(aspnet_Users user)
        {
            MembershipUser mu = Membership.GetUser(user.UserName);
            //UserName = user.UserName;
            //FistName = user.FirstName;
            //LastName = user.LastName;
            //HomePhone = user.HomePhone;
            //Mobile = user.Mobile;
            //Email = mu.Email;
            //Password = mu.GetPassword();
            //ConfirmPassword = Password;
            

        }

    }
}
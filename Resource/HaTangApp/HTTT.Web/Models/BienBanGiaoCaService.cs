using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTTT.HaTangApp.Models
{
    public partial class HaTangAppServices
    {
        //Bien ban ban giao Services
        #region Bien ban ban giao Services

        private bool ValidateBienBanGiaoCa(BienBanGiaoCa BienBanGiaoCaToValidate)
        {
            //if (BienBanGiaoCaToValidate.FirstName.Trim().Length == 0)
            //    _validationDictionary.AddError("FirstName", "First name is required.");
            //if (BienBanGiaoCaToValidate.LastName.Trim().Length == 0)
            //    _validationDictionary.AddError("LastName", "Last name is required.");
            ////if (bienBanGiaoCaToValidate.Phone.Length > 0 && !Regex.IsMatch(bienBanGiaoCaToValidate.Phone, @"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}"))
            ////    _validationDictionary.AddError("Phone", "Invalid phone number.");
            //if (BienBanGiaoCaToValidate.Email.Length > 0 && !Regex.IsMatch(bienBanGiaoCaToValidate.Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            //    _validationDictionary.AddError("Email", "Invalid email address.");
            return _validationDictionary.IsValid;
        }

        public IEnumerable<BienBanGiaoCa> ListBienBanGiaoCa()
        {
            return _repository.ListBienBanGiaoCas();
        }

        public bool CreateBienBanGiaoCa(BienBanGiaoCa bienBanGiaoCaToCreate)
        {
            // Validation logic
            if (!ValidateBienBanGiaoCa(bienBanGiaoCaToCreate))
                return false;

            // Database logic
            try
            {
                _repository.CreateBienBanGiaoCa(bienBanGiaoCaToCreate);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool EditBienBanGiaoCa(BienBanGiaoCa bienBanGiaoCaToEdit)
        {
            // Validation logic
            if (!ValidateBienBanGiaoCa(bienBanGiaoCaToEdit))
                return false;

            // Database logic
            try
            {
                _repository.EditBienBanGiaoCa(bienBanGiaoCaToEdit);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool DeleteBienBanGiaoCa(BienBanGiaoCa bienBanGiaoCaToDelete)
        {
            try
            {
                _repository.DeleteBienBanGiaoCa(bienBanGiaoCaToDelete);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public BienBanGiaoCa GetBienBanGiaoCa(int id)
        {
            return _repository.GetBienBanGiaoCa(id);
        }
        #endregion
    }
}
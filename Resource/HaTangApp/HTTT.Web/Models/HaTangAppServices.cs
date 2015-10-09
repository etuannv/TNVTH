using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using HTTT.HaTangApp.Models.Validation;



namespace HTTT.HaTangApp.Models
{
    public partial class HaTangAppServices : HTTT.HaTangApp.Models.IHaTangAppServices
    {
        private IValidationDictionary _validationDictionary;
        private IHaTangAppRepositiory _repository;
        
        // Constructor
        public HaTangAppServices(IValidationDictionary validationDictionary)
            : this(validationDictionary, new HaTangAppRepositiory ())
        { }
        public HaTangAppServices(IValidationDictionary validationDictionary, IHaTangAppRepositiory repository)
        {
            _validationDictionary = validationDictionary;
            _repository = repository;
        }

        
    }
}
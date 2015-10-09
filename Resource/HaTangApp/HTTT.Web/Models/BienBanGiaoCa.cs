using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HTTT.HaTangApp.Models
{
    [MetadataType(typeof(BienBanGiaoCaMetadata))]
    public partial class BienBanGiaoCa
    {
        class BienBanGiaoCaMetadata
        {
            [Required(ErrorMessage = "StartDate are required")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public string StartDate { get; set; }

            [Required(ErrorMessage = "StartTime are required")]
            public string StartTime { get; set; }

            [Required(ErrorMessage = "EndDate are required")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public string EndDate { get; set; }

            [Required(ErrorMessage = "EndTime are required")]
            public string EndTime { get; set; }

            //[Required(ErrorMessage = "The Phone is required.")]
            //public string Phone { get; set; }

            //[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "InvalidEmailAddress")]
            //[Required(ErrorMessage = "The Email is required.")]
            //public string Email { get; set; }
        }
    }
}
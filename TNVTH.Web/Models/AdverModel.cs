using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TNVTH.Web.Models
{
    [MetadataType(typeof(T_AdverMetadata))]
    public partial class T_Adver
    {
        internal sealed class T_AdverMetadata
        {
            [Required(ErrorMessage = "Phải nhập Tiêu đề")]
            public string Title { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> PublishDate { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> UnpublishDate { get; set; }
        }
    }

    
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TNVTH.Web.Models
{
    [MetadataType(typeof(T_SlideMetadata))]
    public partial class T_Slide
    {
        internal sealed class T_SlideMetadata
        {
            [Required(ErrorMessage = "Phải nhập Tiêu đề")]
            public string Title { get; set; }
        }
    }

    
}

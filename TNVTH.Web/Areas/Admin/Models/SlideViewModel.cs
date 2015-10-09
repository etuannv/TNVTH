using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TNVTH.Web.Models;

namespace TNVTH.Web.Areas.Admin.Models
{
    [MetadataType(typeof(SlideViewModelMetadata))]
    public class SlideViewModel : T_Slide
    {
        public IEnumerable<T_SlideGroup> SlideGroupList { get; set; }

        public SlideViewModel()
        {

        }
        public SlideViewModel(T_Slide iSlide, IEnumerable<T_SlideGroup> iSlideGroupList)
        {
            ID = iSlide.ID;
            Title = iSlide.Title;
            Link = iSlide.Link;
            ImagePath = iSlide.ImagePath;
            Enable = iSlide.Enable;
            GroupID = iSlide.GroupID;
            SlideGroupList = iSlideGroupList;
        }

        public T_Slide GetSlide()
        {
            T_Slide RetSlide = new T_Slide();
            RetSlide.ID = ID;
            RetSlide.Title = Title;
            RetSlide.Link = Link;
            RetSlide.ImagePath = ImagePath;
            RetSlide.Enable = Enable;
            RetSlide.GroupID = GroupID;
            return RetSlide;
        }


        class SlideViewModelMetadata
        {
            [Required(ErrorMessage = "Phải nhập Tên")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Phải chọn nhóm Slide")]
            public int GroupID { get; set; }
        }
    }
}
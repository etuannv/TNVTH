using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using TNVTH.Web.Models;


namespace TNVTH.Web.Areas.Admin.Models
{
    
    [MetadataType(typeof(NewsViewModelMetadata))]
    public class NewsViewModel : T_News
    {
        public NewsViewModel() { }
        public NewsViewModel(T_News iNews) 
        {
            Title = iNews.Title;
            Slug = iNews.Slug;
            AvataImageUrl = iNews.AvataImageUrl;
            IsHotNews = iNews.IsHotNews;
            ContentNews = iNews.ContentNews;
            Author = iNews.Author;
            PublishTime = iNews.PublishTime;
            Status = iNews.Status;
            ModifiedBy = iNews.ModifiedBy;
            ModifiedDate = iNews.ModifiedDate;
        }
        public List<T_Tag> CategoryList { get; set; }

        public List<T_Tag> MyTagList { get; set; }
        public List<T_Tag> MyCategoryList { get; set; }
        class NewsViewModelMetadata
        {
            [Required(ErrorMessage = "Phải nhập Tên")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Phải nhập Slug")]
            public string Slug { get; set; }

            //[Required(ErrorMessage = "EndDate are required")]
            //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            //public string EndDate { get; set; }

            //[Required(ErrorMessage = "EndTime are required")]
            //public string EndTime { get; set; }

            //[Required(ErrorMessage = "The Phone is required.")]
            //public string Phone { get; set; }

            //[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "InvalidEmailAddress")]
            //[Required(ErrorMessage = "The Email is required.")]
            //public string Email { get; set; }
        }
    }
}
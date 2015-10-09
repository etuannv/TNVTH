using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TNVTH.Web.Models;


namespace TNVTH.Web.Areas.Admin.Models
{
    [MetadataType(typeof(TagViewModelMetadata))]
    public class TagViewModel: T_Tag
    {
        public IEnumerable<T_Tag> TagList { get; set; }
        
        public TagViewModel(){ 

        }
        public TagViewModel(T_Tag iTag, IEnumerable<T_Tag> iTagList) {
            ID = iTag.ID;
            Title = iTag.Title;
            Slug = iTag.Slug;
            Taxonomy = iTag.Taxonomy;
            Description = iTag.Description;
            ParentID = iTag.ParentID;
            ParentPath = iTag.ParentPath;
            CreatedDate = iTag.CreatedDate;
            CreatedBy = iTag.CreatedBy;
            ModifiedDate = iTag.ModifiedDate;
            ModifiedBy = iTag.ModifiedBy;
            TagList = iTagList;
        }

        public T_Tag GetTag()
        {
            T_Tag RetTag = new T_Tag();
            RetTag.ID = ID;
            RetTag.Title = Title;
            RetTag.Slug = Slug;
            RetTag.Taxonomy = Taxonomy;
            RetTag.Description = Description;
            RetTag.ParentID = ParentID;
            RetTag.ParentPath = ParentPath;
            RetTag.CreatedDate = CreatedDate;
            RetTag.CreatedBy = CreatedBy;
            RetTag.ModifiedDate = ModifiedDate;
            RetTag.ModifiedBy = ModifiedBy;
            return RetTag;
        }


        class TagViewModelMetadata
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
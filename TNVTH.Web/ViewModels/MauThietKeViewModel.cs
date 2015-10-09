using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNVTH.Web.Models;

namespace TNVTH.Web.ViewModels
{
    public class MauThietKeViewModel
    {
        public T_News MauThietKe { get; set; }
        public T_Tag Cate { get; set; }

        public MauThietKeViewModel()
        {
        }
        public MauThietKeViewModel(T_News iMauThietKe, T_Tag iCate)
        {
            MauThietKe = iMauThietKe;
            Cate = iCate;
        }
    }
}
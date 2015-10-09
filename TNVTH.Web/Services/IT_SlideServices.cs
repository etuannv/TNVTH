using System.Collections.Generic;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;

namespace TNVTH.Web.Services
{
    public interface IT_SlideServices
    {
        IEnumerable<T_Slide> GetAll();
        T_Slide GetByID(int id);
        ReturnValue<bool> AddNewSlide(T_Slide iSlide);
        T_Slide AddNewSlideAndReturn(T_Slide iSlide);
        ReturnValue<bool> UpdateSlide(T_Slide iSlide);
        ReturnValue<bool> DeleteSlide(T_Slide iSlide);
        ReturnValue<bool> DeleteSlide(int id);
        IEnumerable<T_Slide> SlideSearch(string term, int? slideGroupID);
        IEnumerable<T_Slide> GetEnableSlide();
        IEnumerable<T_Slide> GetSlideByGroupID(int GroupID);
        void DeleteSlideBySlideGroup(int groupID);
        void AdvertismentClick(int adId);
    }
}

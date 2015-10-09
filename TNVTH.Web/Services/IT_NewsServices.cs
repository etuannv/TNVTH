using System.Collections.Generic;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;


namespace TNVTH.Web.Services
{
    public interface IT_NewsServices
    {
        IEnumerable<T_News> GetAll();

        IEnumerable<T_News> GetNews(int? cateId, string search);
        IEnumerable<T_News> GetByTaxonomy(int iCateID);
        IEnumerable<T_News> GetByTaxonomy(int iCateID, int number);
        IEnumerable<T_News> GetRandomByTaxonomy(int iCateID, int number);
        T_News GetByID(int id);
        T_News GetBySlug(string slug);
        ReturnValue<bool> AddNewNews(T_News iNews);
        T_News AddNewNewsAndReturn(T_News iNews);
        ReturnValue<bool> UpdateNews(T_News iNews);
        ReturnValue<bool> DeleteNews(T_News iNews);
        ReturnValue<bool> DeleteNews(int id);

        IEnumerable<T_News> GetLastNews(int limit);

        IEnumerable<T_News> GetRelatedNews(int newsId, int limit);

        IEnumerable<T_News> GetNewsByTag(int tagId, int limit);

        T_Tag GetCateByNewsID(int newsID);

        //IEnumerable<T_News> GetByTaxonomyList(List<int> CateIdList, int Number);

        IEnumerable<T_News> Search(string term);
    }
}

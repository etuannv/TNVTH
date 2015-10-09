using System.Collections.Generic;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;


namespace TNVTH.Web.Services
{
    public interface IT_TagServices
    {
        IEnumerable<T_Tag> GetAll();
        IEnumerable<T_Tag> GetByTaxonomy(string taxonomy);
        IEnumerable<T_Tag> GetByTaxonomyForDisplay(string taxonomy, IEnumerable<T_Tag> excepTagList = null, string searchKey = null);
        IEnumerable<T_Tag> GetByTaxonomy(string taxonomy, string searchKey);
        IEnumerable<T_Tag> GetByTaxonomy(string taxonomy, int limit);
        T_Tag GetByID(int id);
        T_Tag GetBySlug(string slug);
        ReturnValue<bool> AddNewTag(T_Tag iTag);
        
        T_Tag AddNewTagAndReturn(T_Tag iTag);

        T_Tag AddNewTagAndReturn(string iTag);
        ReturnValue<bool> UpdateTag(T_Tag iTag);
        ReturnValue<bool> DeleteTag(T_Tag iTag);
        ReturnValue<bool> DeleteTag(int id);
        string GetPath(int? parentID);

        IEnumerable<KeyValuePair<int, string>> TagSearch(string term);

        IEnumerable<T_Tag> GetTagByNewsID(string taxonomy, int newsId);
    }
}

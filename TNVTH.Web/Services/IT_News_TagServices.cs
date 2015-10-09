using System.Collections.Generic;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;


namespace TNVTH.Web.Services
{
    public interface IT_News_TagServices
    {
        IEnumerable<T_News_Tag> GetAll();
        T_News_Tag GetByID(int id);
        IEnumerable<T_Tag> GetTagByNewsID(int iNewID, string taxonomy);
        IEnumerable<T_News_Tag> GetByTagID(int iTagID, string taxonomy);
        ReturnValue<bool> AddNewNews_Tag(T_News_Tag iTag);
        ReturnValue<bool> AddNewNews_Tag(int iNewsID, int iTagID);
        
        //T_News_Tag AddNewTagAndReturn(T_News_Tag iTag);
        ReturnValue<bool> UpdateNews_Tag(T_News_Tag iTag);
        ReturnValue<bool> DeleteNews_Tag(T_News_Tag iTag);
        ReturnValue<bool> DeleteNews_Tag(int id);
        ReturnValue<bool> DeleteAllTagByNewsID(int iNewsID);
    }
}

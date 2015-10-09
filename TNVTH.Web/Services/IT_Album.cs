using System.Collections.Generic;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;

namespace TNVTH.Web.Services
{
    public interface IT_AlbumServices
    {
        IEnumerable<T_Album> GetAll();
        T_Album GetByID(int id);
        ReturnValue<bool> AddNewAlbum(T_Album iAlbum);
        T_Album AddNewAlbumAndReturn(T_Album iAlbum);
        ReturnValue<bool> UpdateAlbum(T_Album iAlbum);
        ReturnValue<bool> DeleteAlbum(T_Album iAlbum);
        ReturnValue<bool> DeleteAlbum(int id);
        IEnumerable<T_Album> AlbumSearch(string term);
    }
}

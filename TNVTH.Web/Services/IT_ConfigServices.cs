using System.Collections.Generic;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;

namespace TNVTH.Web.Services
{
    public interface IT_ConfigServices
    {
        IEnumerable<T_Config> GetAll();
        T_Config GetByID(int id);
        ReturnValue<bool> AddNewConfig(T_Config iConfig);
        T_Config AddNewConfigAndReturn(T_Config iConfig);
        ReturnValue<bool> UpdateConfig(T_Config iConfig);
        ReturnValue<bool> DeleteConfig(T_Config iConfig);
        ReturnValue<bool> DeleteConfig(int id);
        IEnumerable<T_Config> ConfigSearch(string term);
        T_Config GetByKey(string key);
    }
}

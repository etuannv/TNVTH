using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNVTH.Web.Models;
using TNVTH.Web.Services;

namespace TNVTH.Web
{
    public sealed class GlobalConfig
    {
        private static readonly Lazy<GlobalConfig> lazy =
            new Lazy<GlobalConfig>(() => new GlobalConfig());
        public static GlobalConfig Instance { get { return lazy.Value; } }
        IEnumerable<T_Config> ConfigList;
        private GlobalConfig()
        {
            ConfigList = new T_ConfigServices().GetAll();
        }
        public string GetValue(string key)
        {
            string ret = "";
            T_Config data = ConfigList.Where(m => m.Key == key).SingleOrDefault();
            if(data != null)
            {
                ret = data.Value;
            }
            return ret;
        }

        public void SetValue(string key, string value)
        {
            T_ConfigServices service = new T_ConfigServices();
            T_Config config = service.GetByKey(key);
            if (config != null)
            {
                config.Value = value;
                service.UpdateConfig(config);
            }
            
        }

        public string GetString4LinkArticle(string key)
        {
            string Value = GetValue(key);
            T_NewsServices service = new T_NewsServices();
            T_News MyNews = service.GetByID(Convert.ToInt32(Value));
            if(MyNews != null)  return MyNews.ID + "/" + MyNews.Slug;
            else return Value;
        }

        public string GetString4LinkCategory(string key)
        {
            string Value = GetValue(key);
            T_TagServices service = new T_TagServices();
            T_Tag MyTag = service.GetByID(Convert.ToInt32(Value));
            if (MyTag != null) return MyTag.ID + "/" + MyTag.Slug;
            else return Value;
        }
    }
}


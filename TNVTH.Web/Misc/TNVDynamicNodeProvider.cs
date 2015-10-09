using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapProvider;
using TNVTH.Web.Services;
using TNVTH.Web.Models;

namespace TNVTH.Web
{
    public class ArticleDynamicNodeProvider : DynamicNodeProviderBase
    {
        T_NewsServices _newServices;
        T_TagServices _tagServices;

        public ArticleDynamicNodeProvider()
        {
            _newServices = new T_NewsServices();
            _tagServices = new T_TagServices();
        }
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            // Build value 
            var returnValue = new List<DynamicNode>();

            // Create a node for each Arrticle
            foreach (var article in _newServices.GetAll())
            {
                T_News_TagServices newsTagServies = new T_News_TagServices();
                IEnumerable<T_Tag> TagList = newsTagServies.GetTagByNewsID(article.ID, TNVTH.Web.Utilities.Constants.TAXONOMY_TAG);
                string TagString = "";
                foreach (var tag in TagList)
                {
                    TagString += tag.Title;
                    TagString += ", ";
                }

                DynamicNode dynamicNode = new DynamicNode();
                dynamicNode.Title = article.Title;
                dynamicNode.RouteValues.Add("id", article.ID);
                dynamicNode.RouteValues.Add("slug", article.Slug);
                dynamicNode.PreservedRouteParameters = new List<string> { article.Slug };
                string Description = Utilities.Common.GetDescription(article.ContentNews, 50);
                dynamicNode.Description = (Description== null)? article.Title: Description;
                dynamicNode.Attributes.Add("keywords", TagString);
                dynamicNode.Attributes.Add("og:image", "http://" + HttpContext.Current.Request.Url.Host + article.AvataImageUrl);
                dynamicNode.Attributes.Add("type", "article");
                dynamicNode.Attributes.Add("author", article.Author);
                T_Tag NewsCate = _newServices.GetCateByNewsID(article.ID);
                if (NewsCate != null)
                {
                    dynamicNode.ParentKey = "NewsCate" + NewsCate.ID;
                }
                returnValue.Add(dynamicNode);
            }

            // Return 
            return returnValue;
        }

    }
    public class CategoryDynamicNodeProvider : DynamicNodeProviderBase
    {
        T_TagServices _tagServices;

        public CategoryDynamicNodeProvider()
        {
            _tagServices = new T_TagServices();
        }
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            // Build value 
            var returnValue = new List<DynamicNode>();

            // Create a node for each Category
            foreach (var tag in _tagServices.GetByTaxonomy(TNVTH.Web.Utilities.Constants.TAXONOMY_CATEGORY))
            {
                DynamicNode dynamicNode = new DynamicNode();
                dynamicNode.Key = "NewsCate" + tag.ID;
                dynamicNode.Title = tag.Title;
                dynamicNode.RouteValues.Add("id", tag.ID);
                dynamicNode.RouteValues.Add("slug", tag.Slug);
                dynamicNode.PreservedRouteParameters = new List<string> { tag.Slug };
                dynamicNode.Description = tag.Description;
                dynamicNode.Attributes.Add("keywords", tag.Slug);
                dynamicNode.Attributes.Add("type", "website");

                if (tag.ParentID != null)
                {
                    //Add parent key
                    T_Tag Cate = _tagServices.GetByID((int)tag.ParentID);
                    if (Cate != null)
                    {
                        dynamicNode.ParentKey = "NewsCate" + Cate.ID;
                    }
                }

                returnValue.Add(dynamicNode);
            }

            // Return 
            return returnValue;
        }
    }

    public class AlbumDynamicNodeProvider : DynamicNodeProviderBase
    {
        IT_AlbumServices _albumService;

        public AlbumDynamicNodeProvider()
        {
            _albumService = new T_AlbumServices();
        }
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            // Build value 
            var returnValue = new List<DynamicNode>();

            // Create a node for each Category
            foreach (var item in _albumService.GetAll())
            {
                DynamicNode dynamicNode = new DynamicNode();
                dynamicNode.Title = item.Title;
                dynamicNode.RouteValues.Add("id", item.ID);
                dynamicNode.RouteValues.Add("slug", item.Slug);
                dynamicNode.PreservedRouteParameters = new List<string> { item.Slug };
                dynamicNode.Description = item.Description;
                dynamicNode.ParentKey = "Album";
                returnValue.Add(dynamicNode);
            }

            // Return 
            return returnValue;
        }
    }        
}
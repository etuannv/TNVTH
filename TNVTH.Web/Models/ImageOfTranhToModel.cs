using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TNVTH.Web.Models
{
    public class ImageOfTranhToModel
    {
        public string FullPath { get; set; }
        public string ThumbPath { get; set; }
        public string FileName { get; set; }

        public string PictureSlug { get; set; }
        
        public ImageOfTranhToModel(string filename, string pictureSlug)
        {
            FileName = filename;
            PictureSlug = pictureSlug;
            var PictureFolder = Utilities.Constants.PICTURE_FOLDER_PATH + pictureSlug;
            FullPath = PictureFolder + "/" + filename;
            ThumbPath = PictureFolder + "/thumb/"+ filename;
        }
    }
}
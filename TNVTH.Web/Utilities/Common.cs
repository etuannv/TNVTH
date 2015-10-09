using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TNVTH.Web.Utilities
{
    public enum Config
    {
        SlideChinhGroupID,
        SlideDoiTacGroupID,
        AlbumPath,
        PageSizeAdmin,
        PageSizeClient,
        VisitCount,
        Ad_Home_Center1,
        Ad_Home_Center2,
        Ad_Home_Center3,
        Ad_Home_Center4,
        Ad_Home_Right1,
        Ad_Home_Right2,
        Ad_Home_Right3,
        Ad_Home_Right4,
        Ad_Home_Right5,
        Ad_Home_Right6,
        Ad_Video_Center1,
        Ad_Video_Center2,
        Ad_Video_Center3,
        Ad_Video_Center4,
        Ad_Video_Right1,
        Ad_Video_Right2,
        Ad_Video_Right3,
        Ad_Video_Right4,
        Ad_XemVideo_Right1,
        Ad_XemVideo_Right2,
        Ad_XemVideo_Right3,
        Ad_XemVideo_Right4,
        Ad_XemVideo_Center1,
        Ad_XemVideo_Center2,
        Ad_TranhTo_Right1,
        Ad_TranhTo_Right2,
        Ad_TranhTo_Right3,
        Ad_TranhTo_Right4,
        Ad_TranhTo_Center1,
        Ad_TranhTo_Center2,
        Ad_TranhTo_Center3,
        Ad_TranhTo_Center4,
        Ad_XemTranh_Right1,
        Ad_XemTranh_Right2,
        Ad_XemTranh_Right3,
        Ad_XemTranh_Right4,
        Ad_XemTranh_Center1,
        Ad_XemTranh_Center2,
        Ad_XemTranh_Center3,
        Ad_XemTranh_Center4,
        Ad_GocChaMe_Right1,
        Ad_GocChaMe_Right2,
        Ad_GocChaMe_Right3,
        Ad_GocChaMe_Right4,
        Ad_GocChaMe_Center1,
        Ad_GocChaMe_Center2,
        Ad_GocChaMe_Center3,
        Ad_GocChaMe_Center4,
        Ad_Game_Right1,
        Ad_Game_Right2,
        Ad_Game_Right3,
        Ad_Game_Right4,
        Ad_Game_Center1,
        Ad_Game_Center2,
        Ad_Game_Center3,
        Ad_Game_Center4,
        Conf_Blog_ID,
        Conf_GoiThietKe_ID,
        Conf_MauThietKe_ID,
        Conf_DichVu_ID,
    }

    public enum HotLinkType
    {
        Video,
        Picture,
        Game,
        ForParent,
        Home
    }
    public enum LinkTarGet
    {
        _parent,
        _blank,
        _self,
        _top,
    }


    public static class Common
    {
        //Return file name only
        public static string SaveFileFromUrl(string filePath, string filename, string url)
        {
            byte[] content;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();

            using (BinaryReader br = new BinaryReader(stream))
            {
                content = br.ReadBytes(500000);
                br.Close();
            }
            response.Close();
            string NewFileName = filename + url.Substring(url.LastIndexOf('.'));
            string fileFullPath = Path.Combine(filePath, NewFileName);

            FileStream fs = new FileStream(fileFullPath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(content);
                fs.Close();
                bw.Close();
                Common.CreateThumbnail(fileFullPath, 255, 195);
                return NewFileName;
            }
            finally
            {
                fs.Close();
                bw.Close();
            }

        }

        public static Image ScaleImage(Image image, int maxHeight)
        {
            var ratio = (double)maxHeight / image.Height;

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        public static void SetMaxImageHeight(string filePath, int height)
        {
            if (File.Exists(filePath))
            {
                // Load image.
                Image image = Image.FromFile(filePath);
                if (image.Height > height)
                {
                    Image Result = ScaleImage(image, height);
                    image.Dispose();
                    if (System.IO.File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    Result.Save(filePath);
                }
            }
        }
        public static bool CreateThumbnail(string fileInput, string fileOutput, int width, int height)
        {
            if (File.Exists(fileInput))
            {
                // Load image.
                Image image = Image.FromFile(fileInput);

                // Compute thumbnail size.
                //Size thumbnailSize = GetThumbnailSize(image);

                // Get thumbnail.
                Image thumbnail = image.GetThumbnailImage(width, height, null, IntPtr.Zero);
                // Save thumbnail.
                image.Dispose();
                // Save thumbnail.
                thumbnail.Save(fileOutput);
            }
            return true;
        }
        public static bool CreateThumbnail(string fileInput, int width, int height)
        {
            if (File.Exists(fileInput))
            {
                // Load image.
                Image image = Image.FromFile(fileInput);

                // Compute thumbnail size.
                //Size thumbnailSize = GetThumbnailSize(image);

                // Get thumbnail.
                Image thumbnail = image.GetThumbnailImage(width, height, null, IntPtr.Zero);

                // Save thumbnail.
                image.Dispose();
                if (System.IO.File.Exists(fileInput))
                {
                    File.Delete(fileInput);
                }

                thumbnail.Save(fileInput);
            }
            return true;
        }
        public static bool CreateThumbnail(string fileInput, string fileOutput)
        {
            if (File.Exists(fileInput))
            {
                // Load image.
                Image image = Image.FromFile(fileInput);

                // Compute thumbnail size.
                Size thumbnailSize = GetThumbnailSize(image);

                // Get thumbnail.
                Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width,
                    thumbnailSize.Height, null, IntPtr.Zero);
                // Save thumbnail.
                image.Dispose();
                // Save thumbnail.
                thumbnail.Save(fileOutput);
            }
                return true;
        }
        public static Size GetThumbnailSize(Image original)
        {
            // Maximum size of any dimension.
            const int maxPixels = 250;

            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }
        public static IEnumerable<SelectListItem> GetForGenderList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Bé trai", Value = Constants.GENDER_MALE},
                new SelectListItem{Text = "Bé gái", Value = Constants.GENDER_FEMALE},
                new SelectListItem{Text = "Cả hai", Value = Constants.GENDER_BOTH}
            };
            return items;
        }
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        public static string GetDescription(string content, int limit = 0)
        {
            string result = "";
            string[] data = content.Split(new string[] { @"<hr />" }, StringSplitOptions.None);
            if (data.Count() > 0)
            {
                result = data[0];

                if (limit != 0 && result.Length > limit)
                {
                    result = string.Join(" ", result.Split().Take(limit));
                    result += "..";
                }
            }

            return StripHTML(result);
        }

        public static string GetDescriptionHTML(string content, int limit = 0)
        {
            string result = "";
            string[] data = content.Split(new string[] { @"<hr />" }, StringSplitOptions.None);
            if (data.Count() > 0)
            {
                result = data[0];

                if (limit != 0 && result.Length > limit)
                {
                    result = string.Join(" ", result.Split().Take(limit));
                    result += "...";
                }
            }

            return result;
        }

        public static string GetDescriptionByChar(string content, int limit = 0)
        {
            string result = "";
            string[] data = content.Split(new string[] { @"<hr />" }, StringSplitOptions.None);
            if (data.Count() > 0)
            {
                result = data[0];

                if (limit != 0 && result.Length > limit)
                {
                    result = result.Substring(0, limit);
                    result += "...";
                }
            }

            return StripHTML(result);
        }

        public static string GetYoutubeDurationString(string duration)
        {
            if (string.IsNullOrEmpty(duration)) return "";
            StringBuilder sb = new StringBuilder();
            TimeSpan youTubeDuration = System.Xml.XmlConvert.ToTimeSpan(duration);
            if (youTubeDuration.Hours > 0)
            {
                sb.Append(youTubeDuration.Hours);
                sb.Append(":");
            }
            sb.Append(youTubeDuration.Minutes);
            sb.Append(":");

            sb.AppendFormat("{0:00}", youTubeDuration.Seconds);
            return sb.ToString();
        }
        public static string GetUniqueString()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static int GetRandomInt(int from, int to)
        {
            Random rnd = new Random();
            return rnd.Next(from, to); // creates a number between from and to
        }
        public static string ToUrlSlug(string value)
        {
            value = RemoveUnicode(value);

            //First to lower case 
            value = value.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);

            value = Encoding.ASCII.GetString(bytes);

            //Replace spaces 
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars 
            value = Regex.Replace(value, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);

            //Trim dashes from end 
            value = value.Trim('-', '_');

            //Replace double occurences of - or \_ 
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",  
                                            "đ",  
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",  
                                            "í","ì","ỉ","ĩ","ị",  
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",  
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",  
                                            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",  
                                            "d",  
                                            "e","e","e","e","e","e","e","e","e","e","e",  
                                            "i","i","i","i","i",  
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",  
                                            "u","u","u","u","u","u","u","u","u","u","u",  
                                            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        //public static string GetFilename(string filePath)
        //{
        //    System.IO.Path.GetFileName(filePath)

        //}

        public static string GetFileNameReal(string item)
        {
            string FileName = System.IO.Path.GetFileNameWithoutExtension(item);
            string[] List = FileName.Split('_');
            if (List.Length > 1)
            {
                return List[1];
            }
            else return FileName;
        }

        public static string FormatAlertMsg(string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("alert('");
            sb.Append(msg);
            sb.Append("');");
            sb.Append("</script>");
            return sb.ToString();
        }

        public static string FormatAlertMsgAndRedirect(string msg, string redirectUrl)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("alert('");
            sb.Append(msg);
            sb.Append("');");
            sb.Append("window.location = ");
            sb.Append(redirectUrl);
            sb.Append("</script>");
            return sb.ToString();
        }
    }
}

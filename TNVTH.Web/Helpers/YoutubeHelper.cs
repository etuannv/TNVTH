using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using TNVTH.Web.Models;
using YoutubeExtractor;

namespace TNVTH.Web.Helpers
{
    public static class YoutubeHelper
    {
        public static List<VideoInfo> GetLinkDownloadYoutube(string youtubeLink)
        {
            List<VideoInfo> videoInfos = new List<VideoInfo>();
            string GoodUrl;
            bool IsValidLink = DownloadUrlResolver.TryNormalizeYoutubeUrl(youtubeLink, out GoodUrl);

            if (IsValidLink)
            {

                videoInfos = DownloadUrlResolver.GetDownloadUrls(GoodUrl, false).ToList();
                var result = from q in videoInfos
                             where q.AudioBitrate > 0 && q.Resolution > 0 && q.VideoExtension != "webm"
                             select q;

            }
            return videoInfos;
        }


        public static bool IsAliveVideo(string youtubeId)
        {
            YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = WebConfigurationManager.AppSettings["ApiKey"],
                ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            });

            var searchListRequest = youtubeService.Videos.List("contentDetails");
            searchListRequest.Id = youtubeId;
            searchListRequest.MaxResults = 1;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = searchListRequest.Execute();//.ExecuteAsync();
            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            if (searchListResponse.PageInfo.TotalResults > 0)
            {
                var searchResult = searchListResponse.Items[0];
                if (searchResult.ContentDetails.RegionRestriction == null) return true;
                if (searchResult.ContentDetails.RegionRestriction.Blocked == null) return true;
                if (!searchResult.ContentDetails.RegionRestriction.Blocked.Contains("VI"))
                {
                    return true;
                }
            }
            return false;
        }
        public static YoutubeVideoObject GetVideoByUId(string youtubeId)
        {
            YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = WebConfigurationManager.AppSettings["ApiKey"],
                ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            });

            var searchListRequest = youtubeService.Videos.List("snippet,contentDetails,statistics,recordingDetails");
            searchListRequest.Id = youtubeId;
            searchListRequest.MaxResults = 1;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = searchListRequest.Execute();//.ExecuteAsync();
            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            if (searchListResponse.PageInfo.TotalResults > 0)
            {
                YoutubeVideoObject Video = new YoutubeVideoObject();
                var searchResult = searchListResponse.Items[0];
                Video.YoutubeId = searchResult.Id;
                Video.UChanelID = searchResult.Snippet.ChannelId;
                Video.UCategoryId = searchResult.Snippet.CategoryId;
                Video.UTitle = searchResult.Snippet.Title;
                Video.UDescription = searchResult.Snippet.Description;
                Video.UDuration = searchResult.ContentDetails.Duration;

                Video.ULiked = searchResult.Statistics.LikeCount;
                Video.UDisliked = searchResult.Statistics.DislikeCount;
                Video.UViewCount = searchResult.Statistics.ViewCount;
                StringBuilder sbTag = new StringBuilder();
                foreach (var tag in searchResult.Snippet.Tags)
                {
                    sbTag.Append(tag);
                    sbTag.Append(";");
                }
                Video.UTags = sbTag.ToString();
                Video.UDefaultLanguage = searchResult.Snippet.DefaultAudioLanguage;
                Video.Ulatitude = searchResult.RecordingDetails.Location.Latitude;
                Video.Ulongitude = searchResult.RecordingDetails.Location.Longitude;
                Video.Ualtitude = searchResult.RecordingDetails.Location.Altitude;
                Video.UThumbnail = searchResult.Snippet.Thumbnails.Medium.Url;
                return Video;
            }
            return null;
        }

        public static List<YoutubePlaylistObject> FeedRelatedVideo(string youtubeId)
        {
            List<YoutubePlaylistObject> ListVideo = new List<YoutubePlaylistObject>();
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = WebConfigurationManager.AppSettings["ApiKey"],
                ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.RelatedToVideoId = youtubeId;
            searchListRequest.MaxResults = 50;
            searchListRequest.Type = "video";

            //Call fist request
            // Call the search.list method to retrieve results matching the specified query term.
            Google.Apis.YouTube.v3.Data.SearchListResponse searchListResponse;
            searchListResponse = searchListRequest.Execute();//.ExecuteAsync();
            //Break if nothing to feed
            int Round = 0;
            do
            {
                searchListResponse = searchListRequest.Execute();//.ExecuteAsync();
                //Break if nothing to feed
                ListVideo.AddRange(ProcessResult(searchListResponse, null));
                searchListRequest.PageToken = searchListResponse.NextPageToken;
                Round++;

            } while (!string.IsNullOrEmpty(searchListResponse.NextPageToken) || Round > 10);

            return ListVideo;
        }
        public static List<YoutubePlaylistObject> GetVideoByChanelId(string chanelId, int lastFeedIndex)
        {
            List<YoutubePlaylistObject> ListVideo = new List<YoutubePlaylistObject>();
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = WebConfigurationManager.AppSettings["ApiKey"],
                ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.ChannelId = chanelId;
            searchListRequest.MaxResults = 50;
            searchListRequest.Type = "video,playlist";

            int MiddlePage = lastFeedIndex % 50;
            int Page = lastFeedIndex / 50;
            int Round = 0;
            //Call fist request
            // Call the search.list method to retrieve results matching the specified query term.
            Google.Apis.YouTube.v3.Data.SearchListResponse searchListResponse;
            do
            {
                searchListResponse = searchListRequest.Execute();//.ExecuteAsync();
                //Break if nothing to feed
                if (searchListResponse.PageInfo.TotalResults <= lastFeedIndex) break;
                if (Round == Page)
                {
                    ListVideo.AddRange(ProcessResult(searchListResponse, MiddlePage));
                }
                else if (Round > Page)
                {
                    ListVideo.AddRange(ProcessResult(searchListResponse, null));
                }

                searchListRequest.PageToken = searchListResponse.NextPageToken;
                Round++;

            } while (!string.IsNullOrEmpty(searchListResponse.NextPageToken) || (Round - Page) > 10);
            return ListVideo;
        }


         private static List<YoutubePlaylistObject> ProcessResult(Google.Apis.YouTube.v3.Data.SearchListResponse searchListResponse, int? MiddlePage)
        {
            int Loop = 0;
            if (MiddlePage.HasValue) Loop = (int)MiddlePage;
            List<YoutubePlaylistObject> ListVideo = new List<YoutubePlaylistObject>();
            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            for (int i = Loop; i < searchListResponse.Items.Count; i++)
            {
                var searchResult = searchListResponse.Items[i];
                YoutubePlaylistObject ItemData;
                if (searchResult.Id.Kind == "youtube#video")
                {
                    ItemData = new YoutubeVideoObject();
                    ItemData.IsPlaylist = false;
                    ItemData.YoutubeId = searchResult.Id.VideoId;
                }
                else
                {
                    ItemData = new YoutubePlaylistObject();
                    ItemData.IsPlaylist = true;
                    ItemData.YoutubeId = searchResult.Id.PlaylistId;
                }
                
                ItemData.UTitle = searchResult.Snippet.Title;
                ItemData.UChanelID = searchResult.Snippet.ChannelId;
                ItemData.UDescription = searchResult.Snippet.Description;
                ItemData.UThumbnail = searchResult.Snippet.Thumbnails.Medium.Url;
                ListVideo.Add(ItemData);
            }
            return ListVideo;
        }

        public static List<YoutubeChanelObject> GetChanelByUser(string username)
        {
            YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = WebConfigurationManager.AppSettings["ApiKey"],
                ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            });

            var searchListRequest = youtubeService.Channels.List("snippet");
            searchListRequest.ForUsername = username;
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = searchListRequest.Execute();//.ExecuteAsync();
            List<YoutubeChanelObject> ListChanel = new List<YoutubeChanelObject>();
            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                YoutubeChanelObject Chanel = new YoutubeChanelObject();
                Chanel.UChanelID = searchResult.Id;
                Chanel.UTitle = searchResult.Snippet.Title;
                ListChanel.Add(Chanel);
            }
            return ListChanel;
        }
    }

    public class YoutubePlaylistObject
    {
        public string YoutubeId { get; set; }
        public string UChanelID { get; set; }
        public string UTitle { get; set; }
        public string UDescription { get; set; }
        public string UThumbnail { get; set; }

        public bool IsPlaylist { get; set; }
    }
    public class YoutubeChanelObject
    {
        public string UChanelID { get; set; }
        public string UTitle { get; set; }
    }
    public class YoutubeVideoObject : YoutubePlaylistObject
    {

        public string UCategoryId { get; set; }

        public string UDuration { get; set; }
        public ulong? ULiked { get; set; }
        public ulong? UDisliked { get; set; }
        public ulong? UViewCount { get; set; }
        public string UTags { get; set; }
        public string UDefaultLanguage { get; set; }
        public double? Ulatitude { get; set; }
        public double? Ulongitude { get; set; }
        public double? Ualtitude { get; set; }
        public YoutubeVideoObject() { }
        public YoutubeVideoObject(string iYoutubeId, string iUChanelID, string iUCategoryId, string iUTitle, string iUDescription
                                    , string iUDuration, ulong? iULiked, ulong? iUDisliked, ulong? iUViewCount
                                    , string iUTags, string iUDefaultLanguage, double? iUlatitude, double? iUlongitude
                                    , double? iUaltitude, string iUThumbnail)
        {
            YoutubeId = iYoutubeId;
            UChanelID = iUChanelID;
            UCategoryId = iUCategoryId;
            UTitle = iUTitle;
            UDescription = iUDescription;
            UDuration = iUDuration;
            ULiked = iULiked;
            UDisliked = iUDisliked;
            UViewCount = iUViewCount;
            UTags = iUTags;
            UDefaultLanguage = iUDefaultLanguage;
            Ulatitude = iUlatitude;
            Ulongitude = iUlongitude;
            Ualtitude = iUaltitude;
            UThumbnail = iUThumbnail;
        }
    }
}
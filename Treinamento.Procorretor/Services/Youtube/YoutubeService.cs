using System.Reflection;
using System.Text.Json;

using Newtonsoft.Json;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using Treinamento.Procorretor.Services.Youtube;
using static Treinamento.Procorretor.Domain.YoutubeJson;
using static Treinamento.Procorretor.Domain.PlayListJson;

namespace Treinamento.Procorretor.Services
{
    /// <summary>
    /// YouTube Data API v3 sample: upload a video.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://developers.google.com/api-client-library/dotnet/get_started
    /// </summary>
    public class YoutubeService : IYoutubeService
    {
        private readonly IConfiguration _Config;
        public string? VideoTitle { get; set; }
        public string? VideoId { get; set; }
        public string? ThumbNailUrl { get; set; }
        public string? VideoDescription { get; set; }

        public YoutubeService(IConfiguration Config)
        {
            _Config = Config;
        }

        public async Task UploadVideo(
            string? title,
            string? description,
            string? tags,
            string? categoryId,
            //string privacyStatus,
            string? file)
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = title; //"Default Video Title";
            video.Snippet.Description = description; //"Default Video Description";
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"
            var filePath = @file;// @"REPLACE_ME.mp4"; // Replace with path to actual movie file.

            using (var fileStream = new FileStream(filePath!, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }

        private void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        private void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }

        public async Task CreatePlayList()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            // Create a new, private playlist in the authorized user's channel.
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = "Test Playlist";
            newPlaylist.Snippet.Description = "A playlist created with the YouTube API v3";
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = "public";
            newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

            // Add a video to the newly created playlist.
            var newPlaylistItem = new PlaylistItem();
            newPlaylistItem.Snippet = new PlaylistItemSnippet();
            newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
            newPlaylistItem.Snippet.ResourceId = new Google.Apis.YouTube.v3.Data.ResourceId();
            newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
            newPlaylistItem.Snippet.ResourceId.VideoId = "GNRMeaz6QRI";
            newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

            // Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, newPlaylist.Id);
        }

        public async Task GetPlayList()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                    // user's account, but not other types of account access.
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            // var channelsListRequest = youtubeService.Channels.List("contentDetails");
            var playlistListRequest = youtubeService.Playlists.List("snippet");
            playlistListRequest.Mine = true;

            // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
            var playlistListResponse = await playlistListRequest.ExecuteAsync();

            foreach (var playlist in playlistListResponse.Items)
            {
                // From the API response, extract the playlist ID that identifies the list
                // of videos uploaded to the authenticated user's channel.
                var playlistListId = playlist.Id;

                Console.WriteLine("Videos in list {0}", playlistListId);
            }
        }

        public async Task<dynamic> GetVideosInPlayListAsync(string playListId)
        {
            var parameters = new Dictionary<string, string>
            {
                // ["key"] = ConfigurationManager.AppSettings["APIKey"],
                ["key"] = _Config.GetSection("appSettings").GetSection("APIKey").Value!,
                ["playlistId"] = "PL285LgYq_FoKoxiqmUEgVX3_wFf1ioi3J", //playListId,
                ["part"] = "snippet",
                ["fields"] = "pageInfo, items/snippet(title, description, thumbnails(default))",
                ["maxResults"] = "50"
            };

            var baseUrl = "https://www.googleapis.com/youtube/v3/playlistItems?";
            var fullUrl = MakeUrlWithQuery(baseUrl, parameters);

            var result = await new HttpClient().GetStringAsync(fullUrl);

            if (result != null)
            {
                return JsonConvert.DeserializeObject(result)!;
            }

            return default(dynamic)!;
        }

        private static string MakeUrlWithQuery(string baseUrl,
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (parameters == null || parameters.Count() == 0)
                return baseUrl;

            return parameters.Aggregate(baseUrl,
                (accumulated, kvp) => string.Format($"{accumulated}{kvp.Key}={kvp.Value}&"));
        }

        private static void PrintResult(dynamic result)
        {
            var count = result.items.Count;
            Console.WriteLine($"Total items in playlist: {result.pageInfo.totalResults,2}");
            Console.WriteLine($"Public items in playlist: {count,2}");
            Console.WriteLine("-----------------------------------------------------");

            var i = 0;

            if (count > 0)
                foreach (var item in result.items)
                    Console.WriteLine(string.Format($"{++i,3}) {item.snippet.title}"));
        }

        public async Task<string> GetVideosPlayListAsync()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                    // user's account, but not other types of account access.
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            var channelsListRequest = youtubeService.Channels.List("contentDetails");
            channelsListRequest.Mine = true;

            // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
            var channelsListResponse = await channelsListRequest.ExecuteAsync();

            var obj = new List<Root>();
            var obj2 = new List<YoutubePlayList>();
            string jsonString = "";

            foreach (var channel in channelsListResponse.Items)
            {
                // From the API response, extract the playlist ID that identifies the list
                // of videos uploaded to the authenticated user's channel.
                var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;

                // Console.WriteLine("Videos in list {0}", uploadsListId);

                var nextPageToken = "";
                while (nextPageToken != null)
                {
                    var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
                    playlistItemsListRequest.PlaylistId = uploadsListId;
                    playlistItemsListRequest.MaxResults = 50;
                    playlistItemsListRequest.PageToken = nextPageToken;
                    // playlistItemsListRequest.Fields = "pageInfo, items/snippet(title, description)";

                    // Retrieve the list of videos uploaded to the authenticated user's channel.
                    var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

                    foreach (var playlistItem in playlistItemsListResponse.Items)
                    {
                        // var objReg = new Root()
                        // {
                        //     title = playlistItem.Snippet.Title,
                        //     videoId = playlistItem.Snippet.ResourceId.VideoId,
                        //     thumbnail = playlistItem.Snippet.Thumbnails.Default__.Url,
                        //     description = playlistItem.Snippet.Description,
                        //     // tags = playlistItem.Snippet.ETag,
                        //     privacyStatus = playlistItem.Status.PrivacyStatus
                        // };

                        // obj.Add(objReg);
                        // Console.WriteLine("{0} ({1})", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId,
                        //     playlistItem.ContentDetails.VideoId);

                        YoutubePlayList? playList = new()
                        {
                            // items = new List<Item>()
                            // {
                            //     new Item()
                            //     {
                            //         snippet = new Snippet()
                            //         {
                            //             title = playlistItem.Snippet.Title,
                            //             description = playlistItem.Snippet.Description,
                            //             thumbnails = new Thumbnails
                            //             {
                            //                 defaultThumbnail = new DefaultThumbnail
                            //                 {
                            //                     url = playlistItem.Snippet.Thumbnails.Default__.Url,
                            //                     width = playlistItem.Snippet.Thumbnails.Default__.Width,
                            //                     height = playlistItem.Snippet.Thumbnails.Default__.Height
                            //                 }
                            //             }
                            //         }
                            //     }
                            // }


                            kind = playlistItem.Kind,
                            etag = playlistItem.ETag,
                            id = playlistItem.Id,
                            snippets = new List<Snippet>()
                            {
                                new Snippet()
                                {
                                    // publishedAt = playlistItem.Snippet.PublishedAt,
                                    channelId = playlistItem.Snippet.ChannelId,
                                    title = playlistItem.Snippet.Title,
                                    description = playlistItem.Snippet.Description,
                                    thumbnails = new Thumbnails()
                                    {
                                        Default = new Default()
                                        {
                                            url = playlistItem.Snippet.Thumbnails.Default__.Url,
                                            width = playlistItem.Snippet.Thumbnails.Default__.Width,
                                            height = playlistItem.Snippet.Thumbnails.Default__.Height
                                        }
                                    },
                                    channelTitle = playlistItem.Snippet.ChannelTitle,
                                    videoOwnerChannelTitle = playlistItem.Snippet.ChannelTitle,
                                    videoOwnerChannelId = playlistItem.Snippet.ChannelId,
                                    playlistId = playlistItem.Snippet.PlaylistId,
                                    position = playlistItem.Snippet.Position,
                                    resourceId = new Treinamento.Procorretor.Domain.PlayListJson.ResourceId()
                                    {
                                        kind = playlistItem.Snippet.ResourceId.Kind,
                                        videoId = playlistItem.Snippet.ResourceId.VideoId
                                    }
                                },
                            },
                            contentDetails = new ContentDetails()
                            {
                                videoId = playlistItem.ContentDetails?.VideoId,
                                startAt = playlistItem.ContentDetails?.StartAt,
                                endAt = playlistItem.ContentDetails?.EndAt,
                                note = playlistItem.ContentDetails?.Note
                                // videoPublishedAt = playlistItem.ContentDetails.VideoPublishedAt,
                            },
                            status = new Status()
                            {
                                privacyStatus = playlistItem.Status?.PrivacyStatus
                            }                               
                        };
                        obj2.Add(playList);
                    };
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    jsonString = System.Text.Json.JsonSerializer.Serialize(obj2, options);

                    // Console.WriteLine(jsonString);

                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
            }
            return jsonString;
        }
    }
}

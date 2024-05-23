namespace Treinamento.Procorretor.Domain
{
    public class PlayListJson
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class YoutubePlayList
        {
            public string? kind { get; set; }
            public string? etag { get; set; }
            public string? id { get; set; }
            public List<Snippet>? snippets { get; set; }
            public ContentDetails? contentDetails { get; set; }
            public Status? status { get; set; }
        }

        public class Snippet
        {
            public DateTime? publishedAt { get; set; }
            public string? channelId { get; set; }
            public string? title { get; set; }
            public string? description { get; set; }
            public Thumbnails? thumbnails { get; set; }
            public string? channelTitle { get; set; }
            public string? videoOwnerChannelTitle { get; set; }
            public string? videoOwnerChannelId { get; set; }
            public string? playlistId { get; set; }
            public long? position { get; set; }
            public ResourceId? resourceId { get; set; }
        }

        public class Thumbnails
        {
            public Default? Default { get; set; }
        }          

        public class Default
        {
            public string? url { get; set; }
            public long? width { get; set; }
            public long? height { get; set; }
        }

        public class ContentDetails
        {
            public string? videoId { get; set; }
            public string? startAt { get; set; }
            public string? endAt { get; set; }
            public string? note { get; set; }
            public DateTime? videoPublishedAt { get; set; }
        }

        public class ResourceId
        {
            public string? kind { get; set; }
            public string? videoId { get; set; }
        }      

        public class Status
        {
            public string? privacyStatus { get; set; }
        }

        // // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

        // public class YoutubePlayList
        // {
        //     public IList<Item>? items { get; set; }
        //     public PageInfo? pageInfo { get; set; }
        // }

        // public class Item
        // {
        //     public Snippet? snippet { get; set; }
        // }

        // public class Snippet
        // {
        //     public string? title { get; set; }
        //     public string? description { get; set; }
        //     public Thumbnails? thumbnails { get; set; }
        // }

        // public class Thumbnails
        // {
        //     public DefaultThumbnail? defaultThumbnail { get; set; }
        // }

        // public class DefaultThumbnail
        // {
        //     public string? url { get; set; }
        //     public long? width { get; set; }
        //     public long? height { get; set; }
        // }

        // public class PageInfo
        // {
        //     public int? totalResults { get; set; }
        //     public int? resultsPerPage { get; set; }
        // }
    }
}
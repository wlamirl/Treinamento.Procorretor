namespace Treinamento.Procorretor.Domain
{
    public class YoutubeJson
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Root
        {
            public string? videoId { get; set; }
            public string? title { get; set; }
            public string? thumbnail { get; set; }
            public string? description { get; set; }
            public string? tags { get; set; }
            // public IList<Tags> tags { get; set; }
            public string? categoryId { get; set; }
            public string? privacyStatus { get; set; }
            public string? filePath { get; set; }
            public string? urlVideo { get; set; }
        }

        // public class Tags
        // {
        //     public string? tag1 { get; set; }
        //     public string? tag2 { get; set; }
        //     public string? tag3 { get; set; }
        // }
    }
}
namespace Treinamento.Procorretor.Services.Youtube
{
    public interface IYoutubeService
    {
        Task UploadVideo(
            string? title, 
            string? description, 
            string? tags, 
            string? categoryId,
            //string privacyStatus,
            string? file);

        Task GetPlayList();
        Task<dynamic> GetVideosInPlayListAsync(string playListId);
        Task<string> GetVideosPlayListAsync();
    }
}
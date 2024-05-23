using Treinamento.Procorretor.Models;

namespace Treinamento.Procorretor.Services
{
    public interface IVideoService
    {
        Task<List<VideoModel>> GetVideos();
        Task<VideoModel> GetVideo(int? id);
        Task<VideoModel> AddVideo(VideoModel video);
        Task<VideoModel> UpdateVideo(VideoModel video);
        Task<VideoModel> DeleteVideo(int id);
    }
}
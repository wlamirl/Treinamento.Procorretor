using Treinamento.Procorretor.Data;
using Treinamento.Procorretor.Models;
using Microsoft.EntityFrameworkCore;

namespace Treinamento.Procorretor.Services
{
    public class VideoService : IVideoService
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _wHEnv;

        public VideoService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _context = context;
            _wHEnv = webHostEnvironment;
        }

        public async Task<VideoModel> AddVideo(VideoModel video)
        {
            if(video is null)
                throw new ArgumentNullException(nameof(video));

            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<VideoModel> DeleteVideo(int id)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(x => x.Id == id);

            if (video == null)
            {
                throw new EntityNotFoundException($"Video com ID {id} não encontrado!");
            }

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return video;
        }

        public async Task<VideoModel> GetVideo(int? id)
        {
            if (id == null)
            {
                throw new EntityNotFoundException($"Vídeo com o Id {nameof(id)} não encontrado");
            }
            
            var video = await _context.Videos.FirstOrDefaultAsync(a => a.Id == id);
            return video ?? new VideoModel();
        }

        public async Task<List<VideoModel>> GetVideos()
        {
            // string[] filePaths = Directory.GetFiles(Path.Combine(_wHEnv.WebRootPath, @"images"));
            // List<VideoModel> videolist = new List<VideoModel>();
            // foreach (string filePath in filePaths)
            // {
            //     var vid = new VideoModel();
            //     vid.Thumbnail = @"images/" + Path.GetFileName(filePath);
            //     vid.Title = Path.GetFileName(filePath);
            //     // vid. = Path.GetExtension(filePath);
            //     videolist.Add(vid);
            // }
            // return videolist;
            return await _context.Videos.ToListAsync();
        }

        public async Task<VideoModel> UpdateVideo(VideoModel video)
        {
            if (video is null)
                throw new ArgumentNullException(nameof(video));

            _context.Entry(video).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return video;
        }
    }
}
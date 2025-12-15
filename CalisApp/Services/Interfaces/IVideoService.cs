using CalisApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalisApp.Services.Interfaces
{
    public interface IVideoService
    {
        Task<IEnumerable<Video>> GetAllVideos();

        Task<IEnumerable<Video>> GetAllVideosByCategory(int categoryId);

        Task<IEnumerable<Video>> SearchVideos(string searchTerm);

        Task<Video> GetVideoDetails(int id);
    }
}

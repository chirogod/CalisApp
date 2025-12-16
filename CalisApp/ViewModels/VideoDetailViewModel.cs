using CalisApp.Models;
using CalisApp.Services;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CalisApp.ViewModels
{
    [QueryProperty(nameof(VideoId), "Id")]
    public class VideoDetailViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IVideoService _videoService;

        private int _videoId;

        private Video _video;

        public VideoDetailViewModel(IAuthService authService, IVideoService videoService)
        {
            _authService = authService;
            _videoService = videoService;
        }
        public int VideoId
        {
            get => _videoId;
            set
            {
                if (_videoId != value)
                {
                    _videoId = value;
                    Task.Run(async () => await CargarDatos(_videoId));

                }
            }
        }

        public Video video
        {
            get => _video;
            set
            {
                if (_video != value)
                {
                    _video = value;
                    OnPropertyChanged(nameof(video));

                }
            }
        }

        private async Task CargarDatos(int id)
        {
            await LoadVideoDetails(id);
        }


        private async Task LoadVideoDetails(int id)
        {
            if (id == 0) return;

            try
            {
                var fetchedVideo = await _videoService.GetVideoDetails(id);
                var SessionUser = await _authService.ObtenerSesion();
                int UserId = int.Parse(SessionUser.Id);

                if (fetchedVideo != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        video = fetchedVideo;

                        Debug.WriteLine($"✅ Detalles de video {id} cargados con éxito. url: {video.Url} - category: {video.Category.Name}");

                    });
                }
                else
                {
                    Debug.WriteLine($"⚠️ ERROR: video {id} no encontrado en el endpoint.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR al cargar detalles de video: {ex.Message}");
            }
        }
    }


}

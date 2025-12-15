using CalisApp.Models;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace CalisApp.ViewModels
{
    public class VideosViewModel : BaseViewModel
    {
        private readonly IVideoService _videoService;
        private readonly ICategoryService _categoryService;

        private ObservableCollection<Video> _videos;
        private ObservableCollection<Category> _categories;

        private string _searchText;
        private bool _isSelected;
        public ICommand SelectFilterCommand { get; }
        public ICommand SearchVideosCommand { get; }

        public ICommand GoToDetailCommand { get; }
        public VideosViewModel(IVideoService videoService, ICategoryService categoryService)
        {
            _videoService = videoService;
            _categoryService = categoryService;
            _videos = new ObservableCollection<Video>();
            _categories = new ObservableCollection<Category>();

            SelectFilterCommand = new Command<Category>(async (category) => await SelectFilterAsync(category));
            SearchVideosCommand = new Command(async () => await SearchVideosAsync());

            GoToDetailCommand = new Command<Video>(async (video) => await GoToDetailAsync(video));

            Task.Run(async () => await LoadVideosAsync(0, string.Empty));
            Task.Run(async () => await LoadCategoriesAsync());
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value) 
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        var todos = Categories.FirstOrDefault(c => c.Id == 0);
                        if (todos != null)
                        {
                            foreach (var c in Categories) c.IsSelected = false;
                            todos.IsSelected = true;
                        }

                        Task.Run(async () => await LoadVideosAsync(0, string.Empty));
                    }
                }
            }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }


        public ObservableCollection<Video> Videos
        {
            get { return _videos; }
            set
            {
                _videos = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        private async Task SelectFilterAsync(Category category)
        {
            if (category is null) return;

            SearchText = string.Empty;
            bool alreadySelected = category.IsSelected;

            foreach (var c in Categories)
            {
                c.IsSelected = false;
            }

            if (!alreadySelected)
            {
                category.IsSelected = true;
                await LoadVideosAsync(category.Id, string.Empty);
            }
            else
            {
                await LoadVideosAsync(0, string.Empty);
            }
        }
        private async Task SearchVideosAsync()
        {
            foreach (var c in Categories)
            {
                c.IsSelected = false;
            }
            if (string.IsNullOrEmpty(SearchText))
            {
                var todos = Categories.FirstOrDefault(c => c.Id == 0);
                if (todos != null) todos.IsSelected = true;
            }

            await LoadVideosAsync(0, SearchText ?? string.Empty);
        }


        public async Task LoadVideosAsync(int categoryId, string searchTerm)
        {
            try
            {

                IEnumerable<Video> videosFromService;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    videosFromService = await _videoService.SearchVideos(searchTerm);
                }
                else if (categoryId > 0)
                {
                    videosFromService = await _videoService.GetAllVideosByCategory(categoryId);
                }
                else
                {
                    videosFromService = await _videoService.GetAllVideos();
                }

                Videos.Clear();
                foreach (var video in videosFromService)
                {
                    Videos.Add(video);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar videos: {ex.Message}");
            }
        }

        public async Task LoadCategoriesAsync()
        {
            try
            {
                var categoriesFromService = await _categoryService.GetAllCategories();

                Categories.Clear();
                Categories.Add(new Category { Id = 0, Name = "Todos", Description = "Todos los videos", IsSelected = true });

                foreach (var category in categoriesFromService)
                {
                    category.IsSelected = false;
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar categorías: {ex.Message}");
            }
        }

        private async Task GoToDetailAsync(Video video)
        {
            if (video == null) return;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                string route = $"VideoDetail?Id={video.Id}";
                await Shell.Current.GoToAsync(route);
            });
        }
    }
}

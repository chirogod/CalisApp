using CalisApp.ViewModels;

namespace CalisApp.Views;

public partial class VideosView : ContentPage
{
	public VideosView(VideosViewModel video)
	{
		InitializeComponent();
		BindingContext = video;
    }
}
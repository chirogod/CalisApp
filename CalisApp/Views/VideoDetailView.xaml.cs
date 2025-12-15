using CalisApp.ViewModels;

namespace CalisApp.Views;

public partial class VideoDetailView : ContentPage
{
	public VideoDetailView(VideoDetailViewModel v)
	{
		InitializeComponent();
		BindingContext = v;
    }
}
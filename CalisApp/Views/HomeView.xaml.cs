using CalisApp.ViewModels;

namespace CalisApp.Views;

public partial class HomeView : ContentPage
{
	public HomeView(HomeViewModel viewmodel)
	{
		InitializeComponent();
		this.BindingContext = viewmodel;
	}
}
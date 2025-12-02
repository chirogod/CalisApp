using CalisApp.ViewModels;

namespace CalisApp.Views;

public partial class ProfileView : ContentPage
{
	public ProfileView(ProfileViewModel viewmodel)
	{
		InitializeComponent();
		BindingContext = viewmodel;
    }
}
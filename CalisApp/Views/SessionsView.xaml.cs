using CalisApp.ViewModels;

namespace CalisApp.Views;

public partial class SessionsView : ContentPage
{
	public SessionsView(SessionsViewModel sessionsViewModel)
	{
		InitializeComponent();
		BindingContext = sessionsViewModel;
	}
}
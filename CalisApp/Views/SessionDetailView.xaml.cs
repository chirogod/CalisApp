using CalisApp.ViewModels;
namespace CalisApp.Views;

public partial class SessionDetailView : ContentPage
{
    public SessionDetailView(SessionDetailViewModel ses)
	{
		InitializeComponent();
        BindingContext = ses;
	}
}
using CalisApp.Services.Interfaces;
using CalisApp.ViewModels;
using System.Diagnostics;

namespace CalisApp.Views;

public partial class LoginView : ContentPage
{

	public LoginView(LoginViewModel login)
	{
		InitializeComponent(); 
		this.BindingContext = login;
    }

    private void clicjing(object sender, EventArgs e)
    {
        Debug.WriteLine("clickinng login button");
    }
}
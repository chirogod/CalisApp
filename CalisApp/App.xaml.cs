using CalisApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CalisApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var loginPage = _serviceProvider.GetRequiredService<LoginView>();

            return new Window(loginPage);
        }
    }
}
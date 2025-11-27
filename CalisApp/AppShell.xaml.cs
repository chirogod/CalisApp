using CalisApp.Views;

namespace CalisApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("SessionDetail", typeof(SessionDetailView));
        }
    }
}

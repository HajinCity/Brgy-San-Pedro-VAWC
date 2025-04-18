namespace Brgy_San_Pedro_VAWC
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LandingPage), typeof(LandingPage));
            Routing.RegisterRoute(nameof(FilingOfComplaint), typeof(FilingOfComplaint));

        }
    }
}

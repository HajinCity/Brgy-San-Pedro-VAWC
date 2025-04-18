using Microsoft.Maui.Controls;

namespace Brgy_San_Pedro_VAWC;

public partial class LandingPage : ContentPage
{
    public LandingPage()
    {
        InitializeComponent();
    }

    private async void OnFileComplaintTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(FilingOfComplaint));
    }
}
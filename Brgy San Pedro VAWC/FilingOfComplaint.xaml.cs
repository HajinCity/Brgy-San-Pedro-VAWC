using Brgy_San_Pedro_VAWC.Models;
using Brgy_San_Pedro_VAWC.Services;
using Microsoft.Maui.Controls;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Brgy_San_Pedro_VAWC;

public partial class FilingOfComplaint : ContentPage
{
    private readonly FirestoreService _firestoreService = new();

    public FilingOfComplaint()
    {
        InitializeComponent();
        RelationshipEntry.SelectedIndexChanged += OnRelationshipSelected;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnFileNowClicked(object sender, EventArgs e)
    {
        if (!IsFormValid())
        {
            await DisplayAlert("Incomplete", "Please fill in all required fields.", "OK");
            return;
        }

        await SubmitComplaintAsync();
    }

    private async Task SubmitComplaintAsync()
    {
        try
        {
            string caseId = await GenerateCaseIDAsync();

            var complaint = new Complaint
            {
                CaseID = caseId,
                Complainant = new ComplainantDetails
                {
                    LastName = LastNameEntry.Text,
                    FirstName = FirstNameEntry.Text,
                    MiddleName = MiddleNameEntry.Text,
                    SexIdentification = MaleRadio.IsChecked ? "Male" : FemaleRadio.IsChecked ? "Female" : "",
                    CivilStatus = CivilStatusEntry.SelectedItem?.ToString() ?? "",
                    Birthdate = BirthdatePicker.Date.ToString("yyyy-MM-dd"),
                    Age = AgeEntry.Text,
                    Religion = ReligionEntry.Text,
                    CellNumber = ContactEntry.Text,
                    Nationality = NationalityEntry.Text,
                    Occupation = OccupationEntry.Text,
                    Address = new Address
                    {
                        Purok = PurokEntry.SelectedItem?.ToString() ?? "N/A",
                        Barangay = "San Pedro",
                        Municipality = CityEntry.Text,
                        Province = "Zamboanga Del Sur",
                        Region = "IX"
                    }
                },
                Respondent = new RespondentDetails
                {
                    LastName = RespLastNameEntry.Text,
                    FirstName = RespFirstNameEntry.Text,
                    MiddleName = RespMiddleNameEntry.Text,
                    Alias = RespAliasEntry.Text,
                    SexIdentification = RespMaleRadio.IsChecked ? "Male" : RespFemaleRadio.IsChecked ? "Female" : "",
                    CivilStatus = RespCivilStatusEntry.SelectedItem?.ToString() ?? "",
                    Birthdate = RespBirthdatePicker.Date.ToString("yyyy-MM-dd"),
                    Age = RespAgeEntry.Text,
                    Religion = RespReligionEntry.Text,
                    CellNumber = RespContactEntry.Text,
                    Nationality = RespNationalityEntry.Text,
                    Occupation = RespOccupationEntry.Text,
                    RelationshipToComplainant = GetRelationship(),
                    Address = new Address
                    {
                        Purok = "N/A",
                        Barangay = "N/A",
                        Municipality = "N/A",
                        Province = "N/A",
                        Region = "N/A"
                    }
                },
                CaseDetails = new CaseDetails
                {
                    CaseNumber = caseId,
                    ComplaintDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                     TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
                    .ToString("yyyy-MM-dd HH:mm:ss"),
                    VAWCCase = "N/A",
                    SubCase = "N/A",
                    CaseStatus = "Pending",
                    ReferredTo = "N/A",
                    IncidentDate = IncidentDatePicker.Date.ToString("yyyy-MM-dd"),
                    IncidentDescription = ComplaintDetailsEditor.Text,
                    PlaceOfIncident = new IncidentLocation
                    {
                        Place = PlaceOftheIncident.SelectedItem?.ToString() ?? "N/A",
                        Purok = PIApurok.Text ?? "",
                        Barangay = PIAbarangay.Text ?? "",
                        Municipality = PIAmunicipal.Text ?? "",
                        Province = PIAprovine.Text ?? "",
                        Region = PIARegion.Text ?? ""
                    }
                }

            };

            bool success = await _firestoreService.SubmitComplaintAsync(complaint);

            if (success)
            {
                await DisplayAlert("Success", $"Complaint submitted!\nCase ID: {caseId}\n Please Go to the Barangay Office for Further instructions of your complaint", "OK");
                await ClearForm();
            }
            else
            {
                await DisplayAlert("Error", "Failed to submit complaint. Check your network or API key.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Submission error: {ex.Message}", "OK");
        }
    }

    private string GetRelationship()
    {
        return RelationshipEntry.SelectedItem?.ToString() == "Other (please specify)"
            ? OtherRelationshipEntry.Text
            : RelationshipEntry.SelectedItem?.ToString() ?? string.Empty;
    }

    private void OnRelationshipSelected(object? sender, EventArgs e)
    {
        OtherRelationshipEntry.IsVisible = RelationshipEntry.SelectedItem?.ToString() == "Other (please specify)";
    }

    private async Task<string> GenerateCaseIDAsync()
    {
        return await Task.Run(() =>
        {
            return $"CFO-{DateTime.UtcNow.Year}-{DateTime.UtcNow:MMddHHmmss}";
        });
    }

    private bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(LastNameEntry.Text) &&
               !string.IsNullOrWhiteSpace(FirstNameEntry.Text) &&
               !string.IsNullOrWhiteSpace(ContactEntry.Text) &&
               !string.IsNullOrWhiteSpace(CityEntry.Text) &&
               PurokEntry.SelectedItem != null &&
               !string.IsNullOrWhiteSpace(RespLastNameEntry.Text) &&
               !string.IsNullOrWhiteSpace(RespFirstNameEntry.Text) &&
               RelationshipEntry.SelectedItem != null &&
               (RelationshipEntry.SelectedItem.ToString() != "Other (please specify)" ||
                !string.IsNullOrWhiteSpace(OtherRelationshipEntry.Text)) &&
               !string.IsNullOrWhiteSpace(ComplaintDetailsEditor.Text);
    }

    private async Task ClearForm()
    {
        await Task.Run(() =>
        {
            Dispatcher.Dispatch(() =>
            {
                // Complainant
                LastNameEntry.Text = FirstNameEntry.Text = MiddleNameEntry.Text = "";
                ContactEntry.Text = CityEntry.Text = "";
                CivilStatusEntry.SelectedIndex = -1;
                BirthdatePicker.Date = DateTime.Today;
                AgeEntry.Text = ReligionEntry.Text = NationalityEntry.Text = OccupationEntry.Text = "";
                PurokEntry.SelectedIndex = -1;
                MaleRadio.IsChecked = false;
                FemaleRadio.IsChecked = false;

                // Respondent
                RespLastNameEntry.Text = RespFirstNameEntry.Text = RespMiddleNameEntry.Text = RespAliasEntry.Text = "";
                RespCivilStatusEntry.SelectedIndex = -1;
                RespBirthdatePicker.Date = DateTime.Today;
                RespAgeEntry.Text = RespReligionEntry.Text = RespContactEntry.Text = RespNationalityEntry.Text = RespOccupationEntry.Text = "";
                RespMaleRadio.IsChecked = false;
                RespFemaleRadio.IsChecked = false;

                // Relationship
                RelationshipEntry.SelectedIndex = -1;
                OtherRelationshipEntry.Text = "";
                OtherRelationshipEntry.IsVisible = false;

                // Incident
                IncidentDatePicker.Date = DateTime.Today;
                PlaceOftheIncident.SelectedIndex = -1;
                PIApurok.Text = PIAbarangay.Text = PIAmunicipal.Text = PIAprovine.Text = PIARegion.Text = "";
                ComplaintDetailsEditor.Text = "";
            });
        });
    }
}

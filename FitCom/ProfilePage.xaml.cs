using System;
using Microsoft.Maui.Controls;

namespace FitCom
{
    public partial class ProfilePage : ContentPage
    {
        FirebaseHelper firebaseHelper;

        public ProfilePage()
        {
            InitializeComponent();
            firebaseHelper = new FirebaseHelper();
            
            // Load existing user data when the page is initialized
            LoadUserData();
        }

        // Load existing user data when the page appears
        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            LoadUserData();
            
        }

        async void LoadUserData()
        {
            try
            {
                // Get the first user record (assuming only one user record exists)
                var userRecords = await firebaseHelper.GetAllUserRecords();
                if (userRecords != null && userRecords.Count > 0)
                {
                    var currentUserRecord = userRecords[0];

                    // Display existing user data in the input fields
                    inputName.Text = currentUserRecord.Name;
                    goalPicker.SelectedItem = currentUserRecord.Goal;
                    inputWeight.Text = currentUserRecord.Weight.ToString();
                    inputHeight.Text = currentUserRecord.Height.ToString();
                }
                else
                {
                    // Reset input fields if no user records exist
                    inputName.Text = "";
                    goalPicker.SelectedItem = null;
                    inputWeight.Text = "";
                    inputHeight.Text = "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        async void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            string selectedOption = picker.SelectedItem?.ToString();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            string name = inputName.Text;
            string goal = goalPicker.SelectedItem?.ToString();
            double weight = double.Parse(inputWeight.Text);
            double height = double.Parse(inputHeight.Text);

            // Calculate BMI and round it to two decimal places
            double bmi = Math.Round(weight / (height * height), 2);

            // Determine BMI status
            string bmiStatus = DetermineBMIStatus(bmi);

            // Delete previously existing user data (if any)
            await firebaseHelper.ClearUserRecords();

            // Add the user record to the database
            await firebaseHelper.AddUserRecord(name, goal, weight, height, bmi, bmiStatus);

            // Optionally, show a message or navigate to another page after saving
            await DisplayAlert("Success", "Data saved successfully!", "OK");

            // Navigate back to the main page
            await Shell.Current.GoToAsync("//MainPage");
        }

        async void OnResetButtonClicked(object sender, EventArgs e)
        {
            // Clear the user records in the database
            await firebaseHelper.ClearUserRecords();

            // Optionally, show a message or perform any additional tasks after resetting
            await DisplayAlert("Reset", "All user records have been cleared successfully!", "OK");

            // Clear and refresh input fields
            LoadUserData();
        }

        string DetermineBMIStatus(double bmi)
        {
            if (bmi < 18.5)
            {
                return "Underweight";
            }
            else if (bmi >= 18.5 && bmi < 25)
            {
                return "Normal";
            }
            else if (bmi >= 25 && bmi < 30)
            {
                return "Overweight";
            }
            else
            {
                return "Obese";
            }
        }
    }
}

using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace FitCom
{
    public partial class StatsPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();

        public StatsPage()
        {
            InitializeComponent();
            LoadUserData();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData(); // Load user exercises when page appears
        }

        private async void LoadUserData()
        {
            try
            {
                // Get the current user's user record
                var userRecords = await firebaseHelper.GetAllUserRecords();
                if (userRecords != null && userRecords.Count > 0)
                {
                    // Assuming we're dealing with the first user record
                    var currentUserRecord = userRecords[0];
                    greetingLabel.Text = $"Hi {currentUserRecord.Name}"; // Display the greeting with the user's name
                    outputResult.Text = currentUserRecord.BMIResult.ToString(); // Display the BMI result

                    // Display the BMI status and set the text color based on its value
                    outputBmiStatus.Text = currentUserRecord.BMIStatus;
                    switch (currentUserRecord.BMIStatus)
                    {
                        case "Normal":
                            outputBmiStatus.TextColor = Colors.Green;
                            break;
                        case "Underweight":
                            outputBmiStatus.TextColor = Colors.Yellow;
                            break;
                        case "Overweight":
                            outputBmiStatus.TextColor = Colors.Orange;
                            break;
                        case "Obese":
                            outputBmiStatus.TextColor = Colors.Red;
                            break;
                        default:
                            outputBmiStatus.TextColor = Colors.Black;
                            break;
                    }
                }
                else
                {
                    // If no user data is available, show default values
                    greetingLabel.Text = "Hi there!";
                    outputResult.Text = "0.00";
                    outputBmiStatus.Text = "N/A";
                    outputBmiStatus.TextColor = Colors.Black; // Set default color
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }



    }
}

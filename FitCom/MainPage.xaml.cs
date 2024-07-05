using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace FitCom
{
    public partial class MainPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        UserRecord currentUserRecord; // Store the current user record

        Label labelNoGoalSelected;
        Button buttonSelectGoal;

        public MainPage()
        {
            InitializeComponent();
            DisplayNoGoalSelectedMessage(); // Display the "Select a Goal" button by default
            Task.Run(async () => await firebaseHelper.PredefinedExercises());
            LoadUserExercises(); // Load user exercises when page is created
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserExercises(); // Load user exercises when page appears
        }

        async void LoadUserExercises()
        {
            await CheckAndLoadUserExercises();
        }

        async Task CheckAndLoadUserExercises()
        {
            var userRecords = await firebaseHelper.GetAllUserRecords();
            if (userRecords != null && userRecords.Count > 0)
            {
                // Assuming only one user record is present
                currentUserRecord = userRecords[0];
                var exercises = await LoadExercisesForUser(currentUserRecord); // Load exercises for the current user
                DisplayUserExercises(exercises); // Display the loaded exercises
                RemoveNoGoalSelectedMessage(); // Remove the message and button if they exist
            }
            else
            {
                ClearDisplayedExercises(); // Clear the displayed exercises
                DisplayNoGoalSelectedMessage(); // Display the "Select a Goal" button
            }
        }

        async Task<List<ExerciseRecord>> LoadExercisesForUser(UserRecord userRecord)
        {
            var exercises = new List<ExerciseRecord>();
            foreach (var exerciseId in userRecord.ExerciseIds)
            {
                var exercise = await firebaseHelper.GetExerciseRecordById(exerciseId);
                exercises.Add(exercise);
            }
            return exercises;
        }

        void DisplayUserExercises(List<ExerciseRecord> exercises)
        {
            mainLayout.Children.Clear(); // Clear the mainLayout before adding new elements

            for (int index = 0; index < exercises.Count; index++)
            {
                var exercise = exercises[index]; // Access the exercise using the index

                var swipeView = new SwipeView
                {
                    HeightRequest = 110,
                    Margin = new Thickness(0, 5),
                    BackgroundColor = Color.FromRgba(0, 0, 0, 0),
                };

                var frame = new Frame
                {
                    Padding = new Thickness(8),
                    BackgroundColor = Color.FromHex("#E8D6B9"),
                    CornerRadius = 10,
                };

                var exerciseNameLabel = new Label
                {
                    Text = exercise.ExerciseName,
                    FontSize = 20,
                    FontFamily = "PoppinsSemibold",
                    TextColor = Color.FromHex("#411400"),
                };

                var shortDescriptionLabel = new Label
                {
                    Text = exercise.ExerciseInfo, // Use ExerciseInfo for short description
                    FontSize = 15,
                    TextColor = Color.FromHex("#000000"), // You can adjust the color as needed
                };

                var stackLayout = new StackLayout();
                stackLayout.Children.Add(exerciseNameLabel);
                stackLayout.Children.Add(shortDescriptionLabel);

                frame.Content = stackLayout;

                swipeView.Content = frame;

                mainLayout.Children.Add(swipeView);
            }
        }

        void ClearDisplayedExercises()
        {
            mainLayout.Children.Clear();
        }

        void DisplayNoGoalSelectedMessage()
        {
            labelNoGoalSelected = new Label
            {
                Text = "You do not currently have a goal selected",
                TextColor = Color.FromHex("#FD4F00"),
                FontFamily = "PoppinsRegular",
                FontSize = 15,
                HorizontalTextAlignment = TextAlignment.Start
            };
            mainLayout.Children.Add(labelNoGoalSelected);

            buttonSelectGoal = new Button
            {
                Text = "Select a Goal",
                ImageSource = "add.svg",
                ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Bottom, 20),
                HeightRequest = 180,
                WidthRequest = 200,
                FontSize = 19,
                FontFamily = "PoppinsSemibold",
                TextColor = Color.FromHex("#411400"),
                BackgroundColor = Color.FromHex("#905C35"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(20),
                Margin = new Thickness(0, 135, 0, 0)
            };
            buttonSelectGoal.Clicked += OnSelectGoalButtonClicked;
            mainLayout.Children.Add(buttonSelectGoal);
        }

        void RemoveNoGoalSelectedMessage()
        {
            // Remove the message and button if they exist
            if (labelNoGoalSelected != null)
            {
                mainLayout.Children.Remove(labelNoGoalSelected);
                labelNoGoalSelected = null;
            }

            if (buttonSelectGoal != null)
            {
                mainLayout.Children.Remove(buttonSelectGoal);
                buttonSelectGoal = null;
            }
        }

        async void OnSelectGoalButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }

    }
}

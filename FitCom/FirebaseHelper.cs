using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

namespace FitCom
{
    internal class FirebaseHelper
    {
        FirebaseClient firebase = new FirebaseClient("https://fitcom-42374-default-rtdb.asia-southeast1.firebasedatabase.app/");

        public async Task AddUserRecord(string name, string goal, double weight, double height, double bmi, string bmiStatus)
        {
            // Fetch exercise IDs corresponding to the selected goal type
            var exerciseIds = await GetExerciseIdsByGoalType(goal);
            // Add user record to database
            await firebase
                .Child("UserRecords")
                .PostAsync(new UserRecord()
                {
                    Name = name,
                    Goal = goal,
                    Weight = weight,
                    Height = height,
                    BMIResult = bmi,
                    BMIStatus = bmiStatus,
                    ExerciseIds = exerciseIds // Assign the list of exercise IDs
                });
        }

        private async Task<List<string>> GetExerciseIdsByGoalType(string goalType)
        {
            var exerciseIds = new List<string>();

            // Query the ExerciseRecords node to get exercise IDs for the specified goal type
            var exercises = await firebase
                .Child("ExerciseRecords")
                .OrderBy("GoalType")
                .EqualTo(goalType)
                .OnceAsync<ExerciseRecord>();

            // Add exercise IDs to the list
            foreach (var exercise in exercises)
            {
                exerciseIds.Add(exercise.Key);
            }

            return exerciseIds;
        }

        public async Task PredefinedExercises()
        {
            // Check if predefined exercises already exist in the database
            var existingExercises = await firebase.Child("ExerciseRecords").OnceAsync<ExerciseRecord>();
            if (existingExercises.Any())
            {
                // Predefined exercises already exist, do not add again
                return;
            }

            // Predefined exercises for gaining weight
            await AddExerciseRecord("Squats", "Gain Weight", "Full-body compound exercise targeting the lower body.");
            await AddExerciseRecord("Deadlifts", "Gain Weight", "Compound exercise focusing on the posterior chain, including the lower back, glutes, and hamstrings.");
            await AddExerciseRecord("Bench Press", "Gain Weight", "Upper body strength exercise targeting the chest, shoulders, and triceps.");
            await AddExerciseRecord("Barbell Rows", "Gain Weight", "Compound movement for the back muscles, particularly targeting the lats and rhomboids.");

            // Predefined exercises for losing weight
            await AddExerciseRecord("Running", "Lose Weight", "Cardiovascular exercise that burns calories and improves cardiovascular health.");
            await AddExerciseRecord("Cycling", "Lose Weight", "Low-impact aerobic exercise great for burning calories and improving leg strength.");
            await AddExerciseRecord("Jump Rope", "Lose Weight", "High-intensity cardio exercise that strengthens muscles and improves coordination.");
            await AddExerciseRecord("Swimming", "Lose Weight", "Full-body workout that burns calories and builds endurance.");

            // Predefined exercises for gaining muscle
            await AddExerciseRecord("Pull-Ups", "Gain Muscle", "Compound exercise for the upper body, particularly targeting the back and arms.");
            await AddExerciseRecord("Push-Ups", "Gain Muscle", "Bodyweight exercise targeting the chest, shoulders, and triceps.");
            await AddExerciseRecord("Dumbbell Curls", "Gain Muscle", "Isolation exercise for the biceps, improving arm strength and size.");
            await AddExerciseRecord("Leg Press", "Gain Muscle", "Compound exercise targeting the lower body, particularly the quadriceps, hamstrings, and glutes.");

        }

        private async Task AddExerciseRecord(string exerciseName, string goalType, string exerciseInfo)
        {
            await firebase
                .Child("ExerciseRecords")
                .PostAsync(new ExerciseRecord
                {
                    ExerciseName = exerciseName,
                    GoalType = goalType,
                    ExerciseInfo = exerciseInfo
                });
        }

        public async Task<UserRecord> GetUserRecordById(string userRecordId)
        {
            var userRecord = await firebase
                .Child("UserRecords")
                .Child(userRecordId) // Use the unique key as the child key
                .OnceSingleAsync<UserRecord>();

            return userRecord;
        }
        public async Task<List<UserRecord>> GetAllUserRecords()
        {
            var userRecords = await firebase
                .Child("UserRecords")
                .OnceAsync<UserRecord>();

            return userRecords?.Select(x => x.Object).ToList();
        }

        public async Task<ExerciseRecord> GetExerciseRecordById(string exerciseId)
        {
            var exerciseRecord = await firebase
                .Child("ExerciseRecords")
                .Child(exerciseId)
                .OnceSingleAsync<ExerciseRecord>();

            return exerciseRecord;
        }

        public async Task UpdateUserRecord(UserRecord userRecord)
        {
            // Find the user record by name
            var userRecords = await firebase
                .Child("UserRecords")
                .OrderByKey()
                .EqualTo(userRecord.Name)  // Assuming 'Name' is the unique identifier
                .OnceAsync<UserRecord>();

            if (userRecords.Any())
            {
                // Update the first user record found with the new data
                var userRecordToUpdate = userRecords.First();
                await firebase
                    .Child("UserRecords")
                    .Child(userRecordToUpdate.Key)
                    .PutAsync(userRecord);
            }
            else
            {
                // Handle case where user record is not found
                Console.WriteLine("User record not found.");
            }
        }

        public async Task ClearUserRecords()
        {
            // Clear user records from the Firebase database
            await firebase.Child("UserRecords").DeleteAsync();
        }


        public async Task<string> FindExerciseIdByName(string exerciseName)
        {
            var exerciseRecords = await firebase
                .Child("ExerciseRecords")
                .OrderBy("ExerciseName")
                .EqualTo(exerciseName)
                .OnceSingleAsync<Dictionary<string, ExerciseRecord>>();

            if (exerciseRecords != null && exerciseRecords.Any())
            {
                // Return the key of the first exercise record found
                return exerciseRecords.Keys.FirstOrDefault();
            }
            else
            {
                // Return null if exercise name is not found
                return null;
            }
        }
    }
}

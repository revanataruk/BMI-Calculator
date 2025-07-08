using BMI_Calculator.Models;
using System;
using System.Collections.Generic;

namespace BMI_Calculator.Controllers
{
    public interface IMainView
    {
        string WeightInput { get; }
        string HeightInput { get; }
        string AgeInput { get; }
        string GenderInput { get; }
        string EmailInput { get; }
        string PasswordInput { get; }
        string CalorieFoodNameInput { get; }
        int CalorieValueInput { get; }
        string CalorieMealTypeInput { get; }
        DateTime CalorieDateInput { get; }
        void ShowBmiResult(string bmiText, string bodyFatText, string dietRecommendation, string exerciseRecommendation);
        void ShowLoginSuccess(string email);
        void ShowLoginFailure(string message);
        void ShowRegistrationResult(bool success, string message);
        void ClearBmiInputs();
        void ClearCalorieInputs();
        void SwitchPanel(string panelName);
        void ShowMessage(string message, string title, bool isError);
        void UpdateCalorieList(List<CalorieRecord> records, int totalCalories);
        void UpdateCalorieChart(Dictionary<string, int> dailySummary);
        string PromptUser(string prompt, string title, string defaultValue = "");
        void UpdateTargetCaloriesDisplay(string targetText);
        void UpdateBmiDisplay(string bmiText, string bodyFatText);
        void UpdateSuggestions(string dietSuggestion, string workoutSuggestion);
        void UpdateCalorieStatus(string statusText);
    }
}
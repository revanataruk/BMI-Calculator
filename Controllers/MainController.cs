using BMI_Calculator.Models;
using System;

namespace BMI_Calculator.Controllers
{
    public class MainController
    {
        private readonly IMainView _view;
        private readonly UserRepository _userRepository;
        private readonly BmiRecordRepository _bmiRecordRepository;
        private readonly CalorieTrackingRepository _calorieRepository; // Ditambahkan
        private readonly BmiCalculatorService _bmiCalculatorService;
        private string _loggedInEmail;
        private bool IsLoggedIn => !string.IsNullOrEmpty(_loggedInEmail);
        private BmiResult _latestBmiResult = null;
        public MainController(IMainView view)
        {
            _view = view;
            _userRepository = new UserRepository();
            _bmiRecordRepository = new BmiRecordRepository();
            _calorieRepository = new CalorieTrackingRepository(); // Ditambahkan
            _bmiCalculatorService = new BmiCalculatorService();
        }

        public void Initialize()
        {
            _view.SwitchPanel("panel1");
        }

        public void NavigateTo(string panelName)
        {
            if (panelName == "panel2" || panelName == "panel3" || panelName == "panel4" || panelName == "panel6")
            {
                if (!IsLoggedIn)
                {
                    _view.SwitchPanel("panel5");
                    return;
                }
            }
            if (panelName == "panel2")
            {
                LoadCalorieDataForDate();
                string targetText = "Target: - Cal"; // Teks default jika belum pernah hitung
                if (_latestBmiResult != null)
                {
                    targetText = $"Target: {_latestBmiResult.BmrValue:F0} Cal";
                }
                _view.UpdateTargetCaloriesDisplay(targetText);
            }
            if (panelName == "panel6")
            {
                ShowCalorieChart();
            }

            _view.SwitchPanel(panelName);
        }

        public void CalculateBmi()
        {
            try
            {
                double weight = double.Parse(_view.WeightInput);
                double height = double.Parse(_view.HeightInput);
                int age = int.Parse(_view.AgeInput);
                string gender = _view.GenderInput;
                var result = _bmiCalculatorService.Calculate(weight, height, age, gender);
                _latestBmiResult = result;
                if (result != null)
                {
                    _view.ShowBmiResult(
                        $"BMI : {result.BmiValue:F2} ({result.Category})",
                        $"Body Fat: {result.BodyFatValue:F2}%",
                        result.DietRecommendation,
                        result.ExerciseRecommendation);
                    if (IsLoggedIn)
                    {
                        var record = new BmiRecord
                        {
                            Date = DateTime.Now,
                            Weight = (float)weight,
                            Height = (float)height,
                            BMI = (float)result.BmiValue,
                            BodyFat = (float)result.BodyFatValue,
                            BMR = (float)result.BmrValue,
                            UserEmail = _loggedInEmail
                        };
                        _bmiRecordRepository.SaveBmiRecord(record);
                        _view.ShowMessage("Data successfully saved to database!", "Success", isError: false);
                    }
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Input a valid number. Error: " + ex.Message, "Input Error", isError: true);
            }
        }

        public void Login()
        {
            if (string.IsNullOrEmpty(_view.EmailInput) || string.IsNullOrEmpty(_view.PasswordInput))
            {
                _view.ShowLoginFailure("Please enter both email and password");
                return;            }

            if (_userRepository.AuthenticateUser(_view.EmailInput, _view.PasswordInput))
            {
                _loggedInEmail = _view.EmailInput;
                _view.ShowLoginSuccess(_loggedInEmail);
                var latestRecord = _bmiRecordRepository.GetLatestBmiRecord(_loggedInEmail);
                if (latestRecord != null)
                {
                    _latestBmiResult = new BmiResult { BmrValue = latestRecord.BMR };
                    string lastBmiText = $"BMI : {latestRecord.BMI:F2}";
                    string lastBodyFatText = $"Body Fat: {latestRecord.BodyFat:F2}%";
                    _view.UpdateBmiDisplay(lastBmiText, lastBodyFatText);
                    var lastResult = new BmiResult
                    {
                        BmiValue = latestRecord.BMI,
                        BodyFatValue = latestRecord.BodyFat,
                        BmrValue = latestRecord.BMR
                    };
                    var recommendations = _bmiCalculatorService.GetRecommendationsFromResult(lastResult);
                    _view.UpdateSuggestions(recommendations.DietRecommendation, recommendations.ExerciseRecommendation);
                }
                _view.SwitchPanel("panel1");
            }
            else
            {
                _view.ShowLoginFailure("Invalid email or password.");
            }
        }

        public void Register()
        {
            if (string.IsNullOrEmpty(_view.EmailInput) || string.IsNullOrEmpty(_view.PasswordInput))
            {
                _view.ShowRegistrationResult(false, "Please enter both email and password");
                return;
            }
            bool success = _userRepository.RegisterUser(_view.EmailInput, _view.PasswordInput);
            if (success)
            {
                _view.ShowRegistrationResult(true, "Registration successful! You can now login.");
            }
        }

        public void AddCalorieEntry()
        {
            if (string.IsNullOrEmpty(_view.CalorieMealTypeInput))
            {
                _view.ShowMessage("Please select a meal type.", "Validation Error", isError: true);
                return;
            }
            if (string.IsNullOrWhiteSpace(_view.CalorieFoodNameInput))
            {
                _view.ShowMessage("Please enter a food name.", "Validation Error", isError: true);
                return;
            }
            if (_view.CalorieValueInput <= 0)
            {
                _view.ShowMessage("Calories must be greater than 0.", "Validation Error", isError: true);
                return;
            }
            var newRecord = new CalorieRecord
            {
                Date = _view.CalorieDateInput.Date,
                MealType = _view.CalorieMealTypeInput,
                FoodName = _view.CalorieFoodNameInput,
                Calories = _view.CalorieValueInput,
                UserEmail = _loggedInEmail,
                BMI = 0 
            };
            _calorieRepository.SaveCalorieRecord(newRecord);
            _view.ShowMessage("Calorie entry saved successfully!", "Success", isError: false);
            _view.ClearCalorieInputs();
            LoadCalorieDataForDate(); 
        }

        public void LoadCalorieDataForDate()
        {
            if (IsLoggedIn)
            {
                DateTime selectedDate = _view.CalorieDateInput;
                var records = _calorieRepository.LoadCalorieDataForDate(_loggedInEmail, selectedDate);
                int consumedCalories = _calorieRepository.GetTotalCaloriesForDate(_loggedInEmail, selectedDate);
                _view.UpdateCalorieList(records, consumedCalories);
                if (_latestBmiResult != null && _latestBmiResult.BmrValue > 0)
                {
                    double targetCalories = _latestBmiResult.BmrValue;
                    double difference = consumedCalories - targetCalories;
                    string statusText;
                    if (Math.Abs(difference) <= 50)
                    {
                        statusText = "Achieved";
                    }
                    else if (difference > 50)
                    {
                        statusText = $"Surplus +{difference:F0} Cal";
                    }
                    else 
                    {
                        statusText = $"Deficit -{Math.Abs(difference):F0} Cal";
                    }
                    _view.UpdateCalorieStatus(statusText);
                }
                else
                {
                    _view.UpdateCalorieStatus("Status: N/A");
                }
            }
        }

        public void ChangeEmail()
        {
            if (!IsLoggedIn) return;
            string newEmail = _view.PromptUser("Enter your new email address:", "Change Email", _loggedInEmail);
            if (string.IsNullOrWhiteSpace(newEmail) || newEmail == _loggedInEmail) return;
            if (_userRepository.EmailExistsForOtherUser(newEmail, _loggedInEmail))
            {
                _view.ShowMessage("This email address is already in use by another account.", "Email Exists", isError: true);
                return;
            }
            string password = _view.PromptUser("To confirm, please enter your current password:", "Confirm Password");
            if (string.IsNullOrWhiteSpace(password) || !_userRepository.VerifyCurrentPassword(_loggedInEmail, password))
            {
                _view.ShowMessage("The password you entered is incorrect.", "Verification Failed", isError: true);
                return;
            }
            if (_userRepository.UpdateUserEmail(_loggedInEmail, newEmail))
            {
                _view.ShowMessage("Your email has been updated successfully.", "Success", isError: false);
                _loggedInEmail = newEmail; 
                _view.ShowLoginSuccess(_loggedInEmail); 
            }
            else
            {
                _view.ShowMessage("An error occurred while updating your email.", "Update Failed", isError: true);
            }
        }

        public void ChangePassword()
        {
            if (!IsLoggedIn) return;
            string currentPassword = _view.PromptUser("Enter your CURRENT password:", "Verify Password");
            if (string.IsNullOrWhiteSpace(currentPassword) || !_userRepository.VerifyCurrentPassword(_loggedInEmail, currentPassword))
            {
                _view.ShowMessage("The current password you entered is incorrect.", "Verification Failed", isError: true);
                return;
            }
            string newPassword = _view.PromptUser("Enter your NEW password:", "New Password");
            if (string.IsNullOrWhiteSpace(newPassword)) return;
            string confirmPassword = _view.PromptUser("CONFIRM your new password:", "Confirm New Password");
            if (newPassword != confirmPassword)
            {
                _view.ShowMessage("The new passwords do not match. Please try again.", "Password Mismatch", isError: true);
                return;
            }
            if (_userRepository.UpdateUserPassword(_loggedInEmail, newPassword))
            {
                _view.ShowMessage("Your password has been updated successfully.", "Success", isError: false);
            }
            else
            {
                _view.ShowMessage("An error occurred while updating your password.", "Update Failed", isError: true);
            }
        }

        public void Logout()
        {
            _loggedInEmail = null;
            _view.ShowMessage("You have been logged out successfully.", "Logout", isError: false);
            _view.SwitchPanel("panel5");
        }

        public void ShowCalorieChart()
        {
            if (IsLoggedIn)
            {
                var summary = _calorieRepository.GetCalorieSummaryForLast3Days(_loggedInEmail);
                _view.UpdateCalorieChart(summary);
            }
        }

    }
}
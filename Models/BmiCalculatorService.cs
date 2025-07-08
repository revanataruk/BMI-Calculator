using System;

namespace BMI_Calculator.Models
{
    public class BmiCalculatorService
    {
        public BmiResult Calculate(double weight, double heightCm, int age, string gender)
        {
            if (heightCm <= 0 || weight <= 0)
                return null;
            double heightM = heightCm / 100.0;
            double bmi = weight / (heightM * heightM);
                        string category;
            if (bmi < 18.5) category = "Underweight";
            else if (bmi < 25) category = "Normal";
            else if (bmi < 30) category = "Overweight";
            else category = "Obesity";
            double bodyFat = (gender.ToLower() == "man")
                ? (1.20 * bmi) + (0.23 * age) - 16.2
                : (1.20 * bmi) + (0.23 * age) - 5.4;
            double bmr = (gender.ToLower() == "man")
                ? (10 * weight) + (6.25 * heightCm) - (5 * age) + 5
                : (10 * weight) + (6.25 * heightCm) - (5 * age) - 161;
            var result = new BmiResult
            {
                BmiValue = bmi,
                BodyFatValue = bodyFat,
                BmrValue = bmr,
                Category = category
            };
            SetRecommendations(result);
            return result;
        }
        public BmiResult GetRecommendationsFromResult(BmiResult result)
        {
            if (result.BmiValue < 18.5) result.Category = "Underweight";
            else if (result.BmiValue < 25) result.Category = "Normal";
            else if (result.BmiValue < 30) result.Category = "Overweight";
            else result.Category = "Obesity";
            SetRecommendations(result);
            return result;
        }

        private void SetRecommendations(BmiResult result)
        {
            string bmrInfo = $"Your estimated daily calorie needs (BMR) is {result.BmrValue:F0} calories.\n\n";

            switch (result.Category)
            {
                case "Underweight":
                    result.DietRecommendation = bmrInfo + "Dietary Recommendations:\n• Increase caloric intake by 300-500 calories above your BMR.\n• Focus on nutrient-dense foods, including lean proteins, complex carbs, and healthy fats.";
                    result.ExerciseRecommendation = "Exercise Program:\n• Focus on resistance training 3-4 times a week to build muscle mass.\n• Include compound movements like squats, deadlifts, and bench presses.";
                    break;
                case "Normal":
                    result.DietRecommendation = bmrInfo + "Dietary Recommendations:\n• Maintain your current caloric intake to match your BMR.\n• Ensure a balanced diet with a good mix of macronutrients (protein, carbs, fat).";
                    result.ExerciseRecommendation = "Exercise Program:\n• A combination of cardiovascular exercise (30 mins, 3-5 times/week) and strength training (2-3 times/week) is ideal.";
                    break;
                case "Overweight":
                    result.DietRecommendation = bmrInfo + "Dietary Recommendations:\n• Create a calorie deficit by eating 300-500 calories below your BMR.\n• Increase fiber intake with fruits and vegetables to feel full longer. Limit processed foods and sugary drinks.";
                    result.ExerciseRecommendation = "Exercise Program:\n• Increase cardiovascular activity to 4-5 times a week (e.g., brisk walking, jogging, cycling).\n• Incorporate full-body strength training to boost metabolism.";
                    break;
                case "Obesity":
                    result.DietRecommendation = bmrInfo + "Dietary Recommendations:\n• Aim for a sustainable calorie deficit of 500-750 calories below your BMR.\n• Prioritize whole foods, lean protein, and high-fiber vegetables. Consult a nutritionist for a personalized plan.";
                    result.ExerciseRecommendation = "Exercise Program:\n• Start with low-impact cardio like swimming or walking, 5-6 times a week.\n• Gradually introduce strength training. Consistency is key. Consult a professional before starting a new regimen.";
                    break;
                default:
                    result.DietRecommendation = "Data unavailable.";
                    result.ExerciseRecommendation = "Data unavailable.";
                    break;
            }
        }
    }
}
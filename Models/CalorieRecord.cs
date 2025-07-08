using System;

namespace BMI_Calculator.Models
{
    public class CalorieRecord
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string MealType { get; set; }
        public string FoodName { get; set; }
        public int Calories { get; set; }
        public string UserEmail { get; set; }
        public float BMI { get; set; }
    }
}
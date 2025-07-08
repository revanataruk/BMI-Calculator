namespace BMI_Calculator.Models
{
    public class BmiResult
    {
        public double BmiValue { get; set; }
        public double BodyFatValue { get; set; }
        public double BmrValue { get; set; }
        public string Category { get; set; }
        public string DietRecommendation { get; set; }
        public string ExerciseRecommendation { get; set; }
    }
}
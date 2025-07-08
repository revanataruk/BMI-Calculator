using System;

namespace BMI_Calculator.Models
{
    public class BmiRecord
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public float BMI { get; set; }
        public float BodyFat { get; set; }
        public float BMR { get; set; }
        public string UserEmail { get; set; }
    }
}
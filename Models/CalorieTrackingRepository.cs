using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BMI_Calculator.Models
{
    public class CalorieTrackingRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=bmi_calculator;Uid=root;Pwd=;";
        public void SaveCalorieRecord(CalorieRecord record)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "INSERT INTO calorie_tracking (Date, MealType, FoodName, Calories, UserEmail, BMI) VALUES (@Date, @MealType, @FoodName, @Calories, @UserEmail, @BMI)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Date", record.Date.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@MealType", record.MealType);
                        cmd.Parameters.AddWithValue("@FoodName", record.FoodName);
                        cmd.Parameters.AddWithValue("@Calories", record.Calories);
                        cmd.Parameters.AddWithValue("@UserEmail", record.UserEmail);
                        cmd.Parameters.AddWithValue("@BMI", record.BMI);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public List<CalorieRecord> LoadCalorieDataForDate(string userEmail, DateTime selectedDate)
        {
            var records = new List<CalorieRecord>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "SELECT ID, MealType, FoodName, Calories, CreatedAt FROM calorie_tracking WHERE UserEmail = @UserEmail AND Date = @Date ORDER BY CreatedAt";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                        cmd.Parameters.AddWithValue("@Date", selectedDate.ToString("yyyy-MM-dd"));
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                records.Add(new CalorieRecord
                                {
                                    ID = reader.GetInt32("ID"),
                                    MealType = reader.GetString("MealType"),
                                    FoodName = reader.GetString("FoodName"),
                                    Calories = reader.GetInt32("Calories"),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading calorie data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return records;
        }

        public int GetTodayTotalCalories(string userEmail)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "SELECT COALESCE(SUM(Calories), 0) FROM calorie_tracking WHERE UserEmail = @UserEmail AND Date = @Date";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd"));
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public int GetTotalCaloriesForDate(string userEmail, DateTime selectedDate)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "SELECT COALESCE(SUM(Calories), 0) FROM calorie_tracking WHERE UserEmail = @UserEmail AND Date = @Date";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                        cmd.Parameters.AddWithValue("@Date", selectedDate.ToString("yyyy-MM-dd"));
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public Dictionary<string, int> GetCalorieSummaryForLast3Days(string userEmail)
        {
            var summary = new Dictionary<string, int>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    for (int i = 2; i >= 0; i--)
                    {
                        DateTime date = DateTime.Now.AddDays(-i);
                        string dateStr = date.ToString("yyyy-MM-dd");
                        string dateLabel = date.ToString("dd/MM");
                        var query = "SELECT COALESCE(SUM(Calories), 0) FROM calorie_tracking WHERE UserEmail = @UserEmail AND Date = @Date";
                        using (var cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                            cmd.Parameters.AddWithValue("@Date", dateStr);
                            int totalCalories = Convert.ToInt32(cmd.ExecuteScalar());
                            summary[dateLabel] = totalCalories;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return summary;
        }
    }
}
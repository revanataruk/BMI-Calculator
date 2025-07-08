using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace BMI_Calculator.Models
{
    public class BmiRecordRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=bmi_calculator;Uid=root;Pwd=;";

        public void SaveBmiRecord(BmiRecord record)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO bmi_records (Date, Weight, Height, BMI, BodyFat, BMR, UserEmail) VALUES (@Date, @Weight, @Height, @BMI, @BodyFat, @BMR, @UserEmail)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Date", record.Date);
                        cmd.Parameters.AddWithValue("@Weight", record.Weight);
                        cmd.Parameters.AddWithValue("@Height", record.Height);
                        cmd.Parameters.AddWithValue("@BMI", record.BMI);
                        cmd.Parameters.AddWithValue("@BodyFat", record.BodyFat);
                        cmd.Parameters.AddWithValue("@BMR", record.BMR); 
                        cmd.Parameters.AddWithValue("@UserEmail", record.UserEmail);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public BmiRecord GetLatestBmiRecord(string userEmail)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "SELECT * FROM bmi_records WHERE UserEmail = @UserEmail ORDER BY Date DESC LIMIT 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new BmiRecord
                                {
                                    ID = reader.GetInt32("ID"),
                                    Date = reader.GetDateTime("Date"),
                                    Weight = reader.GetFloat("Weight"),
                                    Height = reader.GetFloat("Height"),
                                    BMI = reader.GetFloat("BMI"),
                                    BodyFat = reader.GetFloat("BodyFat"),
                                    BMR = reader.IsDBNull(reader.GetOrdinal("BMR")) ? 0 : reader.GetFloat("BMR"),
                                    UserEmail = reader.GetString("UserEmail")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading latest BMI data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null; 
        }
    }
}
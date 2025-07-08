using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace BMI_Calculator.Models
{
    public class UserRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=bmi_calculator;Uid=root;Pwd=;";
        public bool AuthenticateUser(string email, string password)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "SELECT * FROM users WHERE Email = @Email AND Password = @Password";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        using (var reader = cmd.ExecuteReader())
                        {
                            return reader.HasRows;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Authentication Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool RegisterUser(string email, string password)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var checkQuery = "SELECT COUNT(*) FROM users WHERE Email = @Email";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Email already registered.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    var insertQuery = "INSERT INTO users (Email, Password) VALUES (@Email, @Password)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool VerifyCurrentPassword(string email, string password)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "SELECT COUNT(*) FROM users WHERE Email = @Email AND Password = @Password";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateUserPassword(string email, string newPassword)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "UPDATE users SET Password = @NewPassword WHERE Email = @Email";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                        cmd.Parameters.AddWithValue("@Email", email);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool EmailExistsForOtherUser(string newEmail, string currentEmail)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE Email = @Email AND Email != @CurrentEmail";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", newEmail);
                        cmd.Parameters.AddWithValue("@CurrentEmail", currentEmail);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true; 
            }
        }

        public bool UpdateUserEmail(string oldEmail, string newEmail)
        {
            MySqlConnection conn = null;
            MySqlTransaction transaction = null;
            try
            {
                conn = new MySqlConnection(_connectionString);
                conn.Open();
                transaction = conn.BeginTransaction();
                var userQuery = "UPDATE users SET Email = @NewEmail WHERE Email = @OldEmail";
                using (var userCmd = new MySqlCommand(userQuery, conn, transaction))
                {
                    userCmd.Parameters.AddWithValue("@NewEmail", newEmail);
                    userCmd.Parameters.AddWithValue("@OldEmail", oldEmail);
                    userCmd.ExecuteNonQuery();
                }
                var bmiQuery = "UPDATE bmi_records SET UserEmail = @NewEmail WHERE UserEmail = @OldEmail";
                using (var bmiCmd = new MySqlCommand(bmiQuery, conn, transaction))
                {
                    bmiCmd.Parameters.AddWithValue("@NewEmail", newEmail);
                    bmiCmd.Parameters.AddWithValue("@OldEmail", oldEmail);
                    bmiCmd.ExecuteNonQuery();
                }
                var calorieQuery = "UPDATE calorie_tracking SET UserEmail = @NewEmail WHERE UserEmail = @OldEmail";
                using (var calorieCmd = new MySqlCommand(calorieQuery, conn, transaction))
                {
                    calorieCmd.Parameters.AddWithValue("@NewEmail", newEmail);
                    calorieCmd.Parameters.AddWithValue("@OldEmail", oldEmail);
                    calorieCmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Database Error during email update: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                conn?.Close();
            }
        }
    }
}
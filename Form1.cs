using BMI_Calculator.Controllers;
using BMI_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BMI_Calculator
{
    public partial class Form1 : Form, IMainView
    {
        private MainController _controller;

        public Form1()
        {
            InitializeComponent();
            if (!this.DesignMode)
            {
                _controller = new MainController(this);
            }
        }

        #region Implementasi IMainView
        public string WeightInput => textBox2.Text;
        public string HeightInput => textBox1.Text;
        public string AgeInput => textBox3.Text;
        public string GenderInput => comboBox1.SelectedItem?.ToString() ?? "Man";
        public string EmailInput => textBox4.Text.Trim();
        public string PasswordInput => textBox5.Text;
        public void UpdateTargetCaloriesDisplay(string targetText)
        {
            label24.Text = targetText;
            label28.Text = targetText;
        }
        public void UpdateBmiDisplay(string bmiText, string bodyFatText)
        {
            label5.Text = bmiText;
            label6.Text = bodyFatText;
        }
        public void UpdateSuggestions(string dietSuggestion, string workoutSuggestion)
        {
            label8.Text = dietSuggestion;
            label15.Text = workoutSuggestion;
        }
        public void UpdateCalorieStatus(string statusText)
        {
            label27.Text = statusText;
        }
        public string CalorieFoodNameInput => textBox6.Text;
        public int CalorieValueInput => (int)numericUpDown1.Value;
        public string CalorieMealTypeInput => comboBox2.SelectedItem?.ToString();
        public DateTime CalorieDateInput => dateTimePicker1.Value;
        public void ShowBmiResult(string bmiText, string bodyFatText, string dietRecommendation, string exerciseRecommendation)
        {
            label5.Text = bmiText;
            label16.Text = bmiText;
            label6.Text = bodyFatText;
            label8.Text = dietRecommendation;
            label15.Text = exerciseRecommendation;
        }

        public void ClearBmiInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            label5.Text = "BMI";
            label6.Text = "Body Fat";
        }

        public void ClearCalorieInputs()
        {
            comboBox2.SelectedIndex = -1;
            textBox6.Clear();
            numericUpDown1.Value = 0;
        }

        public void SwitchPanel(string panelName)
        {
            panel1.Visible = (panelName == "panel1");
            panel2.Visible = (panelName == "panel2");
            panel3.Visible = (panelName == "panel3");
            panel4.Visible = (panelName == "panel4");
            panel5.Visible = (panelName == "panel5");
            panel6.Visible = (panelName == "panel6");
        }

        public void ShowLoginSuccess(string email)
        {
            label18.Text = $"Welcome {email}";
        }

        public void ShowLoginFailure(string message)
        {
            MessageBox.Show(message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowRegistrationResult(bool success, string message)
        {
            if (success)
            {
                MessageBox.Show(message, "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox4.Clear();
                textBox5.Clear();
            }
            else
            {
                MessageBox.Show(message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void ShowMessage(string message, string title, bool isError)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        public string PromptUser(string prompt, string title, string defaultValue = "")
        {
            if (!this.DesignMode)
            {
                return Microsoft.VisualBasic.Interaction.InputBox(prompt, title, defaultValue);
            }
            return string.Empty;
        }

        public void UpdateCalorieChart(Dictionary<string, int> dailySummary)
        {
            chart1.Series.Clear();
            var series = new Series("Calories")
            {
                ChartType = SeriesChartType.Column
            };

            foreach (var day in dailySummary)
            {
                series.Points.AddXY(day.Key, day.Value);
            }

            chart1.Series.Add(series);
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.Title = "Calories";
        }

        public void UpdateCalorieList(List<CalorieRecord> records, int totalCalories)
        {
            listView1.Items.Clear();
            foreach (var record in records)
            {
                ListViewItem item = new ListViewItem(record.MealType); // Kolom pertama
                item.SubItems.Add(record.FoodName); // Kolom kedua
                item.SubItems.Add(record.Calories.ToString()); // Kolom ketiga
                listView1.Items.Add(item);
            }
            label26.Text = $"Total Calories Today: {totalCalories}";
        }
        #endregion

        #region Event Handlers (Hanya Mendelegasikan ke Controller)
        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            numericUpDown1.Maximum = 9999;
            comboBox1.Items.Add("Man");
            comboBox1.Items.Add("Woman");
            comboBox1.SelectedIndex = 0;
            textBox5.PasswordChar = '*';
            comboBox2.Items.Add("Breakfast");
            comboBox2.Items.Add("Lunch");
            comboBox2.Items.Add("Dinner");
            comboBox2.Items.Add("Snack");
            listView1.View = View.Details; 
            listView1.GridLines = true;        
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Meal", 80);
            listView1.Columns.Add("Food Name", 80);
            listView1.Columns.Add("Calories", -2);
            _controller.Initialize(); 
        }

        private void button1_Click(object sender, EventArgs e) => _controller.CalculateBmi();
        private void button2_Click(object sender, EventArgs e) => ClearBmiInputs();
        private void button9_Click(object sender, EventArgs e) => _controller.Login();
        private void button13_Click(object sender, EventArgs e) => _controller.Register();
        private void button18_Click(object sender, EventArgs e) => _controller.AddCalorieEntry();
        private void button16_Click(object sender, EventArgs e) => ClearCalorieInputs();
        private void button10_Click(object sender, EventArgs e) => _controller.ChangeEmail();
        private void button11_Click(object sender, EventArgs e) => _controller.ChangePassword();
        private void button12_Click(object sender, EventArgs e) => _controller.Logout();
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) => _controller.LoadCalorieDataForDate();
        private void button5_Click(object sender, EventArgs e) => _controller.NavigateTo("panel1");
        private void button6_Click(object sender, EventArgs e) => _controller.NavigateTo("panel2");
        private void button7_Click(object sender, EventArgs e) => _controller.NavigateTo("panel3");
        private void button8_Click(object sender, EventArgs e) => _controller.NavigateTo("panel4");
        private void button14_Click(object sender, EventArgs e) => _controller.NavigateTo("panel1");
        private void button3_Click_1(object sender, EventArgs e) => _controller.NavigateTo("panel6");
        private void button4_Click_1(object sender, EventArgs e) => _controller.NavigateTo("panel2");
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void panel5_Paint(object sender, PaintEventArgs e) { }
        private void panel4_Paint(object sender, PaintEventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) { }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label8_Click_1(object sender, EventArgs e) { }
        private void label28_Click(object sender, EventArgs e) { }
        private void label27_Click(object sender, EventArgs e) { }
        private void label26_Click(object sender, EventArgs e) { }
        private void label24_Click(object sender, EventArgs e) { }
        private void label22_Click(object sender, EventArgs e) { }
        private void label21_Click(object sender, EventArgs e) { }
        private void label20_Click(object sender, EventArgs e) { }
        private void label19_Click(object sender, EventArgs e) { }
        private void label19_Click_1(object sender, EventArgs e) { }
        private void label17_Click(object sender, EventArgs e) { }
        private void label16_Click(object sender, EventArgs e) { }
        private void label15_Click(object sender, EventArgs e) { }
        private void label14_Click(object sender, EventArgs e) { }
        private void label13_Click(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void chart1_Click_1(object sender, EventArgs e) { }
        private void button15_Click_1(object sender, EventArgs e) { }
        #endregion
    }
}
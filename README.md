# 🧮 BMI Calculator

A Windows Forms application that calculates BMI (Body Mass Index) and body fat percentage, tracks history, visualizes progress with charts and saves data to a database

![App Mockup](https://raw.githubusercontent.com/revanataruk/BMI-Calculator/master/mockup.jpg)

## 🧩 Features

- 🔢 Calculate BMI based on height and weight
-🧍‍♂️ Determine BMI category (Skinny, Normal, Fat, Obesity)
- 💪 Calculate body fat percentage based on BMI, age, and gender
- 🕒 Track history of BMI measurements
- 🧠 Data persistence with SQLite database
- 📊 Visualize BMI progress with interactive charts (LiveCharts)
- 📃 View list of past measurements (with date, BMI, fat %, and category)
- ↩️ Navigation between panels (Calculator ↔ Chart History)
- 🧑‍💻 Simple and intuitive Windows Forms interface
- 💾 Auto-save BMI and body fat data into local database on calculation

## 📚 Technologies Used

- C# (.NET Framework 4.8)
- Windows Forms
- LiveCharts (for data visualization)
- Visual Studio 2022
- SQLite (System.Data.SQLite)

## 📋 Requirements

- Windows OS
- .NET Framework 4.8 or higher
- Visual Studio 2022 (for development)

## 📁 Installation

1. Clone this repository:
   ```
   git clone https://github.com/username/BMI-Calculator.git
   ```

2. Open the solution file (`BMI-Calculator.sln`) in Visual Studio 2022

3. Build the solution (Press F6 or use Build > Build Solution)

4. Run the application (Press F5 or use Debug > Start Debugging)

## 👨‍💻 Usage

1.Enter your height (cm), weight (kg), age
2.Select your gender
3.Click Calculate
4.View your BMI, Body Fat %, and category
5.Your data is automatically saved
6.Click the View History button to see your past entries and chart

## ⚙️ How BMI is Calculated
The application uses the standard BMI formula:
```
BMI = weight(kg) / (height(m))²
```

BMI Categories:
- Below 18.5: Skinny
- 18.5 to 24.9: Normal
- 25 to 29.9: Fat
- 30 and above: Obesity

Body Fat Percentage is estimated using the following formulas:
- For men: (1.20 × BMI) + (0.23 × Age) - 16.2
- For women: (1.20 × BMI) + (0.23 × Age) - 5.4

## 📦 Dependencies

- LiveCharts.WinForms
- LiveCharts.Wpf
- System.Data.SQLite

## 🎯 Future Enhancements

- Add user profiles
- Export data to CSV or Excel
- Additional health metrics
- Diet and exercise recommendations based on BMI
- More detailed progress analytics

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📌 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature-name`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin feature-name`
5. Submit a pull request

## 📌 Notes

- This project was built as part of a learning process and small assignment. for learning purpose.

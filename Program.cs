using System;
using System.Windows.Forms;

namespace BMI_Calculator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Inisialisasi Form (View)
            // Controller akan diinisialisasi di dalam constructor Form1
            Application.Run(new Form1());
        }
    }
}
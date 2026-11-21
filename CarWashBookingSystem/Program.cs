using CarWashBookingSystem.Data;
using CarWashBookingSystem.Views;

namespace CarWashBookingSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.ThreadException += (s, e) =>
                MessageBox.Show(e.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Database.Initialize();
            Application.Run(new LoginForm());
        }
    }
}

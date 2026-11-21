using CarWashBookingSystem.Controllers;

namespace CarWashBookingSystem.Views
{
    public class LoginForm : BaseForm
    {
        private readonly AuthController auth = new AuthController();
        private readonly TextBox txtEmail;
        private readonly TextBox txtPassword;

        public LoginForm() : base("Login", 400, 260)
        {
            AddTitle("Login");
            AddLabel("Email", 30, 70);
            txtEmail = AddTextBox(130, 70);
            AddLabel("Password", 30, 110);
            txtPassword = AddTextBox(130, 110, 220, true);
            var login = AddButton("Login", 130, 155, Login_Click, 105);
            AddButton("Register", 245, 155, Register_Click, 105);
            AcceptButton = login;
        }

        private void Login_Click(object sender, EventArgs e)
        {
            Run(() =>
            {
                var user = auth.Login(txtEmail.Text, txtPassword.Text);
                txtPassword.Clear();
                Form menu = user.Role == "Admin" ? new AdminDashboardForm() : new CustomerMenuForm();
                Hide();
                menu.FormClosed += (s, ev) =>
                {
                    auth.Logout();
                    Show();
                };
                menu.Show();
            });
        }

        private void Register_Click(object sender, EventArgs e)
        {
            OpenChild(new RegisterForm());
        }
    }
}

using CarWashBookingSystem.Controllers;

namespace CarWashBookingSystem.Views
{
    public class RegisterForm : BaseForm
    {
        private readonly AuthController auth = new AuthController();
        private readonly TextBox txtName, txtEmail, txtPhone, txtPassword, txtConfirm, txtPlate;
        private readonly ComboBox cboVehicle;

        public static readonly string[] VehicleTypes = { "Car", "SUV", "Pickup", "Van", "Minibus", "Truck", "Motorcycle" };

        public RegisterForm() : base("Register", 460, 430)
        {
            AddTitle("Create Account");
            AddBackButton();
            AddLabel("Full name", 30, 70);
            txtName = AddTextBox(180, 70, 240);
            AddLabel("Email", 30, 110);
            txtEmail = AddTextBox(180, 110, 240);
            AddLabel("Phone (10 digits)", 30, 150);
            txtPhone = AddTextBox(180, 150, 240);
            AddLabel("Password", 30, 190);
            txtPassword = AddTextBox(180, 190, 240, true);
            AddLabel("Confirm password", 30, 230);
            txtConfirm = AddTextBox(180, 230, 240, true);
            AddLabel("Plate number", 30, 270);
            txtPlate = AddTextBox(180, 270, 240);
            AddLabel("Vehicle type", 30, 310);
            cboVehicle = AddComboBox(180, 310, 240);
            cboVehicle.Items.AddRange(VehicleTypes);
            AddButton("Register", 180, 360, Register_Click, 240);
        }

        private void Register_Click(object sender, EventArgs e)
        {
            if (Run(() => auth.Register(txtName.Text, txtEmail.Text, txtPhone.Text, txtPassword.Text, txtConfirm.Text, txtPlate.Text, cboVehicle.Text)))
            {
                Success("Account created. You can now log in.");
                Close();
            }
        }
    }
}

using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Helpers;

namespace CarWashBookingSystem.Views
{
    public class ProfileForm : BaseForm
    {
        private readonly AuthController auth = new AuthController();
        private readonly TextBox txtPhone, txtPlate, txtCurrent, txtNew, txtConfirm;
        private readonly ComboBox cboVehicle;

        public ProfileForm() : base("My Profile", 460, Session.CurrentUser.Role == "Admin" ? 250 : 470)
        {
            var user = Session.CurrentUser;
            AddTitle("My Profile");
            AddBackButton();
            int y = 70;

            if (user.Role != "Admin")
            {
                AddLabel("Phone", 30, y);
                txtPhone = AddTextBox(190, y, 230);
                txtPhone.Text = user.Phone;
                AddLabel("Plate number", 30, y + 40);
                txtPlate = AddTextBox(190, y + 40, 230);
                txtPlate.Text = user.PlateNumber;
                AddLabel("Vehicle type", 30, y + 80);
                cboVehicle = AddComboBox(190, y + 80, 230);
                cboVehicle.Items.AddRange(RegisterForm.VehicleTypes);
                cboVehicle.SelectedItem = user.VehicleType;
                AddButton("Save Profile", 190, y + 120, SaveProfile_Click, 230);
                y += 180;
            }

            AddLabel("Current password", 30, y);
            txtCurrent = AddTextBox(190, y, 230, true);
            AddLabel("New password", 30, y + 40);
            txtNew = AddTextBox(190, y + 40, 230, true);
            AddLabel("Confirm password", 30, y + 80);
            txtConfirm = AddTextBox(190, y + 80, 230, true);
            AddButton("Change Password", 190, y + 120, ChangePassword_Click, 230);
        }

        private void SaveProfile_Click(object sender, EventArgs e)
        {
            if (Run(() => auth.UpdateProfile(txtPhone.Text, txtPlate.Text, cboVehicle.Text)))
                Success("Your profile has been updated.");
        }

        private void ChangePassword_Click(object sender, EventArgs e)
        {
            if (Run(() => auth.ChangePassword(txtCurrent.Text, txtNew.Text, txtConfirm.Text)))
            {
                Success("Your password has been changed.");
                txtCurrent.Clear();
                txtNew.Clear();
                txtConfirm.Clear();
            }
        }
    }
}

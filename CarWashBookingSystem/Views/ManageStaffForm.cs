using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Views
{
    public class ManageStaffForm : BaseForm
    {
        private readonly StaffController controller = new StaffController();
        private readonly DataGridView grid, gridBookings;
        private readonly TextBox txtName, txtPhone, txtSpecialization;
        private readonly CheckBox chkAvailable;
        private int selectedId;

        public ManageStaffForm() : base("Staff", 900, 620)
        {
            AddTitle("Staff Members");
            AddBackButton();
            grid = AddGrid(20, 60, 540, 300);
            grid.SelectionChanged += Grid_SelectionChanged;

            AddLabel("Full name", 580, 70);
            txtName = AddTextBox(690, 70, 190);
            AddLabel("Phone", 580, 110);
            txtPhone = AddTextBox(690, 110, 190);
            AddLabel("Specialization", 580, 150);
            txtSpecialization = AddTextBox(690, 150, 190);
            chkAvailable = new CheckBox { Text = "Available", Location = new Point(690, 190), AutoSize = true, Checked = true };
            Controls.Add(chkAvailable);

            AddButton("Add", 690, 225, Add_Click, 190);
            AddButton("Update", 690, 265, Update_Click, 190);
            AddButton("Remove", 690, 305, Delete_Click, 190);
            AddButton("Clear", 690, 345, (s, e) => ClearFields(), 190);

            AddLabel("Bookings assigned to the selected staff member", 20, 375);
            gridBookings = AddGrid(20, 405, 860, 195);
            LoadStaff();
        }

        private void LoadStaff()
        {
            Run(() =>
            {
                grid.DataSource = controller.GetAll()
                    .Select(s => new { s.Id, Name = s.FullName, s.Phone, s.Specialization, Available = s.IsAvailable ? "Yes" : "No" })
                    .ToList();
                ClearFields();
            });
        }

        private void ClearFields()
        {
            selectedId = 0;
            txtName.Clear();
            txtPhone.Clear();
            txtSpecialization.Clear();
            chkAvailable.Checked = true;
            grid.ClearSelection();
            gridBookings.DataSource = null;
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null || !grid.CurrentRow.Selected) return;
            int id = Convert.ToInt32(grid.CurrentRow.Cells["Id"].Value);
            var staff = controller.GetAll().FirstOrDefault(s => s.Id == id);
            if (staff == null) return;
            selectedId = staff.Id;
            txtName.Text = staff.FullName;
            txtPhone.Text = staff.Phone;
            txtSpecialization.Text = staff.Specialization;
            chkAvailable.Checked = staff.IsAvailable;
            Run(() => gridBookings.DataSource = controller.GetBookings(staff.Id));
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (Run(() => controller.Add(txtName.Text, txtPhone.Text, txtSpecialization.Text, chkAvailable.Checked)))
            {
                Success("Staff member added.");
                LoadStaff();
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            if (Run(() =>
            {
                if (selectedId == 0) throw new AppException("Please select a staff member first.");
                controller.Update(selectedId, txtName.Text, txtPhone.Text, txtSpecialization.Text, chkAvailable.Checked);
            }))
            {
                Success("Staff member updated.");
                LoadStaff();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Please select a staff member first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Confirm("Are you sure you want to remove " + txtName.Text + "?")) return;
            if (Run(() => controller.Delete(selectedId)))
            {
                Success("Staff member removed.");
                LoadStaff();
            }
        }
    }
}

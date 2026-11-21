using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Views
{
    public class ManageServicesForm : BaseForm
    {
        private readonly ServiceTypeController controller = new ServiceTypeController();
        private readonly DataGridView grid;
        private readonly TextBox txtName, txtDescription, txtPrice, txtDuration;
        private int selectedId;

        public ManageServicesForm() : base("Service Types", 860, 450)
        {
            AddTitle("Service Types");
            AddBackButton();
            grid = AddGrid(20, 60, 500, 370);
            grid.SelectionChanged += Grid_SelectionChanged;

            AddLabel("Name", 540, 70);
            txtName = AddTextBox(650, 70, 190);
            AddLabel("Description", 540, 110);
            txtDescription = AddTextBox(650, 110, 190);
            AddLabel("Price (RWF)", 540, 150);
            txtPrice = AddTextBox(650, 150, 190);
            AddLabel("Duration (min)", 540, 190);
            txtDuration = AddTextBox(650, 190, 190);

            AddButton("Add", 650, 240, Add_Click, 190);
            AddButton("Update", 650, 280, Update_Click, 190);
            AddButton("Delete", 650, 320, Delete_Click, 190);
            AddButton("Clear", 650, 360, (s, e) => ClearFields(), 190);
            LoadServices();
        }

        private void LoadServices()
        {
            Run(() =>
            {
                grid.DataSource = controller.GetAll();
                ClearFields();
            });
        }

        private void ClearFields()
        {
            selectedId = 0;
            txtName.Clear();
            txtDescription.Clear();
            txtPrice.Clear();
            txtDuration.Clear();
            grid.ClearSelection();
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow?.DataBoundItem is not ServiceType s || !grid.CurrentRow.Selected) return;
            selectedId = s.Id;
            txtName.Text = s.Name;
            txtDescription.Text = s.Description;
            txtPrice.Text = s.Price.ToString("0.##");
            txtDuration.Text = s.DurationMinutes.ToString();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (Run(() => controller.Add(txtName.Text, txtDescription.Text, txtPrice.Text, txtDuration.Text)))
            {
                Success("Service type added.");
                LoadServices();
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            if (Run(() =>
            {
                if (selectedId == 0) throw new AppException("Please select a service type first.");
                controller.Update(selectedId, txtName.Text, txtDescription.Text, txtPrice.Text, txtDuration.Text);
            }))
            {
                Success("Service type updated.");
                LoadServices();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Please select a service type first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Confirm("Are you sure you want to delete " + txtName.Text + "?")) return;
            if (Run(() => controller.Delete(selectedId)))
            {
                Success("Service type deleted.");
                LoadServices();
            }
        }
    }
}

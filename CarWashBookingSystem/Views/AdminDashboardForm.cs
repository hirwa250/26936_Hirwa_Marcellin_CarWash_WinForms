using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Views
{
    public class AdminDashboardForm : BaseForm
    {
        private readonly BookingController bookings = new BookingController();
        private readonly DataGridView grid;
        private readonly DateTimePicker dtpDate;
        private readonly CheckBox chkDate;
        private readonly ComboBox cboStatus;
        private readonly TextBox txtCustomer;
        private readonly Label lblToday;

        public AdminDashboardForm() : base("Administrator Dashboard", 1050, 580)
        {
            AddTitle("Administrator Dashboard");
            AddButton("Logout", ClientSize.Width - 140, 15, (s, e) => Close());

            AddButton("Service Types", 20, 60, (s, e) => OpenChild(new ManageServicesForm(), LoadBookings), 150);
            AddButton("Staff", 180, 60, (s, e) => OpenChild(new ManageStaffForm(), LoadBookings), 150);
            AddButton("Reports", 340, 60, (s, e) => OpenChild(new ReportsForm()), 150);
            AddButton("My Profile", 500, 60, (s, e) => OpenChild(new ProfileForm()), 150);

            lblToday = AddLabel("", 20, 105);
            lblToday.Font = new Font(Font, FontStyle.Bold);

            chkDate = new CheckBox { Text = "Date", Location = new Point(20, 140), AutoSize = true };
            Controls.Add(chkDate);
            dtpDate = new DateTimePicker { Location = new Point(85, 140), Width = 130, Format = DateTimePickerFormat.Short };
            Controls.Add(dtpDate);
            AddLabel("Status", 230, 140);
            cboStatus = AddComboBox(285, 140, 130);
            cboStatus.Items.Add("All");
            cboStatus.Items.AddRange(BookingStatus.All);
            cboStatus.SelectedIndex = 0;
            AddLabel("Customer", 430, 140);
            txtCustomer = AddTextBox(505, 140, 170);
            AddButton("Search", 690, 137, (s, e) => LoadBookings(), 100);
            AddButton("Clear", 800, 137, Clear_Click, 100);

            grid = AddGrid(20, 180, 1010, 320);

            AddButton("Approve / Start", 20, 520, (s, e) => NextStatus(BookingStatus.Pending), 150);
            AddButton("Mark Completed", 180, 520, (s, e) => NextStatus(BookingStatus.InProgress), 150);
            AddButton("Assign Staff", 340, 520, Assign_Click, 150);
            AddButton("Reschedule", 500, 520, Reschedule_Click, 150);
            AddButton("Cancel Booking", 660, 520, Cancel_Click, 150);
            AddButton("Refresh", 820, 520, (s, e) => LoadBookings(), 150);

            LoadBookings();
        }

        private void LoadBookings()
        {
            Run(() =>
            {
                string status = cboStatus.SelectedIndex <= 0 ? "" : cboStatus.Text;
                grid.DataSource = bookings.Search(chkDate.Checked ? dtpDate.Value.Date : (DateTime?)null, status, txtCustomer.Text.Trim());
                var counts = bookings.TodayCounts();
                lblToday.Text = "Today: " + string.Join("   |   ", counts.Select(c => c.Key + ": " + c.Value));
            });
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            chkDate.Checked = false;
            cboStatus.SelectedIndex = 0;
            txtCustomer.Clear();
            LoadBookings();
        }

        private void NextStatus(string expected)
        {
            Run(() =>
            {
                int id = SelectedId(grid);
                var booking = bookings.Get(id);
                if (booking.Status != expected)
                    throw new AppException("Booking #" + id + " is " + booking.Status + ". This action needs a " + expected + " booking.");
                bookings.MoveToNextStatus(id);
                Success("Booking #" + id + " updated.");
                LoadBookings();
            });
        }

        private void Assign_Click(object sender, EventArgs e)
        {
            Run(() => OpenChild(new AssignStaffForm(SelectedId(grid)), LoadBookings));
        }

        private void Reschedule_Click(object sender, EventArgs e)
        {
            Run(() =>
            {
                int id = SelectedId(grid);
                var booking = bookings.Get(id);
                if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
                    throw new AppException("A " + booking.Status.ToLower() + " booking cannot be changed.");
                OpenChild(new BookingForm(id), LoadBookings);
            });
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Run(() =>
            {
                int id = SelectedId(grid);
                if (!Confirm("Are you sure you want to cancel booking #" + id + "?")) return;
                bookings.Cancel(id);
                Success("Booking #" + id + " has been cancelled.");
                LoadBookings();
            });
        }
    }
}

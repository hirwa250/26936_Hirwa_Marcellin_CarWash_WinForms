using CarWashBookingSystem.Controllers;

namespace CarWashBookingSystem.Views
{
    public class MyBookingsForm : BaseForm
    {
        private readonly BookingController bookings = new BookingController();
        private readonly DataGridView grid;

        public MyBookingsForm() : base("My Bookings", 900, 470)
        {
            AddTitle("My Bookings");
            AddBackButton();
            grid = AddGrid(20, 60, 860, 340);
            AddButton("Cancel Booking", 20, 415, Cancel_Click, 140);
            AddButton("Reschedule", 170, 415, Reschedule_Click, 140);
            AddButton("Pay", 320, 415, Pay_Click, 140);
            AddButton("View Receipt", 470, 415, Receipt_Click, 140);
            AddButton("Refresh", 620, 415, (s, e) => LoadBookings(), 140);
            LoadBookings();
        }

        private void LoadBookings()
        {
            Run(() =>
            {
                grid.DataSource = bookings.GetMyBookings();
                if (grid.Columns.Contains("Customer")) grid.Columns["Customer"].Visible = false;
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

        private void Reschedule_Click(object sender, EventArgs e)
        {
            Run(() =>
            {
                int id = SelectedId(grid);
                var booking = bookings.Get(id);
                if (booking.Status != Models.BookingStatus.Pending)
                    throw new AppException("Only pending bookings can be rescheduled.");
                OpenChild(new BookingForm(id), LoadBookings);
            });
        }

        private void Pay_Click(object sender, EventArgs e)
        {
            Run(() => OpenChild(new PaymentForm(SelectedId(grid)), LoadBookings));
        }

        private void Receipt_Click(object sender, EventArgs e)
        {
            Run(() =>
            {
                var payment = new PaymentController().GetReceipt(SelectedId(grid));
                OpenChild(new ReceiptForm(payment));
            });
        }
    }
}

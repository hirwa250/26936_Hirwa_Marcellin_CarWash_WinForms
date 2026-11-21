using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Views
{
    public class BookingForm : BaseForm
    {
        private readonly BookingController bookings = new BookingController();
        private readonly ComboBox cboService, cboSlot;
        private readonly DateTimePicker dtpDate;
        private readonly Label lblPrice;
        private readonly int rescheduleId;

        public BookingForm(int rescheduleBookingId = 0) : base(rescheduleBookingId == 0 ? "Book a Car Wash" : "Reschedule", 460, 330)
        {
            rescheduleId = rescheduleBookingId;
            AddTitle(rescheduleId == 0 ? "Book a Car Wash" : "Reschedule Booking #" + rescheduleId);
            AddBackButton();

            AddLabel("Service", 30, 75);
            cboService = AddComboBox(150, 75, 270);
            AddLabel("Date", 30, 115);
            dtpDate = new DateTimePicker { Location = new Point(150, 115), Width = 270, Format = DateTimePickerFormat.Short, MinDate = DateTime.Today };
            Controls.Add(dtpDate);
            AddLabel("Time slot", 30, 155);
            cboSlot = AddComboBox(150, 155, 270);
            AddLabel("Price", 30, 195);
            lblPrice = AddLabel("-", 150, 195);
            lblPrice.Font = new Font(Font, FontStyle.Bold);
            AddButton(rescheduleId == 0 ? "Confirm Booking" : "Save New Time", 150, 245, Save_Click, 270);

            Run(() => cboService.DataSource = new ServiceTypeController().GetAll());
            cboService.SelectedIndexChanged += (s, e) => ShowPrice();
            dtpDate.ValueChanged += (s, e) => LoadSlots();

            if (rescheduleId != 0)
            {
                Run(() =>
                {
                    var booking = bookings.Get(rescheduleId);
                    cboService.SelectedItem = ((List<ServiceType>)cboService.DataSource).FirstOrDefault(x => x.Id == booking.ServiceTypeId);
                    cboService.Enabled = false;
                    if (booking.BookingDate >= DateTime.Today) dtpDate.Value = booking.BookingDate;
                });
            }
            ShowPrice();
            LoadSlots();
        }

        private void ShowPrice()
        {
            lblPrice.Text = cboService.SelectedItem is ServiceType s ? s.Price.ToString("N0") + " RWF (" + s.DurationMinutes + " min)" : "-";
        }

        private void LoadSlots()
        {
            Run(() =>
            {
                cboSlot.DataSource = bookings.GetFreeSlots(dtpDate.Value.Date, rescheduleId);
                if (cboSlot.Items.Count == 0)
                    MessageBox.Show("There are no free time slots on this date. Please choose another date.", "No slots", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (rescheduleId == 0)
            {
                int id = 0;
                var service = cboService.SelectedItem as ServiceType;
                if (service != null && !Confirm("Book " + service.Name + " on " + dtpDate.Value.ToString("yyyy-MM-dd") + " at " + cboSlot.Text + " for " + service.Price.ToString("N0") + " RWF?"))
                    return;
                if (Run(() => id = bookings.Book(service, dtpDate.Value.Date, cboSlot.Text)))
                {
                    Success("Booking confirmed. Your booking ID is #" + id + ".");
                    Close();
                }
            }
            else if (Run(() => bookings.Reschedule(rescheduleId, dtpDate.Value.Date, cboSlot.Text)))
            {
                Success("Booking #" + rescheduleId + " has been rescheduled.");
                Close();
            }
        }
    }
}

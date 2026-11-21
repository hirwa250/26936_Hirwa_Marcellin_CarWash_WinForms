using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Views
{
    public class AssignStaffForm : BaseForm
    {
        private readonly BookingController bookings = new BookingController();
        private readonly CheckedListBox lstStaff;
        private readonly int bookingId;

        public AssignStaffForm(int id) : base("Assign Staff", 420, 400)
        {
            bookingId = id;
            var booking = bookings.Get(id);
            if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
                throw new AppException("Staff cannot be assigned to a " + booking.Status.ToLower() + " booking.");

            AddTitle("Assign Staff #" + id);
            AddBackButton();
            AddLabel(booking.BookingDate.ToString("yyyy-MM-dd") + " at " + booking.TimeSlot + " (available staff only)", 20, 60);
            lstStaff = new CheckedListBox { Location = new Point(20, 90), Size = new Size(380, 230), CheckOnClick = true };
            Controls.Add(lstStaff);

            var assigned = bookings.GetAssignedStaff(id);
            foreach (var staff in new StaffController().GetAll().Where(s => s.IsAvailable || assigned.Contains(s.Id)))
                lstStaff.Items.Add(staff, assigned.Contains(staff.Id));

            AddButton("Save", 20, 340, Save_Click, 380);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var ids = lstStaff.CheckedItems.Cast<Staff>().Select(s => s.Id).ToList();
            if (ids.Count == 0 && !Confirm("No staff selected. Remove all staff from this booking?"))
                return;
            if (Run(() => bookings.AssignStaff(bookingId, ids)))
            {
                Success("Staff assignment saved.");
                Close();
            }
        }
    }
}

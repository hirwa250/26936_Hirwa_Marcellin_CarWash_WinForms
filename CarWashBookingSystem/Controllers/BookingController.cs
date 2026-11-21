using System.Data;
using CarWashBookingSystem.Data;
using CarWashBookingSystem.Helpers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Controllers
{
    public class BookingController
    {
        private readonly BookingRepository bookings = new BookingRepository();
        private readonly ServiceTypeRepository services = new ServiceTypeRepository();
        private readonly StaffRepository staff = new StaffRepository();

        public static readonly string[] AllSlots = { "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00" };

        public List<string> GetFreeSlots(DateTime date, int excludeBookingId = 0)
        {
            var taken = bookings.GetTakenSlots(date, excludeBookingId);
            return AllSlots.Where(s => !taken.Contains(s) && date.Date + TimeSpan.Parse(s) > DateTime.Now).ToList();
        }

        private void CheckSlot(DateTime date, string slot, int excludeBookingId)
        {
            if (string.IsNullOrEmpty(slot))
                throw new AppException("Please select a time slot.");
            if (date.Date + TimeSpan.Parse(slot) <= DateTime.Now)
                throw new AppException("The booking date and time cannot be in the past.");
            if (bookings.GetTakenSlots(date, excludeBookingId).Contains(slot))
                throw new AppException("This time slot is already taken. Please choose another one.");
        }

        public int Book(ServiceType service, DateTime date, string slot)
        {
            if (service == null)
                throw new AppException("Please select a service.");
            CheckSlot(date, slot, 0);
            var booking = new Booking
            {
                CustomerId = Session.CurrentUser.Id,
                ServiceTypeId = service.Id,
                BookingDate = date.Date,
                TimeSlot = slot,
                Status = BookingStatus.Pending,
                Amount = service.Price
            };
            return bookings.Add(booking, "Your booking #{0} for " + service.Name + " on " + date.ToString("yyyy-MM-dd") + " at " + slot + " is confirmed.");
        }

        public Booking Get(int id)
        {
            var booking = bookings.GetById(id);
            if (booking == null)
                throw new AppException("Booking not found.");
            if (Session.CurrentUser.Role != "Admin" && booking.CustomerId != Session.CurrentUser.Id)
                throw new AppException("You can only manage your own bookings.");
            return booking;
        }

        private bool IsAdmin
        {
            get { return Session.CurrentUser.Role == "Admin"; }
        }

        private void CheckCanChange(Booking booking)
        {
            if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
                throw new AppException("A " + booking.Status.ToLower() + " booking cannot be changed.");
            if (!IsAdmin && booking.Status != BookingStatus.Pending)
                throw new AppException("This booking is already in progress.");
            if (!IsAdmin && booking.ScheduledAt - DateTime.Now < TimeSpan.FromHours(1))
                throw new AppException("A booking cannot be cancelled or rescheduled less than 1 hour before the scheduled time.");
        }

        public void Cancel(int id)
        {
            var booking = Get(id);
            CheckCanChange(booking);
            bookings.UpdateStatus(booking, BookingStatus.Cancelled, "Your booking #" + booking.Id + " has been cancelled.");
        }

        public void Reschedule(int id, DateTime date, string slot)
        {
            var booking = Get(id);
            CheckCanChange(booking);
            CheckSlot(date, slot, booking.Id);
            foreach (int staffId in staff.GetAssignedStaffIds(booking.Id))
                if (staff.IsBusy(staffId, booking.Id, date.ToString("yyyy-MM-dd"), slot))
                    throw new AppException("An assigned staff member is busy at the new time. Change the staff assignment first.");
            bookings.Reschedule(booking, date, slot, "Your booking #" + booking.Id + " was moved to " + date.ToString("yyyy-MM-dd") + " at " + slot + ".");
        }

        public void MoveToNextStatus(int id)
        {
            var booking = Get(id);
            string next;
            if (booking.Status == BookingStatus.Pending)
                next = BookingStatus.InProgress;
            else if (booking.Status == BookingStatus.InProgress)
                next = BookingStatus.Completed;
            else
                throw new AppException("A " + booking.Status.ToLower() + " booking cannot be changed.");
            bookings.UpdateStatus(booking, next, "Your booking #" + booking.Id + " is now " + next + ".");
        }

        public void AssignStaff(int id, List<int> staffIds)
        {
            var booking = Get(id);
            if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
                throw new AppException("Staff cannot be assigned to a " + booking.Status.ToLower() + " booking.");
            var all = staff.GetAll();
            foreach (int staffId in staffIds)
                if (staff.IsBusy(staffId, booking.Id, booking.BookingDate.ToString("yyyy-MM-dd"), booking.TimeSlot))
                    throw new AppException(all.First(s => s.Id == staffId).FullName + " is already assigned to another booking at this time.");
            staff.SetAssignments(booking.Id, staffIds);
        }

        public List<int> GetAssignedStaff(int id)
        {
            return staff.GetAssignedStaffIds(id);
        }

        public DataTable GetMyBookings()
        {
            return bookings.GetForCustomer(Session.CurrentUser.Id);
        }

        public DataTable Search(DateTime? date, string status, string customer)
        {
            return bookings.Search(date, status, customer);
        }

        public Dictionary<string, int> TodayCounts()
        {
            return bookings.CountByStatusForDate(DateTime.Today);
        }

        public ServiceType GetService(int id)
        {
            return services.GetById(id);
        }
    }
}

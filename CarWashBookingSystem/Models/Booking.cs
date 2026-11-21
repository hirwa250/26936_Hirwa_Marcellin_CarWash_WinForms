namespace CarWashBookingSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime BookingDate { get; set; }
        public string TimeSlot { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }

        public DateTime ScheduledAt
        {
            get { return BookingDate.Date + TimeSpan.Parse(TimeSlot); }
        }
    }

    public static class BookingStatus
    {
        public const string Pending = "Pending";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static readonly string[] All = { Pending, InProgress, Completed, Cancelled };
    }
}

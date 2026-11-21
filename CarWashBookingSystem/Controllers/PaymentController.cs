using CarWashBookingSystem.Data;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Controllers
{
    public class PaymentController
    {
        private readonly PaymentRepository payments = new PaymentRepository();
        private readonly BookingController bookingController = new BookingController();

        public static readonly string[] Methods = { "Cash", "Mobile Money" };

        public Payment Pay(int bookingId, string method)
        {
            var booking = bookingController.Get(bookingId);
            if (booking.Status != BookingStatus.Completed)
                throw new AppException("Only completed bookings can be paid.");
            if (payments.GetByBooking(bookingId) != null)
                throw new AppException("This booking has already been paid.");
            if (string.IsNullOrEmpty(method))
                throw new AppException("Please choose a payment method.");
            var payment = new Payment
            {
                BookingId = booking.Id,
                Method = method,
                Amount = booking.Amount,
                PaidAt = DateTime.Now,
                ReceiptNumber = "RC-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + booking.Id
            };
            payments.Add(payment, booking.CustomerId);
            return payment;
        }

        public bool IsPaid(int bookingId)
        {
            return payments.GetByBooking(bookingId) != null;
        }

        public User GetCustomer(int bookingId)
        {
            return new UserRepository().GetById(bookingController.Get(bookingId).CustomerId);
        }

        public Payment GetReceipt(int bookingId)
        {
            bookingController.Get(bookingId);
            var payment = payments.GetByBooking(bookingId);
            if (payment == null)
                throw new AppException("This booking has not been paid yet.");
            return payment;
        }
    }
}

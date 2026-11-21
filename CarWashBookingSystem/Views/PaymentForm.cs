using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Views
{
    public class PaymentForm : BaseForm
    {
        private readonly PaymentController payments = new PaymentController();
        private readonly ComboBox cboMethod;
        private readonly int bookingId;

        public PaymentForm(int id) : base("Payment", 420, 260)
        {
            bookingId = id;
            var bookingController = new BookingController();
            var booking = bookingController.Get(id);
            if (booking.Status != BookingStatus.Completed)
                throw new AppException("Only completed bookings can be paid.");
            if (payments.IsPaid(id))
                throw new AppException("This booking has already been paid.");
            var service = bookingController.GetService(booking.ServiceTypeId);

            AddTitle("Pay Booking #" + id);
            AddBackButton();
            AddLabel("Service", 30, 75);
            AddLabel(service.Name, 170, 75);
            AddLabel("Amount", 30, 110);
            var amount = AddLabel(booking.Amount.ToString("N0") + " RWF", 170, 110);
            amount.Font = new Font(Font, FontStyle.Bold);
            AddLabel("Payment method", 30, 145);
            cboMethod = AddComboBox(170, 145, 210);
            cboMethod.Items.AddRange(PaymentController.Methods);
            AddButton("Pay Now", 170, 195, Pay_Click, 210);
        }

        private void Pay_Click(object sender, EventArgs e)
        {
            Payment payment = null;
            if (Run(() => payment = payments.Pay(bookingId, cboMethod.Text)))
            {
                Success("Payment successful. Receipt number: " + payment.ReceiptNumber);
                var receipt = new ReceiptForm(payment);
                receipt.FormClosed += (s, ev) => Close();
                Hide();
                receipt.Show();
            }
        }
    }
}

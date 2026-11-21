using System.Drawing.Printing;
using CarWashBookingSystem.Controllers;
using CarWashBookingSystem.Models;

namespace CarWashBookingSystem.Views
{
    public class ReceiptForm : BaseForm
    {
        private readonly string receiptText;

        public ReceiptForm(Payment payment) : base("Receipt", 420, 420)
        {
            var booking = new BookingController().Get(payment.BookingId);
            var service = new BookingController().GetService(booking.ServiceTypeId);
            var customer = new PaymentController().GetCustomer(payment.BookingId);

            receiptText = string.Join(Environment.NewLine, new[]
            {
                "CAR WASH RECEIPT",
                "",
                "Receipt No:   " + payment.ReceiptNumber,
                "Date paid:    " + payment.PaidAt.ToString("yyyy-MM-dd HH:mm"),
                "",
                "Customer:     " + customer.FullName,
                "Plate:        " + customer.PlateNumber,
                "Booking ID:   #" + booking.Id,
                "Service:      " + service.Name,
                "Service date: " + booking.BookingDate.ToString("yyyy-MM-dd") + " " + booking.TimeSlot,
                "",
                "Method:       " + payment.Method,
                "Amount paid:  " + payment.Amount.ToString("N0") + " RWF",
                "",
                "Thank you!"
            });

            AddTitle("Receipt");
            AddBackButton();
            var box = new TextBox
            {
                Location = new Point(20, 60),
                Size = new Size(380, 290),
                Multiline = true,
                ReadOnly = true,
                BackColor = BackColorValue,
                Font = new Font("Consolas", 10),
                Text = receiptText
            };
            Controls.Add(box);
            AddButton("Print", 20, 365, Print_Click, 380);
        }

        private void Print_Click(object sender, EventArgs e)
        {
            Run(() =>
            {
                var document = new PrintDocument();
                document.PrintPage += (s, ev) =>
                    ev.Graphics.DrawString(receiptText, new Font("Consolas", 11), Brushes.Black, ev.MarginBounds.Left, ev.MarginBounds.Top);
                using var dialog = new PrintDialog { Document = document };
                if (dialog.ShowDialog() == DialogResult.OK)
                    document.Print();
            });
        }
    }
}

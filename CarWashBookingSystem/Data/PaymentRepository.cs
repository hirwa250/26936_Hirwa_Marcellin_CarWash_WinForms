using CarWashBookingSystem.Models;
using Microsoft.Data.Sqlite;

namespace CarWashBookingSystem.Data
{
    public class PaymentRepository
    {
        private static Payment Map(SqliteDataReader r)
        {
            return new Payment
            {
                Id = r.GetInt32(0),
                BookingId = r.GetInt32(1),
                ReceiptNumber = r.GetString(2),
                Method = r.GetString(3),
                Amount = Convert.ToDecimal(r.GetDouble(4)),
                PaidAt = DateTime.Parse(r.GetString(5))
            };
        }

        public Payment GetByBooking(int bookingId)
        {
            return DbHelper.List("SELECT Id, BookingId, ReceiptNumber, Method, Amount, PaidAt FROM Payments WHERE BookingId = $b", Map, ("$b", bookingId)).FirstOrDefault();
        }

        public void Add(Payment p, int customerId)
        {
            using var connection = Database.Open();
            using var transaction = connection.BeginTransaction();
            using var insert = DbHelper.Command(connection,
                "INSERT INTO Payments (BookingId, ReceiptNumber, Method, Amount, PaidAt) VALUES ($b, $r, $m, $a, $p)",
                ("$b", p.BookingId), ("$r", p.ReceiptNumber), ("$m", p.Method), ("$a", (double)p.Amount), ("$p", p.PaidAt.ToString("yyyy-MM-dd HH:mm:ss")));
            insert.Transaction = transaction;
            insert.ExecuteNonQuery();
            NotificationRepository.Insert(connection, transaction, customerId,
                "Payment received for booking #" + p.BookingId + ". Receipt " + p.ReceiptNumber + ".");
            transaction.Commit();
        }
    }
}

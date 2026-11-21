using System.Data;
using CarWashBookingSystem.Models;
using Microsoft.Data.Sqlite;

namespace CarWashBookingSystem.Data
{
    public class BookingRepository
    {
        private const string Columns = "Id, CustomerId, ServiceTypeId, BookingDate, TimeSlot, Status, Amount";

        private static Booking Map(SqliteDataReader r)
        {
            return new Booking
            {
                Id = r.GetInt32(0),
                CustomerId = r.GetInt32(1),
                ServiceTypeId = r.GetInt32(2),
                BookingDate = DateTime.Parse(r.GetString(3)),
                TimeSlot = r.GetString(4),
                Status = r.GetString(5),
                Amount = Convert.ToDecimal(r.GetDouble(6))
            };
        }

        public Booking GetById(int id)
        {
            return DbHelper.List("SELECT " + Columns + " FROM Bookings WHERE Id = $id", Map, ("$id", id)).FirstOrDefault();
        }

        public List<string> GetTakenSlots(DateTime date, int excludeBookingId)
        {
            return DbHelper.List("SELECT TimeSlot FROM Bookings WHERE BookingDate = $d AND Status <> 'Cancelled' AND Id <> $id",
                r => r.GetString(0), ("$d", date.ToString("yyyy-MM-dd")), ("$id", excludeBookingId));
        }

        public int Add(Booking b, string notification)
        {
            using var connection = Database.Open();
            using var transaction = connection.BeginTransaction();
            using var insert = DbHelper.Command(connection,
                "INSERT INTO Bookings (CustomerId, ServiceTypeId, BookingDate, TimeSlot, Status, Amount, CreatedAt) VALUES ($c, $s, $d, $t, $st, $a, $cr); SELECT last_insert_rowid();",
                ("$c", b.CustomerId), ("$s", b.ServiceTypeId), ("$d", b.BookingDate.ToString("yyyy-MM-dd")), ("$t", b.TimeSlot),
                ("$st", b.Status), ("$a", (double)b.Amount), ("$cr", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            insert.Transaction = transaction;
            int id = Convert.ToInt32(insert.ExecuteScalar());
            NotificationRepository.Insert(connection, transaction, b.CustomerId, string.Format(notification, id));
            transaction.Commit();
            return id;
        }

        public void UpdateStatus(Booking b, string status, string notification)
        {
            using var connection = Database.Open();
            using var transaction = connection.BeginTransaction();
            using var update = DbHelper.Command(connection, "UPDATE Bookings SET Status = $s WHERE Id = $id", ("$s", status), ("$id", b.Id));
            update.Transaction = transaction;
            update.ExecuteNonQuery();
            NotificationRepository.Insert(connection, transaction, b.CustomerId, notification);
            transaction.Commit();
        }

        public void Reschedule(Booking b, DateTime date, string slot, string notification)
        {
            using var connection = Database.Open();
            using var transaction = connection.BeginTransaction();
            using var update = DbHelper.Command(connection, "UPDATE Bookings SET BookingDate = $d, TimeSlot = $t WHERE Id = $id",
                ("$d", date.ToString("yyyy-MM-dd")), ("$t", slot), ("$id", b.Id));
            update.Transaction = transaction;
            update.ExecuteNonQuery();
            NotificationRepository.Insert(connection, transaction, b.CustomerId, notification);
            transaction.Commit();
        }

        private const string ListSql = @"SELECT b.Id AS 'Booking ID', u.FullName AS Customer, u.PlateNumber AS Plate, s.Name AS Service,
b.BookingDate AS Date, b.TimeSlot AS Time, b.Status, b.Amount,
CASE WHEN p.Id IS NULL THEN 'No' ELSE 'Yes' END AS Paid,
(SELECT GROUP_CONCAT(st.FullName, ', ') FROM BookingStaff bs JOIN Staff st ON st.Id = bs.StaffId WHERE bs.BookingId = b.Id) AS Staff
FROM Bookings b JOIN Users u ON u.Id = b.CustomerId JOIN ServiceTypes s ON s.Id = b.ServiceTypeId
LEFT JOIN Payments p ON p.BookingId = b.Id";

        public DataTable GetForCustomer(int customerId)
        {
            return DbHelper.Table(ListSql + " WHERE b.CustomerId = $c ORDER BY b.BookingDate DESC, b.TimeSlot DESC", ("$c", customerId));
        }

        public DataTable Search(DateTime? date, string status, string customer)
        {
            return DbHelper.Table(ListSql + @" WHERE ($d IS NULL OR b.BookingDate = $d) AND ($s = '' OR b.Status = $s) AND u.FullName LIKE $n
ORDER BY b.BookingDate DESC, b.TimeSlot DESC",
                ("$d", date.HasValue ? date.Value.ToString("yyyy-MM-dd") : null), ("$s", status ?? ""), ("$n", "%" + (customer ?? "") + "%"));
        }

        public Dictionary<string, int> CountByStatusForDate(DateTime date)
        {
            var result = BookingStatus.All.ToDictionary(s => s, s => 0);
            var rows = DbHelper.List("SELECT Status, COUNT(*) FROM Bookings WHERE BookingDate = $d GROUP BY Status",
                r => (r.GetString(0), r.GetInt32(1)), ("$d", date.ToString("yyyy-MM-dd")));
            foreach (var (status, count) in rows)
                result[status] = count;
            return result;
        }
    }
}

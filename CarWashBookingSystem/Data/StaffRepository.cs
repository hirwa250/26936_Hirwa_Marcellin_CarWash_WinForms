using System.Data;
using CarWashBookingSystem.Models;
using Microsoft.Data.Sqlite;

namespace CarWashBookingSystem.Data
{
    public class StaffRepository
    {
        private static Staff Map(SqliteDataReader r)
        {
            return new Staff
            {
                Id = r.GetInt32(0),
                FullName = r.GetString(1),
                Phone = r.GetString(2),
                Specialization = r.IsDBNull(3) ? "" : r.GetString(3),
                IsAvailable = r.GetInt32(4) == 1
            };
        }

        public List<Staff> GetAll()
        {
            return DbHelper.List("SELECT Id, FullName, Phone, Specialization, IsAvailable FROM Staff ORDER BY FullName", Map);
        }

        public void Add(Staff s)
        {
            DbHelper.Execute("INSERT INTO Staff (FullName, Phone, Specialization, IsAvailable) VALUES ($n, $p, $s, $a)",
                ("$n", s.FullName), ("$p", s.Phone), ("$s", s.Specialization), ("$a", s.IsAvailable ? 1 : 0));
        }

        public void Update(Staff s)
        {
            DbHelper.Execute("UPDATE Staff SET FullName = $n, Phone = $p, Specialization = $s, IsAvailable = $a WHERE Id = $id",
                ("$n", s.FullName), ("$p", s.Phone), ("$s", s.Specialization), ("$a", s.IsAvailable ? 1 : 0), ("$id", s.Id));
        }

        public void Delete(int id)
        {
            DbHelper.Execute("DELETE FROM Staff WHERE Id = $id", ("$id", id));
        }

        public List<int> GetAssignedStaffIds(int bookingId)
        {
            return DbHelper.List("SELECT StaffId FROM BookingStaff WHERE BookingId = $b", r => r.GetInt32(0), ("$b", bookingId));
        }

        public bool IsBusy(int staffId, int bookingId, string date, string slot)
        {
            return Convert.ToInt32(DbHelper.Scalar(@"SELECT COUNT(*) FROM BookingStaff bs JOIN Bookings b ON b.Id = bs.BookingId
WHERE bs.StaffId = $s AND b.Id <> $b AND b.BookingDate = $d AND b.TimeSlot = $t AND b.Status <> 'Cancelled'",
                ("$s", staffId), ("$b", bookingId), ("$d", date), ("$t", slot))) > 0;
        }

        public void SetAssignments(int bookingId, List<int> staffIds)
        {
            using var connection = Database.Open();
            using var transaction = connection.BeginTransaction();
            using (var delete = DbHelper.Command(connection, "DELETE FROM BookingStaff WHERE BookingId = $b", ("$b", bookingId)))
            {
                delete.Transaction = transaction;
                delete.ExecuteNonQuery();
            }
            foreach (int id in staffIds)
            {
                using var insert = DbHelper.Command(connection, "INSERT INTO BookingStaff (BookingId, StaffId) VALUES ($b, $s)", ("$b", bookingId), ("$s", id));
                insert.Transaction = transaction;
                insert.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        public DataTable GetBookingsForStaff(int staffId)
        {
            return DbHelper.Table(@"SELECT b.Id AS 'Booking ID', u.FullName AS Customer, s.Name AS Service, b.BookingDate AS Date, b.TimeSlot AS Time, b.Status
FROM BookingStaff bs JOIN Bookings b ON b.Id = bs.BookingId JOIN Users u ON u.Id = b.CustomerId JOIN ServiceTypes s ON s.Id = b.ServiceTypeId
WHERE bs.StaffId = $s ORDER BY b.BookingDate DESC, b.TimeSlot", ("$s", staffId));
        }
    }
}

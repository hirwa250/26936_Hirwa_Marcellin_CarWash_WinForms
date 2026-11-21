using System.Data;

namespace CarWashBookingSystem.Data
{
    public class ReportRepository
    {
        private static (string, object)[] Period(DateTime from, DateTime to)
        {
            return new (string, object)[] { ("$f", from.ToString("yyyy-MM-dd")), ("$t", to.ToString("yyyy-MM-dd")) };
        }

        public DataTable BookingsByStatus(DateTime from, DateTime to)
        {
            return DbHelper.Table("SELECT Status, COUNT(*) AS Bookings FROM Bookings WHERE BookingDate BETWEEN $f AND $t GROUP BY Status ORDER BY Status", Period(from, to));
        }

        public DataTable BookingsByService(DateTime from, DateTime to)
        {
            return DbHelper.Table(@"SELECT s.Name AS Service, COUNT(b.Id) AS Bookings FROM ServiceTypes s
LEFT JOIN Bookings b ON b.ServiceTypeId = s.Id AND b.BookingDate BETWEEN $f AND $t GROUP BY s.Id ORDER BY s.Name", Period(from, to));
        }

        public DataTable RevenueByDay(DateTime from, DateTime to)
        {
            return DbHelper.Table("SELECT substr(PaidAt, 1, 10) AS Day, COUNT(*) AS Payments, SUM(Amount) AS Total FROM Payments WHERE substr(PaidAt, 1, 10) BETWEEN $f AND $t GROUP BY Day ORDER BY Day", Period(from, to));
        }

        public DataTable RevenueByService(DateTime from, DateTime to)
        {
            return DbHelper.Table(@"SELECT s.Name AS Service, COUNT(p.Id) AS Payments, IFNULL(SUM(p.Amount), 0) AS Total FROM Payments p
JOIN Bookings b ON b.Id = p.BookingId JOIN ServiceTypes s ON s.Id = b.ServiceTypeId
WHERE substr(p.PaidAt, 1, 10) BETWEEN $f AND $t GROUP BY s.Id ORDER BY s.Name", Period(from, to));
        }

        public DataTable StaffCompleted(DateTime from, DateTime to)
        {
            return DbHelper.Table(@"SELECT st.FullName AS Staff, st.Specialization, COUNT(b.Id) AS Completed FROM Staff st
LEFT JOIN BookingStaff bs ON bs.StaffId = st.Id
LEFT JOIN Bookings b ON b.Id = bs.BookingId AND b.Status = 'Completed' AND b.BookingDate BETWEEN $f AND $t
GROUP BY st.Id ORDER BY Completed DESC", Period(from, to));
        }
    }
}

using System.Data;
using CarWashBookingSystem.Data;
using CarWashBookingSystem.Helpers;

namespace CarWashBookingSystem.Controllers
{
    public class ReportController
    {
        private readonly ReportRepository repository = new ReportRepository();

        public static readonly string[] Reports =
        {
            "Bookings per status",
            "Bookings per service type",
            "Revenue per day",
            "Revenue per service type",
            "Staff completed bookings"
        };

        public DataTable Generate(string report, DateTime from, DateTime to)
        {
            if (from.Date > to.Date)
                throw new AppException("The start date must be before the end date.");
            switch (report)
            {
                case "Bookings per status": return repository.BookingsByStatus(from, to);
                case "Bookings per service type": return repository.BookingsByService(from, to);
                case "Revenue per day": return repository.RevenueByDay(from, to);
                case "Revenue per service type": return repository.RevenueByService(from, to);
                case "Staff completed bookings": return repository.StaffCompleted(from, to);
                default: throw new AppException("Please choose a report.");
            }
        }

        public void Export(string title, DataTable table, string path)
        {
            if (table == null)
                throw new AppException("Generate a report first.");
            if (path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                ReportExporter.ToPdf(title, table, path);
            else
                ReportExporter.ToCsv(table, path);
        }
    }
}

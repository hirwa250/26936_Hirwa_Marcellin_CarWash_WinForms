using CarWashBookingSystem.Models;
using Microsoft.Data.Sqlite;

namespace CarWashBookingSystem.Data
{
    public class ServiceTypeRepository
    {
        private static ServiceType Map(SqliteDataReader r)
        {
            return new ServiceType
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Description = r.IsDBNull(2) ? "" : r.GetString(2),
                Price = Convert.ToDecimal(r.GetDouble(3)),
                DurationMinutes = r.GetInt32(4)
            };
        }

        public List<ServiceType> GetAll()
        {
            return DbHelper.List("SELECT Id, Name, Description, Price, DurationMinutes FROM ServiceTypes ORDER BY Price", Map);
        }

        public ServiceType GetById(int id)
        {
            return DbHelper.List("SELECT Id, Name, Description, Price, DurationMinutes FROM ServiceTypes WHERE Id = $id", Map, ("$id", id)).FirstOrDefault();
        }

        public void Add(ServiceType s)
        {
            DbHelper.Execute("INSERT INTO ServiceTypes (Name, Description, Price, DurationMinutes) VALUES ($n, $d, $p, $m)",
                ("$n", s.Name), ("$d", s.Description), ("$p", (double)s.Price), ("$m", s.DurationMinutes));
        }

        public void Update(ServiceType s)
        {
            DbHelper.Execute("UPDATE ServiceTypes SET Name = $n, Description = $d, Price = $p, DurationMinutes = $m WHERE Id = $id",
                ("$n", s.Name), ("$d", s.Description), ("$p", (double)s.Price), ("$m", s.DurationMinutes), ("$id", s.Id));
        }

        public int CountActiveBookings(int id)
        {
            return Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Bookings WHERE ServiceTypeId = $id AND Status IN ('Pending', 'In Progress')", ("$id", id)));
        }

        public int CountAllBookings(int id)
        {
            return Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Bookings WHERE ServiceTypeId = $id", ("$id", id)));
        }

        public void Delete(int id)
        {
            DbHelper.Execute("DELETE FROM ServiceTypes WHERE Id = $id", ("$id", id));
        }
    }
}

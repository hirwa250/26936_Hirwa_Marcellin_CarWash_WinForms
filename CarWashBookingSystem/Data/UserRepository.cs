using CarWashBookingSystem.Models;
using Microsoft.Data.Sqlite;

namespace CarWashBookingSystem.Data
{
    public class UserRepository
    {
        private static User Map(SqliteDataReader r)
        {
            return new User
            {
                Id = r.GetInt32(0),
                FullName = r.GetString(1),
                Email = r.GetString(2),
                Phone = r.GetString(3),
                PasswordHash = r.GetString(4),
                Role = r.GetString(5),
                PlateNumber = r.IsDBNull(6) ? "" : r.GetString(6),
                VehicleType = r.IsDBNull(7) ? "" : r.GetString(7)
            };
        }

        private const string Columns = "Id, FullName, Email, Phone, PasswordHash, Role, PlateNumber, VehicleType";

        public User GetByEmail(string email)
        {
            return DbHelper.List("SELECT " + Columns + " FROM Users WHERE Email = $email", Map, ("$email", email)).FirstOrDefault();
        }

        public User GetById(int id)
        {
            return DbHelper.List("SELECT " + Columns + " FROM Users WHERE Id = $id", Map, ("$id", id)).FirstOrDefault();
        }

        public bool EmailExists(string email)
        {
            return Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Users WHERE Email = $email", ("$email", email))) > 0;
        }

        public void Add(User user)
        {
            DbHelper.Execute("INSERT INTO Users (FullName, Email, Phone, PasswordHash, Role, PlateNumber, VehicleType) VALUES ($n, $e, $p, $h, $r, $pl, $v)",
                ("$n", user.FullName), ("$e", user.Email), ("$p", user.Phone), ("$h", user.PasswordHash),
                ("$r", user.Role), ("$pl", user.PlateNumber), ("$v", user.VehicleType));
        }

        public void UpdateProfile(User user)
        {
            DbHelper.Execute("UPDATE Users SET Phone = $p, PlateNumber = $pl, VehicleType = $v WHERE Id = $id",
                ("$p", user.Phone), ("$pl", user.PlateNumber), ("$v", user.VehicleType), ("$id", user.Id));
        }

        public void UpdatePassword(int id, string hash)
        {
            DbHelper.Execute("UPDATE Users SET PasswordHash = $h WHERE Id = $id", ("$h", hash), ("$id", id));
        }
    }
}

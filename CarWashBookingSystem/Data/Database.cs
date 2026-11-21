using CarWashBookingSystem.Helpers;
using Microsoft.Data.Sqlite;

namespace CarWashBookingSystem.Data
{
    public static class Database
    {
        private static readonly string DbPath = Path.Combine(AppContext.BaseDirectory, "carwash.db");

        public static SqliteConnection Open()
        {
            var connection = new SqliteConnection("Data Source=" + DbPath);
            connection.Open();
            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }
            return connection;
        }

        public static void Initialize()
        {
            using var connection = Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE COLLATE NOCASE,
    Phone TEXT NOT NULL,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL,
    PlateNumber TEXT,
    VehicleType TEXT
);
CREATE TABLE IF NOT EXISTS ServiceTypes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    Price REAL NOT NULL,
    DurationMinutes INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS Staff (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Phone TEXT NOT NULL,
    Specialization TEXT,
    IsAvailable INTEGER NOT NULL DEFAULT 1
);
CREATE TABLE IF NOT EXISTS Bookings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerId INTEGER NOT NULL REFERENCES Users(Id),
    ServiceTypeId INTEGER NOT NULL REFERENCES ServiceTypes(Id),
    BookingDate TEXT NOT NULL,
    TimeSlot TEXT NOT NULL,
    Status TEXT NOT NULL,
    Amount REAL NOT NULL,
    CreatedAt TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS BookingStaff (
    BookingId INTEGER NOT NULL REFERENCES Bookings(Id) ON DELETE CASCADE,
    StaffId INTEGER NOT NULL REFERENCES Staff(Id) ON DELETE CASCADE,
    PRIMARY KEY (BookingId, StaffId)
);
CREATE TABLE IF NOT EXISTS Payments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    BookingId INTEGER NOT NULL UNIQUE REFERENCES Bookings(Id),
    ReceiptNumber TEXT NOT NULL UNIQUE,
    Method TEXT NOT NULL,
    Amount REAL NOT NULL,
    PaidAt TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Notifications (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    Message TEXT NOT NULL,
    IsRead INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL
);";
            command.ExecuteNonQuery();
            Seed(connection);
        }

        private static void Seed(SqliteConnection connection)
        {
            using var check = connection.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM Users WHERE Role = 'Admin'";
            if (Convert.ToInt32(check.ExecuteScalar()) == 0)
            {
                using var insert = connection.CreateCommand();
                insert.CommandText = "INSERT INTO Users (FullName, Email, Phone, PasswordHash, Role) VALUES ('Administrator', 'admin@carwash.com', '0780000000', $hash, 'Admin')";
                insert.Parameters.AddWithValue("$hash", PasswordHasher.Hash("Admin@123"));
                insert.ExecuteNonQuery();
            }

            check.CommandText = "SELECT COUNT(*) FROM ServiceTypes";
            if (Convert.ToInt32(check.ExecuteScalar()) == 0)
            {
                using var insert = connection.CreateCommand();
                insert.CommandText = @"
INSERT INTO ServiceTypes (Name, Description, Price, DurationMinutes) VALUES
('Basic', 'Exterior wash and dry', 3000, 30),
('Standard', 'Exterior wash, interior vacuum and windows', 5000, 45),
('Premium', 'Full wash, interior cleaning, wax and tyre shine', 10000, 60);";
                insert.ExecuteNonQuery();
            }
        }
    }
}

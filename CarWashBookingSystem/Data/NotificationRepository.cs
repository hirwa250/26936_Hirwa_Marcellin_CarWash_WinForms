using CarWashBookingSystem.Models;
using Microsoft.Data.Sqlite;

namespace CarWashBookingSystem.Data
{
    public class NotificationRepository
    {
        public static void Insert(SqliteConnection connection, SqliteTransaction transaction, int userId, string message)
        {
            using var command = DbHelper.Command(connection, "INSERT INTO Notifications (UserId, Message, IsRead, CreatedAt) VALUES ($u, $m, 0, $c)",
                ("$u", userId), ("$m", message), ("$c", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        public List<Notification> GetForUser(int userId, bool unreadOnly)
        {
            return DbHelper.List("SELECT Id, UserId, Message, IsRead, CreatedAt FROM Notifications WHERE UserId = $u AND ($o = 0 OR IsRead = 0) ORDER BY Id DESC",
                r => new Notification
                {
                    Id = r.GetInt32(0),
                    UserId = r.GetInt32(1),
                    Message = r.GetString(2),
                    IsRead = r.GetInt32(3) == 1,
                    CreatedAt = DateTime.Parse(r.GetString(4))
                }, ("$u", userId), ("$o", unreadOnly ? 1 : 0));
        }

        public void MarkAllRead(int userId)
        {
            DbHelper.Execute("UPDATE Notifications SET IsRead = 1 WHERE UserId = $u", ("$u", userId));
        }
    }
}

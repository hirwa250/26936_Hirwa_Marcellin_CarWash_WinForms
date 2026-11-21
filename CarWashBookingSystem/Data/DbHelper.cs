using System.Data;
using Microsoft.Data.Sqlite;

namespace CarWashBookingSystem.Data
{
    public static class DbHelper
    {
        public static SqliteCommand Command(SqliteConnection connection, string sql, params (string, object)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var (name, value) in parameters)
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            return command;
        }

        public static int Execute(string sql, params (string, object)[] parameters)
        {
            using var connection = Database.Open();
            using var command = Command(connection, sql, parameters);
            return command.ExecuteNonQuery();
        }

        public static object Scalar(string sql, params (string, object)[] parameters)
        {
            using var connection = Database.Open();
            using var command = Command(connection, sql, parameters);
            return command.ExecuteScalar();
        }

        public static DataTable Table(string sql, params (string, object)[] parameters)
        {
            using var connection = Database.Open();
            using var command = Command(connection, sql, parameters);
            using var reader = command.ExecuteReader();
            var table = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
                table.Columns.Add(reader.GetName(i), typeof(object));
            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                table.Rows.Add(values);
            }
            return table;
        }

        public static List<T> List<T>(string sql, Func<SqliteDataReader, T> map, params (string, object)[] parameters)
        {
            using var connection = Database.Open();
            using var command = Command(connection, sql, parameters);
            using var reader = command.ExecuteReader();
            var items = new List<T>();
            while (reader.Read())
                items.Add(map(reader));
            return items;
        }
    }
}

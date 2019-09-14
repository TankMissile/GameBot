using System;
using System.Linq;

using Npgsql;

namespace GameBot.Modules.DB
{
    abstract class DBTable
    {
        protected static readonly string dbString;

        public int Id { get; protected set; } = -1;

        static DBTable()
        {
            string host = Environment.GetEnvironmentVariable("DB_Host") ?? "localhost";
            string username = Environment.GetEnvironmentVariable("DB_Username") ?? "gamebot";
            string password = Environment.GetEnvironmentVariable("DB_Password") ?? "gamebot";
            string database = Environment.GetEnvironmentVariable("DB_Database") ?? "gamebot";

            dbString = $"Host={host};Username={username};Password={password};Database={database}";
        }

        protected void Save(string tablename)
        {
            var properties = GetType().GetProperties();
            Console.WriteLine(string.Join(", ", properties.Select(x => x.ToString())));
            using (var conn = new NpgsqlConnection(dbString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    if (this.Id >= 0) // Has been saved, update in database
                    {
                        string values = string.Join(", ", properties.Where(x => x.Name != "Id").Select(x => $"{x.Name} = @{x.Name}"));
                        cmd.CommandText = $"UPDATE {tablename} SET {values} WHERE id = @id";
                        Console.WriteLine(cmd.CommandText);
                        foreach (var prop in properties)
                        {
                            cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(this));
                        }
                        cmd.ExecuteNonQuery();
                    }
                    else if (this.Id < 0) // Hasn't been saved, insert into database
                    {
                        var filteredProps = properties.Where(x => x.Name != "Id");
                        string columns = string.Join(", ", filteredProps.Select(x => x.Name));
                        string values = string.Join(", ", filteredProps.Select(x => $"@{x.Name}"));
                        cmd.CommandText = $"INSERT INTO {tablename} ({columns}) VALUES ({values}) RETURNING Id";
                        Console.WriteLine(cmd.CommandText);
                        foreach (var prop in filteredProps)
                        {
                            cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(this));
                        }
                        this.Id = (int)cmd.ExecuteScalar();
                    }
                }
            }
        }
    }
}

using Npgsql;
using System.Collections.Generic;

namespace GameBot.Modules.DB
{
    class Rip : DBTable
    {
        private static readonly string TABLENAME = "rips";
        public string Value { get; set; }
        public string Creator { get; set; }

        public Rip(string rip, string creator)
        {
            Value = rip;
            Creator = creator;
        }

        private Rip(int id, string rip, string creator)
        {
            Id = id;
            Value = rip;
            Creator = creator;
        }

        override public string ToString()
        {
            return $"{Id}: {Value}";
        }

        public static List<Rip> GetAll()
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} ORDER BY id ASC";

                    List<Rip> rips = new List<Rip>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rips.Add(new Rip((int)reader["id"], (string)reader["value"], (string)reader["creator"]));
                        }
                    }

                    return rips;
                }
            }
        }

        public static List<Rip> GetByCreator(string creator)
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} WHERE Creator = @Creator ORDER BY id ASC";
                    cmd.Parameters.AddWithValue("Creator", creator);

                    List<Rip> rips = new List<Rip>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rips.Add(new Rip((int)reader["id"], (string)reader["value"], (string)reader["creator"]));
                        }
                    }

                    return rips;
                }
            }
        }

        public static Rip GetById(int id)
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} WHERE id = @id";
                    cmd.Parameters.AddWithValue("id", id);
                    var result = cmd.ExecuteReader();
                    result.Read();
                    return new Rip((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static List<Rip> GetByIds(int[] ids)
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} WHERE id = ANY(@ids) ORDER BY id ASC";
                    cmd.Parameters.AddWithValue("ids", ids);
                    var result = cmd.ExecuteReader();
                    List<Rip> rips = new List<Rip>();
                    while (result.Read())
                    {
                        rips.Add(new Rip((int)result["id"], (string)result["value"], (string)result["creator"]));
                    }
                    return rips;
                }
            }
        }

        public static Rip GetRandom()
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} ORDER BY random() limit 1";
                    var result = cmd.ExecuteReader();
                    result.Read();
                    return new Rip((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static Rip GetRandomByCreator(string creator)
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} WHERE creator = @creator ORDER BY random() limit 1";
                    cmd.Parameters.AddWithValue("creator", creator);
                    var result = cmd.ExecuteReader();
                    result.Read();
                    return new Rip((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static void DeleteById(int id)
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"DELETE FROM {TABLENAME} WHERE Id = @id";
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteScalar();
                }
            }
        }

        public void Save()
        {
            Save(TABLENAME);
        }
    }
}

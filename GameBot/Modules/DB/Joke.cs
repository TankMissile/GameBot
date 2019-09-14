using System.Collections.Generic;
using Npgsql;

namespace GameBot.Modules.DB
{
    class Joke : DBTable
    {
        private static readonly string TABLENAME = "jokes";
        public string Value { get; set; }
        public string Creator { get; set; }

        public Joke(string joke, string creator)
        {
            Value = joke;
            Creator = creator;
        }

        private Joke(int id, string joke, string creator)
        {
            Id = id;
            Value = joke;
            Creator = creator;
        }

        override public string ToString()
        {
            return $"{Id}: {Value}";
        }

        public static List<Joke> GetAll()
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} ORDER BY id ASC";

                    List<Joke> jokes = new List<Joke>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jokes.Add(new Joke((int)reader["id"], (string)reader["value"], (string)reader["creator"]));
                        }
                    }

                    return jokes;
                }
            }
        }

        public static List<Joke> GetByCreator(string creator)
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} WHERE Creator = @Creator ORDER BY id ASC";
                    cmd.Parameters.AddWithValue("Creator", creator);

                    List<Joke> jokes = new List<Joke>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jokes.Add(new Joke((int)reader["id"], (string)reader["value"], (string)reader["creator"]));
                        }
                    }

                    return jokes;
                }
            }
        }

        public static Joke GetById(int id)
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
                    return new Joke((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static List<Joke> GetByIds(int[] ids)
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
                    List<Joke> jokes = new List<Joke>();
                    while (result.Read())
                    {
                        jokes.Add(new Joke((int)result["id"], (string)result["value"], (string)result["creator"]));
                    }
                    return jokes;
                }
            }
        }

        public static Joke GetRandom()
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
                    return new Joke((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static Joke GetRandomByCreator(string creator)
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
                    return new Joke((int)result["id"], (string)result["value"], (string)result["creator"]);
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

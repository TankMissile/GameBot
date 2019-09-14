using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBot.Modules.DB
{
    class Insult : DBTable
    {
        private static readonly string TABLENAME = "insults";
        public string Value { get; set; }
        public string Creator { get; set; }

        public Insult(string insult, string creator)
        {
            Value = insult;
            Creator = creator;
        }

        private Insult(int id, string insult, string creator)
        {
            Id = id;
            Value = insult;
            Creator = creator;
        }

        override public string ToString()
        {
            return $"{Id}: {Value}";
        }

        public static List<Insult> GetAll()
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} ORDER BY id ASC";

                    List<Insult> insults = new List<Insult>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            insults.Add(new Insult((int)reader["id"], (string)reader["value"], (string)reader["creator"]));
                        }
                    }

                    return insults;
                }
            }
        }

        public static List<Insult> GetByCreator(string creator)
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} WHERE Creator = @Creator ORDER BY id ASC";
                    cmd.Parameters.AddWithValue("Creator", creator);

                    List<Insult> insults = new List<Insult>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            insults.Add(new Insult((int)reader["id"], (string)reader["value"], (string)reader["creator"]));
                        }
                    }

                    return insults;
                }
            }
        }

        public static Insult GetById(int id)
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
                    return new Insult((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static List<Insult> GetByIds(int[] ids)
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
                    List<Insult> insults = new List<Insult>();
                    while (result.Read())
                    {
                        insults.Add(new Insult((int)result["id"], (string)result["value"], (string)result["creator"]));
                    }
                    return insults;
                }
            }
        }

        public static Insult GetRandom()
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
                    return new Insult((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static Insult GetRandomByCreator(string creator)
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
                    return new Insult((int)result["id"], (string)result["value"], (string)result["creator"]);
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

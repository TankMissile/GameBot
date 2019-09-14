using System.Collections.Generic;
using Npgsql;

namespace GameBot.Modules.DB
{
    class Food : DBTable
    {
        private static readonly string TABLENAME = "foods";
        public string Value { get; set; }
        public string Creator { get; set; }

        public Food(string food, string creator)
        {
            Value = food;
            Creator = creator;
        }

        private Food(int id, string food, string creator)
        {
            Id = id;
            Value = food;
            Creator = creator;
        }

        override public string ToString()
        {
            return $"{Id}: {Value}";
        }

        public static List<Food> GetAll()
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} ORDER BY id ASC";

                    List<Food> foods = new List<Food>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foods.Add(new Food((int)reader["id"], (string)reader["value"], (string)reader["creator"]));
                        }
                    }

                    return foods;
                }
            }
        }

        public static List<Food> GetByCreator(string creator)
        {
            using (var conn = new NpgsqlConnection(dbString))
            {
                using (var cmd = new NpgsqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {TABLENAME} WHERE Creator = @Creator ORDER BY id ASC";
                    cmd.Parameters.AddWithValue("Creator", creator);

                    List<Food> foods = new List<Food>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foods.Add(new Food((int)reader["id"], (string)reader["value"], (string)reader["creator"]));
                        }
                    }

                    return foods;
                }
            }
        }

        public static Food GetById(int id)
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
                    return new Food((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static List<Food> GetByIds(int[] ids)
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
                    List<Food> foods = new List<Food>();
                    while (result.Read())
                    {
                        foods.Add(new Food((int)result["id"], (string)result["value"], (string)result["creator"]));
                    }
                    return foods;
                }
            }
        }

        public static Food GetRandom()
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
                    return new Food((int)result["id"], (string)result["value"], (string)result["creator"]);
                }
            }
        }

        public static Food GetRandomByCreator(string creator)
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
                    return new Food((int)result["id"], (string)result["value"], (string)result["creator"]);
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

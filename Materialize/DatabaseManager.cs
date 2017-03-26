using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Materialize
{
    /// <summary>
    /// Creates and manages the sqlite database
    /// </summary>
    static class DatabaseManager
    {
        const string DATABASE_NAME = "materialize.db";
        const int MAX_PATH_CHARS = 260;

        const string SEARCH_DIRECTORIES_NAME = "searchDirectories";

        static SQLiteConnection conn;

        /// <summary>
        /// initializes some database goodies that we'll need for this whole class to work
        /// </summary>
        static DatabaseManager()
        {
            // establish a connection to the sqlite database
            conn = new SQLiteConnection($"Data Source={DATABASE_NAME};Version=3");

            // create our database file if it does not yet exist
            if (!File.Exists(DATABASE_NAME))
            {
                SetupDatabase();
            }
        }

        /// <summary>
        /// Sets up the database and tables needed to function
        /// </summary>
        static void SetupDatabase()
        {
            conn.Open();
            string sql = $"CREATE TABLE {SEARCH_DIRECTORIES_NAME} (directory VARCHAR({MAX_PATH_CHARS}))";
            var command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQueryAsync();
            conn.Close();
        }

        /// <summary>
        /// Just a way to get our static initializer to do stuff before it's referenced elsewhere
        /// </summary>
        public static void Poke()
        {

        }

        /// <summary>
        /// Saves the 
        /// </summary>
        /// <param name="directories"></param>
        public static void SetDirectorySearchPaths(string[] directories)
        {
            conn.Open();
            using (var command = new SQLiteCommand(conn))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    // just clear the table - it's easier to start over than loop over stuff and see what's different
                    command.CommandText = $"DELETE FROM {SEARCH_DIRECTORIES_NAME}";
                    command.ExecuteNonQuery();

                    for (var i = 0; i < directories.Length; i++)
                    {
                        // populate new entries into our database
                        command.CommandText =
                            $"INSERT INTO {SEARCH_DIRECTORIES_NAME} (directory) VALUES ($val)";
                        command.Parameters.AddWithValue("$val", directories[i]);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            conn.Close();
        }

        /// <summary>
        /// Get the saved directories settings
        /// </summary>
        /// <returns></returns>
        public static string[] GetDirectorySearchPaths()
        {
            List<string> directories = new List<string>();

            conn.Open();
            var sql = $"SELECT * FROM {SEARCH_DIRECTORIES_NAME}";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        directories.Add(reader.GetString(0));
                    }
                }
            }
            conn.Close();

            return directories.ToArray();
        }
    }
}

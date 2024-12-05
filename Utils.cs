using ChildDailyTaskApp.Models;
using Microsoft.Data.SqlClient;
using System.IO;

namespace ChildDailyTaskApp
{
    internal class Utils
    {
        public static bool HasCompletedToday()
        {
            if (Program.AppConfiguration != null)
            {
                if (Program.AppConfiguration.UseSqlServer)
                {
                    using SqlConnection conn = new(Program.AppConfiguration.ConnectionStrings.ChildTaskDb);
                    conn.Open();
                    string query = Constants.SQL_SELECT_QUERY;
                    using SqlCommand command = new(query, conn);
                    return (int)command.ExecuteScalar() > 0;
                }
                else
                {
                    // build and read from store.edb
                    DataStore dataStore = GetDataStore();

                    DateTime? parsedLastConfirmedDate = dataStore.ParseLastConfirmedDate();
                    return parsedLastConfirmedDate != null && parsedLastConfirmedDate.Value.Date == DateTime.Today.Date;
                }
                
            }

            return false;
            
        }

        public static int? PersistConfirmationToDatabase()
        {
            if (Program.AppConfiguration != null)
            {
                if (Program.AppConfiguration.UseSqlServer)
                {
                    using SqlConnection conn = new(Program.AppConfiguration.ConnectionStrings.ChildTaskDb);
                    if (!HasCompletedToday())
                    {
                        conn.Open();
                        var query = Constants.SQL_UPDATE_QUERY;
                        using SqlCommand command = new(query, conn);
                        command.Parameters.AddWithValue("dateCompleted", DateTime.Now.Date);
                        return command.ExecuteNonQuery();

                    }

                    return null;
                } else
                {
                    if (!HasCompletedToday())
                    {
                        if (File.Exists(Constants.STOREDB_PATH))
                        {
                            List<string> storeDbLines = File.ReadLines(Constants.STOREDB_PATH).ToList();
                            for (int i = 0; i < storeDbLines.Count; i++)
                            {
                                string line = storeDbLines[i];
                                bool isComment = line.StartsWith('#');
                                string key = line.Split('=').ToList().First();
                                string[] keyValueItem = new string[2];
                                if (!isComment)
                                {
                                    if (key.Length > 0)
                                    {
                                        string cleanKey = key.Replace("[", "").Replace("]", "");
                                        if (cleanKey.Equals("LastConfirmedDate"))
                                        {
                                            keyValueItem[0] = key;
                                            keyValueItem[1] = DateTime.Now.Date.ToString();

                                            storeDbLines[i] = string.Join("=", keyValueItem);
                                        }

                                    }

                                }
                            }

                            File.WriteAllLines(Constants.STOREDB_PATH, storeDbLines);
                            return 1;
                        }
                    }
                }
                
            }

            return null;
            
        }

        public static DataStore GetDataStore()
        {
            List<string> storeDbLines = File.ReadLines(Constants.STOREDB_PATH).ToList();
            DataStore dataStore = new();
            storeDbLines.ForEach(line =>
            {
                bool isComment = line.StartsWith('#');
                string[] keyValueItem = line.Split('=');
                if (!isComment)
                {
                    if (keyValueItem.Length == 2)
                    {
                        string key = keyValueItem[0].Replace("[", "").Replace("]", "");
                        string value = keyValueItem[1];

                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            System.Reflection.PropertyInfo? prop = typeof(DataStore).GetProperty(key);
                            prop?.SetValue(dataStore, value);
                        }

                    }

                }
            });

            return dataStore;
        }
    }
}

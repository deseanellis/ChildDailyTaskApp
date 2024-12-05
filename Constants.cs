using System.IO;

namespace ChildDailyTaskApp
{
    public class Constants
    {
        public static readonly string STOREDB_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "store.edb");
        public const string SQL_SELECT_QUERY = "SELECT COUNT(*) FROM TaskCompletion WHERE DateCompleted = CAST(GETDATE() AS DATE)";
        public const string SQL_UPDATE_QUERY = "INSERT INTO TaskCompletion (DateCompleted, IsCompleted) VALUES (@dateCompleted, 1)";

    }
}

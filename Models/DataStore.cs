namespace ChildDailyTaskApp.Models
{
    public class DataStore
    {
        public string? LastConfirmedDate { get; set; }
        public string? Password { get; set; }

        public DateTime? ParseLastConfirmedDate()
        {
            if (LastConfirmedDate == null) return null;

            if (DateTime.TryParse(LastConfirmedDate.ToString(), out DateTime lastConfirmedDate))
                return lastConfirmedDate;

            return null;
        }
    }
}

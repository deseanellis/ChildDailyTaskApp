namespace ChildDailyTaskApp.Models.Subs
{
    public class EmailSettings
    {
        public required string From { get; set; }
        public required string Password { get; set; }
        public required string Server { get; set; }
        public int Port { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(From) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Server) && Port > 0;
        }
    }
}

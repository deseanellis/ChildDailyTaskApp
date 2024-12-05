using ChildDailyTaskApp.Models.Subs;

namespace ChildDailyTaskApp.Models
{
    public class Configuration
    {
        public required bool UseSqlServer { get; set; }
        public required ConnectionStrings ConnectionStrings { get; set; }
        public required IEnumerable<string> EmailRecipients { get; set; }
        public required EmailSettings EmailSettings { get; set; }
        public required Child Child { get; set; }
        public required IEnumerable<string> Tasks { get; set; }
        public required int ExecutionIntervalInMinutes { get; set; }
        public required int PasswordAttemptThreshold {  get; set; }


    }
}

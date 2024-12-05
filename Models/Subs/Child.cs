namespace ChildDailyTaskApp.Models.Subs
{
    public class Child
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public string GetFullName () => FirstName + " " + LastName;
    }
}

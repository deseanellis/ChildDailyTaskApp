namespace ChildDailyTaskApp.Models
{
    public class CheckBoxOption(string content, bool isChecked)
    {
        public string Content { get; set; } = content;
        public bool IsChecked { get; set; } = isChecked;
    }
}

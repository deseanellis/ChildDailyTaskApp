using System.Windows;

namespace ChildDailyTaskApp
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public string ErrorMessage { get; set; }
        public ErrorWindow(string message)
        {
            InitializeComponent();
            ErrorMessage = "Error: " + message;
        }
    }
}

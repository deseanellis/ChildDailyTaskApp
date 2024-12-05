using ChildDailyTaskApp.Models;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;

namespace ChildDailyTaskApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _passwordAttempts = 0;
        public string TitleText { get; set; } = "Please confirm your tasks for the day:";
        public ObservableCollection<CheckBoxOption> CheckBoxItems { get; set; } = [];
        public ICommand CheckAllCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            if (Program.AppConfiguration != null)
            {
                List<CheckBoxOption> tasks = [];
                for (int i = 0; i < Program.AppConfiguration.Tasks.Count(); i++)
                {
                    string taskContent = Program.AppConfiguration.Tasks.ElementAt(i);
                    tasks.Add(new CheckBoxOption(taskContent, false));
                }

                CheckBoxItems = new ObservableCollection<CheckBoxOption>(tasks);
                TitleText = $"Good Day {Program.AppConfiguration.Child.GetFullName()}, please confirm your tasks for the day:";

            }

            CheckAllCommand = new RelayCommand(CheckAllCheckBoxes);
            DataContext = this;

        }

        private bool AreAllChecked()
        {
            return CheckBoxItems.All(item => item.IsChecked);
        }
        private void CheckAllCheckBoxes()
        {
            if (AreAllChecked())
            {
                MessageBox.Show("All tasks confirmed. You can proceed.");
                Utils.PersistConfirmationToDatabase(); // Add confirmation to the database
                SendTaskConfirmationEmail();


                Application.Current.Dispatcher.Invoke(Hide); // Close modal after confirmation
            }
            else
            {
                MessageBox.Show("Please complete all tasks.");
            }
        }

        private void SendTaskConfirmationEmail()
        {
            if (Program.AppConfiguration != null && Program.AppConfiguration.EmailSettings.IsValid())
            {
                try
                {
                    string body = $"<p>The following daily tasks have been confirmed by {Program.AppConfiguration.Child.GetFullName()}:</p>";
                    body += "<ol>";
                    for (int i = 0; i < CheckBoxItems.Count; i++)
                    {
                        body += $"<li>{CheckBoxItems[i].Content}</li>";
                    }
                    body += "</ol>";
                    MailMessage mail = new(Program.AppConfiguration.EmailSettings.From, string.Join(",", Program.AppConfiguration.EmailRecipients));
                    SmtpClient client = new(Program.AppConfiguration.EmailSettings.Server)
                    {
                        Port = Program.AppConfiguration.EmailSettings.Port,
                        Credentials = new System.Net.NetworkCredential(Program.AppConfiguration.EmailSettings.From, Program.AppConfiguration.EmailSettings.Password),
                        EnableSsl = true
                    };
                    mail.Subject = $"{Program.AppConfiguration.Child.GetFullName()} Daily Tasks Completed";
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending email: {ex.Message}. Show message to parent now.");
                }
            }
            
        }

        private void SendPasswordThresholdExceededEmail()
        {
            if (Program.AppConfiguration != null && Program.AppConfiguration.EmailSettings.IsValid())
            {
                try
                {
                    string body = $"<p>The system clear password for the Daily Task application has been entered incorrectly {_passwordAttempts} times by {Program.AppConfiguration.Child.GetFullName()}.</p>";
                    MailMessage mail = new(Program.AppConfiguration.EmailSettings.From, string.Join(",", Program.AppConfiguration.EmailRecipients));
                    SmtpClient client = new(Program.AppConfiguration.EmailSettings.Server)
                    {
                        Port = Program.AppConfiguration.EmailSettings.Port,
                        Credentials = new System.Net.NetworkCredential(Program.AppConfiguration.EmailSettings.From, Program.AppConfiguration.EmailSettings.Password),
                        EnableSsl = true
                    };
                    mail.Subject = $"Daily Task System Clear Attempts";
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending email: {ex.Message}. Show message to parent now.");
                }
            }
        }
        private void Window_Closed( object sender, EventArgs e )
        {
            Application.Current.Shutdown();
        }

        private void SystemClearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataStore dataStore = Utils.GetDataStore();
                if (Program.AppConfiguration != null && !string.IsNullOrWhiteSpace(dataStore.Password))
                {
                    // check if password if correct
                    if (Password.Password.Trim().Equals(dataStore.Password))
                    {
                        if (Program.Scheduler != null && Program.Scheduler.IsStarted)
                            Program.Scheduler.Shutdown();

                        Program.RemoveFromStartup();
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBox.Show($"Password is incorrect.");
                        _passwordAttempts += 1;
                    }

                    if (_passwordAttempts > 0 && _passwordAttempts % Program.AppConfiguration.PasswordAttemptThreshold == 0)
                        SendPasswordThresholdExceededEmail();
                    
                    
                }
                else
                {
                    MessageBox.Show($"Error: Application configuration not loaded.");
                }
            }
            catch (Exception ex )
            {
                MessageBox.Show($"Error clearing/removing application: {ex.Message}.");
            }
            
            
        }
    }
}
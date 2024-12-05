using Quartz;
using System.Windows;

namespace ChildDailyTaskApp.Jobs
{
    public class LaunchModal : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (!Utils.HasCompletedToday())
                {
                        Application app = Application.Current ?? throw new InvalidOperationException("The WPF Application instance is not initialized.");
                        app.Dispatcher.Invoke(() =>
                        {
                            MainWindow mainWindow = new();
                            mainWindow.Show();
                        });
                }
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex, refireImmediately: true)
                {
                    UnscheduleFiringTrigger = true,
                    UnscheduleAllTriggers = true
                };
            }

            return Task.CompletedTask;
        }
    }
}

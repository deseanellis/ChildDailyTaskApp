using ChildDailyTaskApp.Models;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using Quartz;
using ChildDailyTaskApp.Jobs;
using System.Diagnostics;

namespace ChildDailyTaskApp
{
    public class Program
    {
        public static Configuration? AppConfiguration { get; } = GetConfiguration();
        public static IScheduler? Scheduler { get; set; }

        [STAThread]
        public static void Main()
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            var processes = Process.GetProcessesByName(processName);
            bool isAlreadyRunning = Process.GetProcessesByName(processName).Where(p => p.Id != Environment.ProcessId).Any();

            if (isAlreadyRunning)
            {
                MessageBox.Show("The application is already running.", "Instance Running", MessageBoxButton.OK, MessageBoxImage.Information);
            } else
            {
                Application app = new();

                if (AppConfiguration != null)
                {
                    SchedulerBuilder scheduleBuilder = SchedulerBuilder.Create();

                    if (AppConfiguration.UseSqlServer)
                    {
                        // get scheduler
                        scheduleBuilder.UsePersistentStore(x =>
                        {
                            x.UseProperties = true;
                            x.UseClustering();
                            x.UseSqlServer(AppConfiguration.ConnectionStrings.SqlQuartzDb);
                            x.UseSystemTextJsonSerializer();
                        });

                        Scheduler = Task.Run(scheduleBuilder.BuildScheduler).GetAwaiter().GetResult();

                    }
                    else
                    {
                        Scheduler = Task.Run(scheduleBuilder.BuildScheduler).GetAwaiter().GetResult();
                    }

                    // start scheduler
                    Task.Run(() => Scheduler.Start()).GetAwaiter().GetResult();

                    IJobDetail job = JobBuilder.Create<LaunchModal>()
                     .WithIdentity("child-daily-task-reminder-job")
                     .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity("child-daily-task-reminder-trigger")
                     .StartNow()
                     .WithSimpleSchedule(x => x
                      .WithIntervalInMinutes(AppConfiguration.ExecutionIntervalInMinutes)
                      .RepeatForever())
                     .Build();

                    // add app to windows startup programs
                    AddToStartup();

                    // run scheduled job
                    Task.Run(() => Scheduler.ScheduleJob(job, trigger)).GetAwaiter().GetResult();

                    // start app
                    app.Run();

                }
                else
                {
                    //run error window
                    app.Run(new ErrorWindow("Application configuration could not be found and are not loaded."));
                }
            }
            
            
        }

        public static void AddToStartup()
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            key?.SetValue("ChildDailyTaskApp", System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("dll", "exe"));

        }

        public static void RemoveFromStartup()
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            key?.DeleteValue("ChildDailyTaskApp");

        }

        public static Configuration? GetConfiguration()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<Configuration>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}

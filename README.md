## Child Daily Task App

Create a checklist of tasks for your child to complete before they are allowed to use their PCs. This application uses background scheduling tasks via Quartz.NET to execute a modal window for your child to check-off the pre-defined tasks you've created. 

### How-to-Use

**Download**

The application is package as a release to this repository.

**Application Configuration**

The application can be configured using the “config.json” file located in the application root folder. The configurable options are as follows:

1.  _UseSqlServer_ \- This allows you to use MSSQL to store scheduling data (see Quartz documentation) and the database to log checklist confirmations. The **CREATE TABLE** script is provided in this repository under the _“SqlScripts”_ folder.
2.  _ConnectionStrings_ \- These are the MSSQL connection strings for the Quartz and checklist confirmation databases.
3.  _EmailRecipients_ \- An array of email address. This will be used to send notifications.
4.  _EmailSettings_ \- The SMTP settings required for sending emails.
5.  _Child_ \- The name of the child.
6.  _Tasks_ \- An array for tasks that the child must complete before getting access to the PC.
7.  _ExecutionIntervalInMinutes_ \- The number of minutes, in terms of frequency, that the modal will reappear on the child's screen, if confirmation of the checklist has not been received on the day. Once the checklist is confirmed, the application will not show again within the confirmed day.
8.  _PasswordAttemptThreshold_ \- The number of attempts allowed before notification of excessive attempts will be sent to the _“EmailRecipients”_.

**Store Configuration**

1.  The system password is stored within a file titled “store.edb” location in the application root folder. This password is used to stop the scheduler and remove the application from the Startup Apps for Windows 11, thus killing the service.

---

**This application has been tested on Windows 11 Pro 23H2, however, it should still work with various versions of Windows 10.**
using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using ActivityMonitor.ApplicationMonitor;
using ActMon.Database;
using ActMon.Forms;
using ActMon.Properties;
using Microsoft.Win32;

namespace ActMon
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        private static AppTrayIconContext TrayIcon;
        
        [STAThread]
        static void Main()
        {
            //Check if running other instances of application for the same user
            String mutexName = "ActMon" + System.Security.Principal.WindowsIdentity.GetCurrent().User.AccountDomainSid;
            Boolean createdNew;            

            Mutex mutex = new Mutex(true, mutexName, out createdNew);

            if (createdNew)
            {
                //SessionClose Handling
                SystemEvents.SessionEnding += new SessionEndingEventHandler(AppExit);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                TrayIcon = new AppTrayIconContext();
                Application.Run(TrayIcon);
                Application.ApplicationExit += new System.EventHandler(AppExit);
            }
        }
        static void AppExit(object sender, EventArgs e)
        {
            TrayIcon.GracefulExit();
        }
    }


    public class AppTrayIconContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private UserSession usrSession;
        private AppMonitor appMon;
        private DataDumper DBDumper;
        private DB Database;
        private SettingsManager.Settings AppSettings;
        
        //TODO: Refactor this
        private bool _dialogActive;
        private bool _registerActivityActive;
        private bool _settingsActive;

        private FormActivity fStats;
        
        public AppTrayIconContext()
        {
            //Initialize Objects
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            AppSettings = new SettingsManager.Settings();

            // Check if Runs Hidden
            if (!AppSettings.RunHidden)
            {
                //Build Menu
                ContextMenu mnu = new ContextMenu();

                mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_Activity, OpenStats));

                //if (!AppSettings.LockSettings)
                //    mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_Settings, OpenSettings));

                if (!AppSettings.HideMenuExit)
                    mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_RegisterActivity, OpenRegisterActivity));
                    mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_Exit, Exit));

                    //mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_About, About));


                trayIcon = new NotifyIcon()
                {
                    Icon = Resources.ClockIcon,
                    ContextMenu = mnu,
                    Visible = true
                };

                trayIcon.DoubleClick += new System.EventHandler(OpenStats);
                AppDomain.CurrentDomain.ProcessExit += new System.EventHandler(Exit);
            }

            usrSession = new UserSession();
            appMon = new AppMonitor();

            Database = new DB();

            DBDumper = new DataDumper(appMon, Database, AppSettings.DBDumprate);

            appMon.Start();

            //TODO: Start DB dumper anyway
            if (setDBConfig())
                DBDumper.Start();
        }

        private bool setDBConfig()
        {
            Database.Server = AppSettings.DBServer;
            Database.Database = AppSettings.DBDatabase;
            Database.Username = AppSettings.DBUsername;
            Database.Password = AppSettings.DBPassword;

            return Database.Connect();
        }
        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            GracefulExit();
        }
        public void GracefulExit()
        {
            trayIcon.Visible = false;
            appMon.EndSession();
            DBDumper.Stop();
            Application.Exit();
        }

        void OpenSettings(Object sender, EventArgs e)
        {
            if (!_settingsActive) { 
                Form fs = new Forms.FormSettings(AppSettings);
                _settingsActive = true;
                fs.ShowDialog();
                fs.Dispose();
                _settingsActive = false;
            }
        }
        void OpenRegisterActivity(Object sender, EventArgs e)
        {
            if (!_registerActivityActive)
            {
                FormRegisterActivity fAbout = new FormRegisterActivity(appMon);
                _registerActivityActive = true;
                fAbout.ShowDialog();
                _registerActivityActive = false;
            }
        }
        void OpenStats(Object sender, EventArgs e)
        {
            if (!_dialogActive) { 
                fStats = new Forms.FormActivity(appMon);
                _dialogActive = true;
                fStats.ShowDialog();
                fStats.Dispose();
                _dialogActive = false;
            } else
            {
                fStats.Restore();
            }
        }
    }

}

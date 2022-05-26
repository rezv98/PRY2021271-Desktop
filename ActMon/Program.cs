using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using ActivityMonitor.ApplicationMonitor;
using ActivityMonitor.ApplicationImp.HistoryModels;
using ActMon.Database;
using ActMon.Forms;
using ActMon.Properties;
using Microsoft.Win32;
using System.Collections.Generic;
using ActivityMonitor.ApplicationImp;

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
        private bool _historyActive;
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

                //mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_Activity, OpenStats));

                //if (!AppSettings.LockSettings)
                //    mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_Settings, OpenSettings));

                if (!AppSettings.HideMenuExit)
                {
                    mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_RegisterActivity, OpenRegisterActivity));
                    //mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_History, OpenHistory));
                    mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_Exit, Exit));

                    //mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_About, About));
                }


                trayIcon = new NotifyIcon()
                {
                    Icon = Resources.ClockIcon,
                    ContextMenu = mnu,
                    Visible = true
                };

                trayIcon.DoubleClick += new System.EventHandler(OpenRegisterActivity);
                AppDomain.CurrentDomain.ProcessExit += new System.EventHandler(Exit);
            }

            usrSession = new UserSession();
            appMon = new AppMonitor();

            Database = new DB();

            DBDumper = new DataDumper(appMon, Database, AppSettings.DBDumprate);

            appMon.Start();

            if (DateTime.Now.Hour >= Global.closeTimeHour)
            {
                GracefulExit();
            }

            //TODO: Start DB dumper anyway
            if (setDBConfig())
                DBDumper.Start();
        }

        private void GetHistory()
        {
            //Obtener de chrome
            ChromeHistory chrome = new ChromeHistory();

            var browserList = new List<Browser>
            {
                new Browser() { Name = "Chrome", DataTable = chrome.GetDataTable() },

            };

            foreach (var browser in browserList)
            {
                if (browser.DataTable != null)
                {
                    // rows that contains all information about the browser history
                    foreach (dynamic row in browser.DataTable.Rows)
                    {
                        var request = new UrlRequest
                        {
                            Browser = browser.Name,
                            Url = row[0],
                            Title = row[1],
                            Time = row[2],
                            Date = row[3]
                        };
                    }
                }
            }
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
            if (Global.responseUserId == 12)
            {
                GracefulExit();
            }
            else
            {
                MessageBox.Show("Your account doesn't have permissions to close the application.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
                FormRegisterActivity formRegisterActivity = new FormRegisterActivity(appMon);
                _registerActivityActive = true;
                formRegisterActivity.ShowDialog();
                _registerActivityActive = false;
            }
        }
        
        void OpenHistory(Object sender, EventArgs e)
        {
            if (!_historyActive)
            {
                FormHistory formHistory = new FormHistory();
                _historyActive = true;
                formHistory.ShowDialog();
                _historyActive = false;
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

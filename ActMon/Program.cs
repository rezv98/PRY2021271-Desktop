using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using ActivityMonitor.ApplicationMonitor;
using ActivityMonitor.ApplicationImp.HistoryModels;
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
        private SettingsManager.Settings AppSettings;
        
        private bool _registerActivityActive;
        
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

                if (!AppSettings.HideMenuExit)
                {
                    mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_RegisterActivity, OpenRegisterActivity));
                    mnu.MenuItems.Add(new MenuItem(ResFiles.GlobalRes.traymenu_Exit, Exit));
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

            appMon.Start();

            if (DateTime.Now.Hour >= Global.closeTimeHour)
            {
                GracefulExit();
            }
        }

        void Exit(object sender, EventArgs e)
        {
            if (Global.responseUserId == 0) //Cambiar por 1
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
            Application.Exit();
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
    }
}

using System;
using System.Windows.Forms;
using ActMon.SettingsManager;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ActivityMonitor.ApplicationImp;
using Newtonsoft.Json;
using System.Text;
using ActivityMonitor.Application;
using ActivityMonitor.ApplicationMonitor;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using ActivityMonitor.ApplicationImp.HistoryModels;
using ActMon.Services;
using Azure.Storage.Blobs;
using System.IO;
using ActivityMonitor.ApplicationImp.ScreenshotModels;

namespace ActMon.Forms
{
    public partial class FormRegisterActivity : Form
    {
        public AppMonitor _appMon;
        public FormRegisterActivity(AppMonitor AppMonitor)
        {
            InitializeComponent();
            _appMon = AppMonitor;
        }

        private async void btnOK_Click(object sender, EventArgs e)
        {
            await Login(emailText.Text, passwordText.Text);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public async Task Login(string userEmail, string userPassword)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Global.apiUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var loginInfo = new { email = userEmail, password = userPassword };

            var json = JsonConvert.SerializeObject(loginInfo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("user/authenticate", content);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Invalid Email or Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(result);
                Global.responseToken = loginResponse.Token;
                Global.responseUserId = loginResponse.Id;
                MessageBox.Show("You have logged in successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

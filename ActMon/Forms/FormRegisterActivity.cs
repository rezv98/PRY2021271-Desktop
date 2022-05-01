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
            client.BaseAddress = new Uri("https://montracapi20220413154050.azurewebsites.net/api/");
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
                try
                {
                    await CreateRegistry(loginResponse.Token, loginResponse.Id);
                    await SendHistory(loginResponse.Token, loginResponse.Id);
                    MessageBox.Show("Your activities has been registered successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("An error occurred while registering your activities.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public async Task CreateRegistry(string responseToken, int responseUserId)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://montracapi20220413154050.azurewebsites.net/api/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseToken);

            try
            {
                foreach (ActivityMonitor.Application.Application lApp in _appMon.Applications)
                {
                    var nombre = lApp.Name;
                    var totalTime = lApp.TotalUsageTime.Minutes;
                    var usages = lApp.Usage;
                    var startTime = usages[0].BeginTime.ToUniversalTime();
                    var size = usages.Count;
                    var endTime = usages.Last().EndTime.ToUniversalTime();
                    if (nombre == "Activity Monitor")
                    {
                        endTime = DateTime.UtcNow;
                    }

                    if (totalTime < 1)
                    {
                        totalTime = 1;
                    }
                    var body = new { 
                        description = nombre,
                        startDate = startTime, 
                        endDate = endTime,
                        timeUsed = totalTime, 
                        userId = responseUserId 
                    };

                    //Console.WriteLine(body);
                    var json = JsonConvert.SerializeObject(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("program", content);
                    var result = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task SendHistory(string responseToken, int responseUserId)
        {
            //Browsers
            ChromeHistory chrome = new ChromeHistory();
            OperaHistory opera = new OperaHistory();
            UrlService urlService = new UrlService();

            var browserList = new List<Browser>
            {
                new Browser() { Name = "Chrome", DataTable = chrome.GetDataTable() },
                new Browser() { Name = "Opera", DataTable = opera.GetDataTable() }
            };

            foreach (var browser in browserList)
            {
                if (browser.DataTable != null)
                {
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
                        try
                        {
                            await urlService.SendUrl(request, responseToken, responseUserId);
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
    }
}

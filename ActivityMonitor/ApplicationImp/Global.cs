using ActivityMonitor.Application;
using ActivityMonitor.ApplicationImp.HistoryModels;
using ActivityMonitor.ApplicationImp.ScreenshotModels;
using ActivityMonitor.ApplicationMonitor;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace ActivityMonitor.ApplicationImp
{
    static public class Global
    {
        static public string responseToken = "";
        static public int responseUserId = 0;
        static public string apiUrl = "https://montracapi1.azurewebsites.net/api/";

        static public int screenshotTimer = 295; //295
        static public int infoSenderTimer = 600; //600
        static public int closeTimeHour = 18;

        static public int checkInternetTimer = 5;
        static public bool connectedToInternet = true;

        static public string productivityPercentage = "";

        static public void TakeScreenshot()
        {
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            string filename = String.Format("file{0}-{1}.jpg", Date, DateTime.Now.Ticks);
            string directory = "C:\\IMAGENES";
            if (!Directory.Exists(directory))
            {
                DirectoryInfo di = Directory.CreateDirectory(directory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            string path = Path.Combine(directory, filename);
            var image = ScreenCapture.CaptureDesktop();
            image.Save(path, ImageFormat.Jpeg);
        }

        static public async Task CreateRegistry(string responseToken, int responseUserId, Applications apps)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(Global.apiUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseToken);

            try
            {
                foreach (ActivityMonitor.Application.Application lApp in apps)
                {
                    var nombre = lApp.Name;
                    var totalTime = lApp.TotalUsageTime.Minutes;
                    var usages = lApp.Usage;
                    var startTime = usages[0].BeginTime.ToUniversalTime();
                    var size = usages.Count;
                    var endTime = usages.Last().EndTime.ToUniversalTime();
                    if (endTime.Year == 1)
                    {
                        endTime = DateTime.UtcNow;
                    }

                    if (totalTime < 1)
                    {
                        totalTime = 1;
                    }
                    var body = new
                    {
                        description = nombre,
                        startDate = startTime,
                        endDate = endTime,
                        timeUsed = totalTime,
                        userId = responseUserId
                    };

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

        static public async Task SendHistory(string responseToken, int responseUserId)
        {
            //Browsers
            ChromeHistory chrome = new ChromeHistory();

            chrome.GetDataTable();

            string source = @"Data Source=C:\TempHistory\History;Version=3;New=False;Compress=True;";

            SQLiteDataAdapter sqlDataAdapter;
            DataTable dt = new DataTable();

            using (SQLiteConnection sqlConnection = new SQLiteConnection(source))
            {
                try
                {
                    sqlConnection.Open();
                    sqlConnection.CreateCommand();

                    sqlDataAdapter = new SQLiteDataAdapter(chrome.query, sqlConnection);
                    sqlDataAdapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            Browser browser = new Browser() { Name = "Chrome", Data = dt };

            foreach (dynamic row in browser.Data.Rows)
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
                    await UrlServiceRequest(request, responseToken, responseUserId);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            }
        }

        static public async Task SendScreenshot(string responseToken, int responseUserId)
        {
            string pathImg = @"C:\IMAGENES";
            var directory = Directory.GetFiles(pathImg, "*.*", SearchOption.AllDirectories);

            foreach (var path in directory)
            {
                try
                {
                    var filename = path.Split('\\')[2];
                    var blob = await UploadFile(filename, path);

                    if (blob != null)
                    {
                        var request = new ScreenshotRequest()
                        {
                            Date = DateTime.Now.ToUniversalTime(),
                            Name = filename,
                            Blob = blob.ToString()
                        };
                        //var urlService = new Screenshot();
                        await ScreenShotServiceRequest(request, responseToken, responseUserId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static public async Task<string> UploadFile(string filename, string path)
        {
            var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=blobmontracstorage1;AccountKey=t88W6V6GNK8Xyvt43hWYn2byZOOZ6v9SyI6OYraqkQxmEKKcmmar4NINdMMinHsOXXS8Ny5oRhAT+AStRf+LHw==;EndpointSuffix=core.windows.net";
            var blobStorageContainerName = "fileupload";
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);

            var blob = container.GetBlobClient(filename);
            var stream = File.OpenRead(path);
            try
            {
                await blob.UploadAsync(stream);
                return blob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        static public async Task<dynamic> UrlServiceRequest(UrlRequest request, string token, int userId)
        {
            try
            {
                var path = "url";
                request.UserId = userId;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(path, content);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        static public async Task<dynamic> ScreenShotServiceRequest(ScreenshotRequest request, string token, int userId)
        {
            try
            {
                var path = "screenshot";
                request.UserId = userId;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(path, content);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        static public bool IsConnectedToInternet()
        {
            string host = "google.com";
            bool result = false;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                {
                    connectedToInternet = true;
                    return true;
                }                    
            }
            catch { }
            connectedToInternet = false;
            return result;
        }

        static public void TestApps(Applications apps)
        {
            foreach (ActivityMonitor.Application.Application lApp in apps)
            {
                var nombre = lApp.Name;
                var totalTime = lApp.TotalUsageTime.Minutes;
                var usages = lApp.Usage;
                var startTime = usages[0].BeginTime.ToUniversalTime();
                var size = usages.Count;
                var endTime = usages.Last().EndTime.ToUniversalTime();
                if (endTime.Year == 1)
                {
                    endTime = DateTime.UtcNow;
                }

                if (totalTime < 1)
                {
                    totalTime = 1;
                }
                var body = new
                {
                    description = nombre,
                    startDate = startTime,
                    endDate = endTime,
                    timeUsed = totalTime,
                    userId = responseUserId
                };
                Console.WriteLine(body);
            }
        }
    }
}

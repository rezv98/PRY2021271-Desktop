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
using System.Windows.Forms;

namespace ActivityMonitor.ApplicationImp
{
    static public class Global
    {
        static public string responseToken = "";
        static public int responseUserId = 0;
        static public string apiUrl = "https://montracapi20220507053436.azurewebsites.net/api/";

        static public int screenshotTimer = 30;
        static public int infoSenderTimer = 90;

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
                    if (nombre == "Activity Monitor")
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
            OperaHistory opera = new OperaHistory();
            //UrlService urlService = new UrlService();

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
                            await UrlServiceRequest(request, responseToken, responseUserId);
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        static public async Task SendScreenshot(string responseToken, int responseUserId)
        {
            var directory = Directory.GetFiles("C:\\IMAGENES", "*.*", SearchOption.AllDirectories);

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
            var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=montracblobstorage;AccountKey=38jbNkUcrrfKlFWiEWBgVL3LuXRjumPkKQZZroJ7JSJo17TQgwbMDI3oOr5LwNMwrr9TxUtUtumD+AStBF/MNw==;EndpointSuffix=core.windows.net";
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
    }
}

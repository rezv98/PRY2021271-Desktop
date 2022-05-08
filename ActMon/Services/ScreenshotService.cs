using ActivityMonitor.ApplicationImp.ScreenshotModels;
using ActMon.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ActMon.Services
{
    public class Screenshot : IScreenshotService
    {
        public Screenshot()
        {
        }

        public async Task<dynamic> SendScreenshot(ScreenshotRequest request, string token, int userId)
        {
            try
            {
                var path = "screenshot";
                request.UserId = userId;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://montracapi20220413154050.azurewebsites.net/api/");
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

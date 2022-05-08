using ActivityMonitor.ApplicationImp;
using ActivityMonitor.ApplicationImp.HistoryModels;
using ActMon.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ActMon.Services
{
    public class UrlService : IUrlService
    {
        public UrlService()
        {
        }

        public async Task<dynamic> SendUrl(UrlRequest request, string token, int userId)
        {
            try
            {
                var path = "url";
                request.UserId = userId;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Global.apiUrl);
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

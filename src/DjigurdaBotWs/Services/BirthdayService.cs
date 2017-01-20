using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DjigurdaBotWs.Services
{
    public interface IBirthdayService
    {
        Task<List<string>> GetBirthdayBoysForTodayAsync();
    }

    public class BirthdayService : IBirthdayService
    {
        private static HttpClient HttpClient;

        static BirthdayService()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("api-key", "3espumekuqesPuja7udU");
        }

        public async Task<List<string>> GetBirthdayBoysForTodayAsync()
        {
            var result = await HttpClient.GetAsync("https://auth2.survstat.ru/birthday/today");
            if (!result.IsSuccessStatusCode) return new List<string> { };

            string resultContent = result.Content.ReadAsStringAsync().Result;
            return JArray.Parse(resultContent).ToObject<List<string>>();
        }
    }
}

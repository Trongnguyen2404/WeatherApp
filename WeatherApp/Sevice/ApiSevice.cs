using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Sevices
{
    internal static class ApiSevice
    {
        public static async Task<Root> GetWeather(double latitude, double longtime)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(String.Format("https://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&units=metric&appid=5c77d02338c9fd6ed04cd2a47e4ebec1", latitude, longtime));
            return JsonConvert.DeserializeObject<Root>(response)!;
        }

        public static async Task<Root> GetWeatherByCIty(string city)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/forecast?q={0}&units=metric&appid=5c77d02338c9fd6ed04cd2a47e4ebec1", city));
            return JsonConvert.DeserializeObject<Root>(response);
        }
    }
}

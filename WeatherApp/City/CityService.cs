using System.Collections.Generic;
using System.IO;
using System.Linq; // Thêm dòng này để sử dụng LINQ
using Newtonsoft.Json;

namespace WeatherApp.City
{
    public class CityService
    {
        private List<City> cities;

        public CityService()
        {
            LoadCities();
        }

        private void LoadCities()
        {
            // Lấy đường dẫn hiện tại
            string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Đường dẫn đến file city.list.json
            string jsonFilePath = Path.Combine(currentDirectory, "city.list.json");

            if (File.Exists(jsonFilePath))
            {
                var json = File.ReadAllText(jsonFilePath);
                cities = JsonConvert.DeserializeObject<List<City>>(json);
            }
            else
            {
                throw new FileNotFoundException("Không tìm thấy tệp city.list.json trong thư mục hiện tại.");
            }
        }

        public List<City> GetCities(string searchText)
        {
            // Tìm các thành phố có tên chứa đoạn văn bản nhập vào
            return cities
                .Where(city => city.name.ToLower().Contains(searchText.ToLower()))
                .Take(100)
                .ToList();
        }
    }

    // Lớp City để lưu trữ thông tin thành phố
    public class City
    {
        public double id { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public Coord coord { get; set; }
    }

    // Lớp Coord để lưu trữ tọa độ
    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }
}

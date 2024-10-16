using Microsoft.VisualBasic;
using System;
using WeatherApp.Sevices;
using WeatherApp.City;



namespace WeatherApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public List<Models.List> Weatherlist;
        private double latitude;
        private double longitude;
        private HorizontalStackLayout _previousTappedLayout;
        private Color _initialBackgroundColor = Colors.White;

        public MainPage()
        {
            InitializeComponent();
            Weatherlist = new List<Models.List>();
            // Gán sự kiện nhấn cho các Column
            AddTapGestureRecognizer(Column1);
            AddTapGestureRecognizer(Column2);
            AddTapGestureRecognizer(Column3);
            AddTapGestureRecognizer(Column4);
            AddTapGestureRecognizer(Column5);
            AddTapGestureRecognizer(Column6);
            AddTapGestureRecognizer(Column7);
            AddTapGestureRecognizer(Column8);
        }
        private void AddTapGestureRecognizer(VerticalStackLayout column)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnColumnTapped;
            column.GestureRecognizers.Add(tapGestureRecognizer);
        }
        private void OnColumnTapped(object sender, EventArgs e)
        {
            var column = sender as VerticalStackLayout;
            if (column != null)
            {
                // Lấy giá trị từ HiddenListColumn của Column tương ứng
                var hiddenLabel = column.Children[3] as Label; // HiddenListColumn là phần tử thứ 4 (index = 3)
                if (hiddenLabel != null && int.TryParse(hiddenLabel.Text, out int weatherIndex))
                {
                    if (weatherIndex >= 0 && weatherIndex < Weatherlist.Count)
                    {
                        var forecast = Weatherlist[weatherIndex];

                        // Hiển thị dữ liệu dự báo thời tiết
                        LblWeatherDescription.Text = forecast.weather[0].description;
                        LblTemperature.Text = forecast.main.temperature + "℃";
                        LblHumidity.Text = forecast.main.humidity + "%";
                        LblWind.Text = forecast.wind.speed + "Km/h";
                        ImgWeatherIcon.Source = forecast.weather[0].customIcon;
                        LbltimeDate.Text = forecast.dt_txt;
                    }
                    else
                    {
                        // Xử lý nếu weatherIndex vượt quá số phần tử trong Weatherlist
                        Console.WriteLine("Invalid weather index: " + weatherIndex);
                    }
                }
            }
        }

        private void AddTapGestureRecognizer(HorizontalStackLayout layout, int dayIndex)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => OnHorizontalStackLayoutTapped(s, e, dayIndex);
            layout.GestureRecognizers.Add(tapGestureRecognizer);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GetLocation();
            await GetWeatherDataByLocation(latitude, longitude);
            UpdateDateAndTime();
        }

        public async Task GetLocation()
        {
            var location = await Geolocation.GetLocationAsync();
            latitude = location.Latitude;
            longitude = location.Longitude;
        }

        private async void TapLocation_Tapped(object sender, EventArgs e)
        {
            ResetBackgroundColorForAllLayouts(); // Hủy tô màu trước khi lấy dữ liệu theo vị trí
            await GetLocation();
            await GetWeatherDataByLocation(latitude, longitude);
        }

        public async Task GetWeatherDataByLocation(double latitude, double longitude)
        {
            var result = await ApiSevice.GetWeather(latitude, longitude);
            UpdateUI(result);
        }
        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            var searchBar = (SearchBar)sender;
            string searchText = searchBar.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                var cityService = new CityService();
                var matchingCities = cityService.GetCities(searchText);

                if (matchingCities != null && matchingCities.Count > 0)
                {
                    // Hiển thị các thành phố trong một ListView hoặc CollectionView
                    CitiesListView.ItemsSource = matchingCities;
                    CitiesListView.IsVisible = true; // Hiển thị danh sách thành phố
                }
                else
                {
                    await DisplayAlert("Notification", "No cities found.", "OK");
                }

                searchBar.Text = ""; // Xóa nội dung trong SearchBar sau khi tìm kiếm
            }
        }
        private async void CitiesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedCity = (WeatherApp.City.City)e.Item;
            if (selectedCity != null)
            {
                ResetBackgroundColorForAllLayouts(); // Hủy tô màu trước khi tìm kiếm
                await GetWeatherDataByCity(selectedCity.name);
                CitiesListView.IsVisible = false; // Ẩn danh sách sau khi chọn
            }
        }



        public async Task GetWeatherDataByCity(string city)
        {
            var result = await ApiSevice.GetWeatherByCIty(city);
            UpdateUI(result);
        }

        public void UpdateUI(dynamic result)
        {
            Weatherlist.Clear();

            foreach (var item in result.list)
            {
                Weatherlist.Add(item);
            }
            // CvWeather.ItemsSource = Weatherlist;

            LblCity.Text = result.city.name;
            ShowForecastDetails(1);
            var today = DateTime.Now.Date;
            LblDate1.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            LblDescription1.Text = today.ToString("dddd");

            int dayCounter = 1;
            DateTime? lastDate = null;

            foreach (var forecast in result.list)
            {
                DateTime forecastDate = DateTime.Parse(forecast.dt_txt).Date;

                // Check if this forecast is for the next day
                if (lastDate.HasValue && forecastDate > lastDate.Value)
                {
                    string dayOfWeek = forecastDate.ToString("dddd");

                    switch (dayCounter)
                    {
                        case 1:
                            LblDate2.Text = forecastDate.ToString("dd/MM/yyyy");
                            LblDescription2.Text = dayOfWeek;
                            break;
                        case 2:
                            LblDate3.Text = forecastDate.ToString("dd/MM/yyyy");
                            LblDescription3.Text = dayOfWeek;
                            break;
                        case 3:
                            LblDate4.Text = forecastDate.ToString("dd/MM/yyyy");
                            LblDescription4.Text = dayOfWeek;
                            break;
                        case 4:
                            LblDate5.Text = forecastDate.ToString("dd/MM/yyyy");
                            LblDescription5.Text = dayOfWeek;
                            break;
                        case 5:
                            LblDate6.Text = forecastDate.ToString("dd/MM/yyyy");
                            LblDescription6.Text = dayOfWeek;
                            break;
                    }

                    dayCounter++;
                    if (dayCounter > 5) break;
                }

                lastDate = forecastDate;
            }
        }


        private void ShowForecastDetails(int dayIndex)
        {
            int currentHour = DateTime.Now.Hour;
            DateTime firstForecastTime = DateTime.Parse(Weatherlist[0].dt_txt);
            int firstForecastHour = firstForecastTime.Hour;
            if (currentHour < 3 && currentHour > 0)
            {
                currentHour += 24;
            }
            // Tính toán phần tử trong danh sách Weatherlist
            int index;
            int a = 0;
            int b = 0;
            TempLabel1.Text = "Null";
            TempLabel2.Text = "Null";
            TempLabel3.Text = "Null";
            TempLabel4.Text = "Null";
            TempLabel5.Text = "Null";
            TempLabel6.Text = "Null";
            TempLabel7.Text = "Null";
            TempLabel8.Text = "Null";
            TempRectangle1.HeightRequest = 20;
            TempRectangle2.HeightRequest = 20;
            TempRectangle3.HeightRequest = 20;
            TempRectangle4.HeightRequest = 20;
            TempRectangle5.HeightRequest = 20;
            TempRectangle6.HeightRequest = 20;
            TempRectangle7.HeightRequest = 20;
            TempRectangle8.HeightRequest = 20;
            HiddenListColumn1.Text = "Null";
            HiddenListColumn2.Text = "Null";
            HiddenListColumn3.Text = "Null";
            HiddenListColumn4.Text = "Null";
            HiddenListColumn5.Text = "Null";
            HiddenListColumn6.Text = "Null";
            HiddenListColumn7.Text = "Null";
            HiddenListColumn8.Text = "Null";
            if (dayIndex == 1)
            {
                index = (currentHour - firstForecastHour) / 3;
                int load = (24 - firstForecastHour) / 3;
                for (int i = 8-load; i < 8; i++)
                {
                    var forecast0 = Weatherlist[a];
                    string temperature = Math.Round(forecast0.main.temp, 1) + "℃";


                    string dateTime = DateTime.Parse(forecast0.dt_txt).ToString("HH:mm");

                    switch (i)
                    {
                        case 0:
                            TempLabel1.Text = temperature;
                            TimeLabel1.Text = dateTime;
                            TempRectangle1.HeightRequest = 200 * (forecast0.main.temperature / 40);
                            HiddenListColumn1.Text = (load - 8).ToString();
                            break;
                        case 1:
                            TempLabel2.Text = temperature;
                            TimeLabel2.Text = dateTime;
                            TempRectangle2.HeightRequest = 200 * (forecast0.main.temperature / 40);
                            HiddenListColumn2.Text = (load - 7).ToString();
                            break;
                        case 2:
                            TempLabel3.Text = temperature;
                            TimeLabel3.Text = dateTime;
                            TempRectangle3.HeightRequest = 200 * (forecast0.main.temperature / 40);
                            HiddenListColumn3.Text = (load - 6).ToString();
                            break;
                        case 3:
                            TempLabel4.Text = temperature;
                            TimeLabel4.Text = dateTime;
                            TempRectangle4.HeightRequest = 200 * (forecast0.main.temperature / 40);
                            HiddenListColumn4.Text = (load - 5).ToString();
                            break;
                        case 4:
                            TempLabel5.Text = temperature;
                            TimeLabel5.Text = dateTime;
                            TempRectangle5.HeightRequest = 200 * (forecast0.main.temperature / 40);
                            HiddenListColumn5.Text = (load - 4).ToString();
                            break;
                        case 5:
                            TempLabel6.Text = temperature;
                            TimeLabel6.Text = dateTime;
                            TempRectangle6.HeightRequest = 200 * (forecast0.main.temperature / 40);
                            HiddenListColumn6.Text = (load - 3).ToString();
                            break;
                        case 6:
                            TempLabel7.Text = temperature;
                            TimeLabel7.Text = dateTime;
                            TempRectangle7.HeightRequest = 200 * (forecast0.main.temperature / 40);
                            HiddenListColumn7.Text = (load - 2).ToString();
                            break;
                        case 7:
                            TempLabel8.Text = temperature;
                            TimeLabel8.Text = dateTime;
                            TempRectangle8.HeightRequest = 200 * (forecast0.main.temperature / 40);
                            HiddenListColumn8.Text = (load - 1).ToString();
                            break;
                    }
                    b = a;
                    a = b + 1;
                }
            }
            else
            {
                index = ((24 - firstForecastHour) / 3) + (8 * (dayIndex - 2));
                for (int i = 0; i < 8; i++)  // Vòng lặp đảm bảo chỉ load 8 cột
                {
                    int currentIndex = index + i;
                    if (currentIndex < Weatherlist.Count)  // Kiểm tra để tránh truy cập vượt quá mảng
                    {
                        var forecast = Weatherlist[currentIndex];
                        string temperature = Math.Round(forecast.main.temp, 1) + "℃";

                        string dateTime = DateTime.Parse(forecast.dt_txt).ToString("HH:mm");

                        switch (i)
                        {
                            case 0:
                                TempLabel1.Text = temperature;
                                TimeLabel1.Text = dateTime;
                                TempRectangle1.HeightRequest = 200 * (forecast.main.temperature / 40);
                                HiddenListColumn1.Text = (index ).ToString();

                                break;
                            case 1:
                                TempLabel2.Text = temperature;
                                TimeLabel2.Text = dateTime;
                                TempRectangle2.HeightRequest = 200 * (forecast.main.temperature / 40);
                                HiddenListColumn2.Text = (index + 1).ToString();


                                break;
                            case 2:
                                TempLabel3.Text = temperature;
                                TimeLabel3.Text = dateTime;
                                TempRectangle3.HeightRequest = 200 * (forecast.main.temperature / 40);
                                HiddenListColumn3.Text = (index + 2).ToString();

                                break;
                            case 3:
                                TempLabel4.Text = temperature;
                                TimeLabel4.Text = dateTime;
                                TempRectangle4.HeightRequest = 200 * (forecast.main.temperature / 40);
                                HiddenListColumn4.Text = (index + 3).ToString();

                                break;
                            case 4:
                                TempLabel5.Text = temperature;
                                TimeLabel5.Text = dateTime;
                                TempRectangle5.HeightRequest = 200 * (forecast.main.temperature / 40);
                                HiddenListColumn5.Text = (index + 4).ToString();

                                break;
                            case 5:
                                TempLabel6.Text = temperature;
                                TimeLabel6.Text = dateTime;
                                TempRectangle6.HeightRequest = 200 * (forecast.main.temperature / 40);
                                HiddenListColumn6.Text = (index + 5).ToString();
                                break;
                            case 6:
                                TempLabel7.Text = temperature;
                                TimeLabel7.Text = dateTime;
                                TempRectangle7.HeightRequest = 200 * (forecast.main.temperature / 40);
                                HiddenListColumn7.Text = (index + 6).ToString();
                                break;
                            case 7:
                                TempLabel8.Text = temperature;
                                TimeLabel8.Text = dateTime;
                                TempRectangle8.HeightRequest = 200 * (forecast.main.temperature / 40);
                                HiddenListColumn8.Text = (index + 7).ToString();
                                break;
                        }
                    }
                }
            }


            if (index < Weatherlist.Count)
            {
                var forecast = Weatherlist[index];
                LblWeatherDescription.Text = forecast.weather[0].description;
                LblTemperature.Text = forecast.main.temperature + "℃";
                LblHumidity.Text = forecast.main.humidity + "%";
                LblWind.Text = forecast.wind.speed + "Km/h";
                ImgWeatherIcon.Source = forecast.weather[0].customIcon;
                LbltimeDate.Text = forecast.dt_txt;
            }
            else
            {
                // Nếu index vượt quá danh sách, hãy xử lý hoặc thông báo lỗi.
                Console.WriteLine("Index out of range: " + index);
            }
        }

        private void UpdateDateAndTime()
        {
            var now = DateTime.Now;
            nowDay.Text = now.ToString("dd/MM/yyyy");
            nowTime.Text = now.ToString("HH:mm");
        }

        private void OnHorizontalStackLayoutTapped1(object sender, EventArgs e)
        {
            OnHorizontalStackLayoutTapped(sender, e, 1);
        }

        private void OnHorizontalStackLayoutTapped2(object sender, EventArgs e)
        {
            OnHorizontalStackLayoutTapped(sender, e, 2);
        }

        private void OnHorizontalStackLayoutTapped3(object sender, EventArgs e)
        {
            OnHorizontalStackLayoutTapped(sender, e, 3);
        }

        private void OnHorizontalStackLayoutTapped4(object sender, EventArgs e)
        {
            OnHorizontalStackLayoutTapped(sender, e, 4);
        }

        private void OnHorizontalStackLayoutTapped5(object sender, EventArgs e)
        {
            OnHorizontalStackLayoutTapped(sender, e, 5);
        }
        private void OnHorizontalStackLayoutTapped6(object sender, EventArgs e)
        {
            OnHorizontalStackLayoutTapped(sender, e, 6);
        }

        private void OnHorizontalStackLayoutTapped(object sender, EventArgs e, int dayIndex)
        {
            var currentLayout = sender as HorizontalStackLayout;

            if (currentLayout != null)
            {
                // Nếu một layout đã được nhấn trước đó, đặt lại màu nền của nó
                if (_previousTappedLayout != null && _previousTappedLayout != currentLayout)
                {
                    _previousTappedLayout.BackgroundColor = _initialBackgroundColor; // Đặt lại màu nền ban đầu
                }

                // Lưu màu nền ban đầu của layout hiện tại nếu _previousTappedLayout là null
                if (_previousTappedLayout == null)
                {
                    _initialBackgroundColor = currentLayout.BackgroundColor;
                }

                // Thay đổi màu nền của layout hiện tại
                currentLayout.BackgroundColor = Color.FromHex("#97BEE1");

                // Cập nhật tham chiếu tới layout hiện tại
                _previousTappedLayout = currentLayout;

                // Hiển thị chi tiết dự báo thời tiết cho ngày tương ứng
                ShowForecastDetails(dayIndex);
            }
        }
        private void ResetBackgroundColorForAllLayouts()
        {
            if (_previousTappedLayout != null)
            {
                _previousTappedLayout.BackgroundColor = _initialBackgroundColor;
                _previousTappedLayout = null;
            }
        }

    }

}

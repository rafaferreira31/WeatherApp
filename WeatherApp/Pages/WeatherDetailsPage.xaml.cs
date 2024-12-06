using WeatherApp.Services;

namespace WeatherApp.Pages;

public partial class WeatherDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    public List<Models.List> WeatherList;
    private double latitude;
    private double longitude;

    public WeatherDetailsPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
        WeatherList = new List<Models.List>();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetLocation();
        await GetWeatherDataByLocation(latitude, longitude);
    }


    public async Task GetLocation()
    {
        var location = await Geolocation.GetLocationAsync();
        latitude = location.Latitude;
        longitude = location.Longitude;
    }

    public async Task GetWeatherDataByLocation(double latitude, double longitude)
    {
        var result = await _apiService.GetWeather(latitude, longitude);

        foreach (var item in result.list)
        {
            WeatherList.Add(item);
        }
        CvWeather.ItemsSource = WeatherList;

        LblCity.Text = result.city.name;
        LblWeatherDescription.Text = result.list[0].weather[0].description;
        LblTemperature.Text = result.list[0].main.temperature + "ºC";
        LblHumidity.Text = result.list[0].main.humidity + "%";
        LblVent.Text = result.list[0].wind.speed + "km/h";
        ImgWeatherIcon.Source = result.list[0].weather[0].customIcon;
    }

    public async Task GetWeatherDataByCity(string city)
    {
        var result = await _apiService.GetWeatherByCity(city);

        foreach (var item in result.list)
        {
            WeatherList.Add(item);
        }
        CvWeather.ItemsSource = WeatherList;

        LblCity.Text = result.city.name;
        LblWeatherDescription.Text = result.list[0].weather[0].description;
        LblTemperature.Text = result.list[0].main.temperature + "ºC";
        LblHumidity.Text = result.list[0].main.humidity + "%";
        LblVent.Text = result.list[0].wind.speed + "km/h";
        ImgWeatherIcon.Source = result.list[0].weather[0].customIcon;
    }


    private async void TapLocation_Tapped(object sender, TappedEventArgs e)
    {
        await GetLocation();
        await GetWeatherDataByLocation(latitude, longitude);
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        var response = await DisplayPromptAsync(title:"",message:"", placeholder:"Search Weather by city", accept: "Search", cancel:"Cancel");
        
        if (response != null)
        {
            await GetWeatherDataByCity(response);
        }
    }
}
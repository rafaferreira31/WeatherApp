using System.Collections.ObjectModel;
using WeatherApp.Services;

namespace WeatherApp.Pages;

public partial class CityWeatherDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    public ObservableCollection<Models.List> WeatherList;
    private readonly string _city;

    public CityWeatherDetailsPage(ApiService apiService, string city)
	{
		InitializeComponent();
        _apiService = apiService;
        _city = city;
        WeatherList = new ObservableCollection<Models.List>();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetWeatherDataByCity(_city);
    }

    public async Task GetWeatherDataByCity(string city)
    {
        var result = await _apiService.GetWeatherByCity(city);

        WeatherList.Clear();
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
}
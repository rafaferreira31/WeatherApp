using System.Collections.ObjectModel;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private FavoritesService _favouritesService = new FavoritesService();
    public ObservableCollection<Models.List> WeatherList;
    private double latitude;
    private double longitude;

    public HomePage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
        WeatherList = new ObservableCollection<Models.List>();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetLocation();
        await GetWeatherDataByLocation(latitude, longitude);

        var cityId = await GetCityId(latitude, longitude);
        UpdateFavoriteButton(cityId);
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
            var city = await _apiService.GetCityIdByName(response);
            UpdateFavoriteButton((int)city);
        }
    }

    private async Task<int> GetCityId(double latitude, double longitude)
    {
        var result = await _apiService.GetWeather(latitude, longitude);

        return result.city.id;
    }

    
    private async void ImgBtnFavorite_Clicked(object sender, EventArgs e)
    {
        try
        {
            var cityId = await GetCityId(latitude, longitude);

            var favoriteExists = await _favouritesService.ReadAsync(cityId);

            if (favoriteExists != null)
            {
                await _favouritesService.DeleteAsync(favoriteExists);
            }
            else
            {
                var newFavorite = new FavoriteCity
                {
                    CityId = cityId,
                    Name = LblCity.Text, 
                    IsFavourite = true
                };

                await _favouritesService.CreateAsync(newFavorite);
            }

            UpdateFavoriteButton(cityId);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occured: {ex.Message}", "OK");
        }
    }

    private async void UpdateFavoriteButton(int cityId)
    {
        var favoriteExists = await _favouritesService.ReadAsync(cityId);

        ImgBtnFavorite.Source = favoriteExists != null ? "heartfill" : "heart";
    }
}
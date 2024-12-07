using System.Collections.ObjectModel;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly FavoritesService _favouritesService;
    private readonly ApiService _apiService;
    public List<Models.List> WeatherList;


    public FavoritesPage(ApiService apiService)
	{
		InitializeComponent();
        _favouritesService = ServiceFactory.CreateFavouritesService();
        _apiService = apiService;
        WeatherList = new List<Models.List>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavouriteCities();
    }

    private async Task GetFavouriteCities()
    {
        try
        {
            // Obtém as cidades favoritas armazenadas
            var favouriteCities = await _favouritesService.ReadAllAsync();

            if (favouriteCities is null || favouriteCities.Count == 0)
            {
                CvCities.ItemsSource = null; // Limpa a CollectionView
                LblWarning.IsVisible = true; // Exibe mensagem de aviso
            }
            else
            {
                foreach (var favCity in favouriteCities)
                {
                    // Obtém informações climáticas da cidade favorita
                    var weatherInfo = await _apiService.GetWeatherByCity(favCity.Name);
                    if (weatherInfo != null && weatherInfo.list.Count > 0)
                    {
                        // Atualiza os campos da cidade favorita com os dados da API
                        favCity.Name = weatherInfo.city.name; // Nome atualizado
                        favCity.IsFavourite = true; // Marca como favorita
                        favCity.CityId = weatherInfo.city.id; // ID da cidade

                        // Atualiza informações de clima e ícone
                        favCity.Temperature = weatherInfo.list[0].main.temperatureDisplay;
                        favCity.WeatherIcon = weatherInfo.list[0].weather[0].customIcon;
                    }
                }

                // Atualiza o CollectionView com os dados atualizados
                CvCities.ItemsSource = favouriteCities;
                LblWarning.IsVisible = false; // Esconde mensagem de aviso
            }
        }
        catch (Exception ex)
        {
            // Mostra mensagem de erro em caso de falha
            await DisplayAlert("Error", $"Something went wrong!: {ex.Message}", "OK");
        }
    }



    private void CvCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteCity;

        if (currentSelection is null) return;

        Navigation.PushAsync(new CityWeatherDetailsPage(_apiService,currentSelection.Name));

        ((CollectionView)sender).SelectedItem = null;
    }
}
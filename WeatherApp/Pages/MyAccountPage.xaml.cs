using WeatherApp.Services;
using WeatherApp.Validations;

namespace WeatherApp.Pages;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private const string UserNameKey = "usuarionome";
    private const string UserEmailKey = "usuarioemail";
    private const string UserPhoneKey = "usuariofone";

    public MyAccountPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        CarregarInformacoesUsuario();
        ImgBtnProfile.Source = await GetImagemPerfilAsync();
    }

    private void CarregarInformacoesUsuario()
    {
        LblUserName.Text = Preferences.Get(UserNameKey, string.Empty);
        EntName.Text = LblUserName.Text;
        EntEmail.Text = Preferences.Get(UserEmailKey, string.Empty);
        EntPhone.Text = Preferences.Get(UserPhoneKey, string.Empty);
    }

    private async Task<string?> GetImagemPerfilAsync()
    {
        string defaultImage = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Error", "Not Authorized", "OK");
                    return defaultImage;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Unable to obtain image.", "OK");
                    return defaultImage;
            }
        }

        if (response?.UrlImagem is not null)
        {
            return response.CaminhoImagem;
        }
        return defaultImage;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(UserNameKey, EntName.Text);
        Preferences.Set(UserEmailKey, EntEmail.Text);
        Preferences.Set(UserPhoneKey, EntPhone.Text);

        await DisplayAlert("Saved Information", "Your information has been saved successfully!", "OK");

        await Navigation.PushAsync(new ProfilePage(_apiService, _validator));
    }
}
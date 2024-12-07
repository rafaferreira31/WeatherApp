using WeatherApp.Models;
using WeatherApp.Pages;
using WeatherApp.Services;
using WeatherApp.Validations;

namespace WeatherApp
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;

        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService);
            var favorites = new FavoritesPage(_apiService);
            var questions = new QuestionsPage();
            var about = new AboutPage();
            var profile = new ProfilePage(_apiService, _validator);


            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent { Title = "Home", Icon ="home", Content = homePage },
                    new ShellContent { Title = "Favourites", Icon ="heart", Content = favorites },
                    new ShellContent { Title = "Questions", Icon ="perguntas", Content = questions },
                    new ShellContent { Title = "About", Icon = "icon_13d", Content = about },
                    new ShellContent {Title = "Profile", Icon="profile", Content = profile },
                }

            }
            );
        }
    }
}
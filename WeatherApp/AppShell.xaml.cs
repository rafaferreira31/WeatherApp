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
            var homePage = new HomePage();


            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent { Title = "Home", Icon ="home", Content = homePage },
                }

            }
            );
        }
    }
}

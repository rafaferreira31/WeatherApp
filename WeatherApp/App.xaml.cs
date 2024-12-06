﻿using WeatherApp.Pages;
using WeatherApp.Services;
using WeatherApp.Validations;

namespace WeatherApp
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;

        public App(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;


            SetMainPage();
        }

        private void SetMainPage()
        {
            var accessToken = Preferences.Get("accesstoken", string.Empty);

            if (string.IsNullOrEmpty(accessToken))
            {
                MainPage = new NavigationPage(new RegisterPage(_apiService, _validator));
                return;
            }

            MainPage = new AppShell(_apiService, _validator);
        }
    }
}

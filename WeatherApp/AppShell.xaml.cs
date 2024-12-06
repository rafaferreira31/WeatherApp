﻿using WeatherApp.Models;
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
            var homePage = new WeatherDetailsPage(_apiService);
            var questions = new QuestionsPage();
            var profile = new ProfilePage(_apiService, _validator);


            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent { Title = "Home", Icon ="home", Content = homePage },
                    new ShellContent { Title = "Questions", Icon ="perguntas", Content = questions },
                    new ShellContent {Title = "Profile", Icon="profile", Content = profile },
                }

            }
            );
        }
    }
}
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public static readonly string _baseUrl = "https://nmbd2wm8-7066.uks1.devtunnels.ms/";
        private readonly ILogger<ApiService> _logger;
        JsonSerializerOptions _serializerOptions;


        public ApiService(HttpClient httpClient,
                          ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<bool>> Register(string name, string email,
                                                     string phone, string password)
        {
            try
            {
                var register = new Register()
                {
                    Nome = name,
                    Email = email,
                    Telefone = phone,
                    Senha = password
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Usuarios/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP request: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro registering this user: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }



        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Senha = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Usuarios/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP request: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accesstoken", result!.AccessToken);
                Preferences.Set("usuarioid", (int)result.UsuarioId!);
                Preferences.Set("usuarionome", result.UsuarioNome);
                Preferences.Set("usuarioemail", result.UsuarioEmail);
                Preferences.Set("usuariofone", result.UsuarioFone);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login Error: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var enderecoUrl = _baseUrl + uri;
            try
            {
                var result = await _httpClient.PostAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending POST request for {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<(ProfileImage? ImagemPerfil, string? ErrorMessage)> GetUserProfileImage()
        {
            string endpoint = "api/usuarios/imagemperfil";
            return await GetAsync<ProfileImage>(endpoint);
        }


        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro de requisição HTTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Erro de desserialização JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ApiResponse<bool>> UploadUserImage(byte[] imageArray)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageArray), "imagem", "image.jpg");
                var response = await PostRequest("api/usuarios/uploadfoto", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                      ? "Unauthorized"
                      : $"Erro ao enviar requisição HTTP: {response.StatusCode}";

                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao fazer upload da imagem do usuário: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }
    }
}

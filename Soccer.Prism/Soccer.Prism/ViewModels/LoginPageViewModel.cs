using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
using Soccer.Prism.Views;
using System.Collections.Generic;

namespace Soccer.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isRemember;
        private string _password;
        private DelegateCommand _loginCommand;
        private DelegateCommand _registerCommand;
        private DelegateCommand _forgotPasswordCommand;


        public List<VersionResponse> MyVersions { get; set; }
        public LoginPageViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            IsRemember = true;
            IsEnabled = true;
            Title = "Login";
            IsEnabled = true;
            Email = "luis@yopmail.com";
            Password = "123456";
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(LoginAsync));

        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(RegisterAsync));

        public DelegateCommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new DelegateCommand(ForgotPasswordAsync));

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsRemember
        {
            get => _isRemember;
            set => SetProperty(ref _isRemember, value);
        }

        public string Email { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private async void LoginAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Usuario",
                    "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Password",
                    "Aceptar");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            if (!_apiService.CheckConnection())
            {
                IsRunning = true;
                IsEnabled = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }

            TokenRequest request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            Response response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                   "Error",
                    "Usuario o Password incorrecto",
                    "Aceptar");
                Password = string.Empty;
                return;
            }

            TokenResponse token = (TokenResponse)response.Result;
            EmailRequest request2 = new EmailRequest
            {
                Email = Email
            };

            Response response2 = await _apiService.GetUserByEmail(
                url,
                "api",
                "/Account/GetUserByEmail",
                "bearer",
                token.Token, request2);

            PlayerResponse playerResponse = (PlayerResponse)response2.Result;

            Settings.Player = JsonConvert.SerializeObject(playerResponse);
            Settings.Token = JsonConvert.SerializeObject(token);
            Settings.IsLogin = true;
            Settings.IsRemembered = IsRemember;

            //********** CONTROLA NUMERO DE VERSION **********
            var response3 = await _apiService.GetList2Async<VersionResponse>(
                 url,
                 "api",
                 "/Versions");

            this.MyVersions = (List<VersionResponse>)response3.Result;



            if (response3.IsSuccess)
            {
                var bandera = 0;
                foreach (var cc in MyVersions)
                {
                    if (cc.NroVersion != "1.4")
                    {
                        bandera = 1;
                    }
                }

                if (bandera == 1)
                {
                    //Avisar que hay una nueva version
                    await App.Current.MainPage.DisplayAlert(
                   "Aviso",
                    "Hay una nueva versión en Google Play para descargar",
                    "Aceptar");
                }
            }
















            IsRunning = false;
            IsEnabled = true;

            await _navigationService.NavigateAsync("/SoccerMasterDetailPage/NavigationPage/TournamentsPage");
            Password = string.Empty;
        }
        private async void RegisterAsync()
        {
            await _navigationService.NavigateAsync(nameof(RegisterPage));
        }

        private async void ForgotPasswordAsync()
        {
            await _navigationService.NavigateAsync(nameof(RememberPasswordPage));
        }


    }
}

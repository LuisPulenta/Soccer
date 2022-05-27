using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
using Soccer.Prism.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Soccer.Prism.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IRegexHelper _regexHelper;
        private readonly IApiService _apiService;
        private readonly IFilesHelper _filesHelper;
        private ImageSource _image;
        private UserRequest _user;
        private TeamResponse _team;
        private int _leagueId;
        private LeagueResponse _league;
        private MediaFile _file;
        private ObservableCollection<TeamResponse> _teams;
        private ObservableCollection<LeagueResponse> _leagues;
        private Sex _sex;
        private ObservableCollection<Sex> _sexs;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _registerCommand;
        private DelegateCommand _changeImageCommand;

        public RegisterPageViewModel(
            INavigationService navigationService,
            IRegexHelper regexHelper,
            IApiService apiService,
            IFilesHelper filesHelper) : base(navigationService)
        {
            _navigationService = navigationService;
            _regexHelper = regexHelper;
            _apiService = apiService;
            _filesHelper = filesHelper;
            LoadLeaguesAsync();

            Title = "Registro de Nuevo Usuario";
            Image = App.Current.Resources["UrlNoImage"].ToString();
            IsEnabled = true;
            Sexs = new ObservableCollection<Sex>(CombosHelper.GetSexs());
            Teams = new ObservableCollection<TeamResponse>();
            User = new UserRequest {  };
        }

        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(RegisterAsync));
        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public Sex Sex
        {
            get => _sex;
            set => SetProperty(ref _sex, value);
        }

        public ObservableCollection<Sex> Sexs
        {
            get => _sexs;
            set => SetProperty(ref _sexs, value);
        }

        public UserRequest User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public TeamResponse Team
        {
            get => _team;
            set => SetProperty(ref _team, value);
        }
        public int LeagueId
        {
            get => _leagueId;
            set => SetProperty(ref _leagueId, value);
        }

        public LeagueResponse League
        {
            get => _league;
            //set => SetProperty(ref _league, value);
            set
            {
                if (_league != value)
                {
                    SetProperty(ref _league, value);
                    LoadTeams(_league.Id);
                }
            }
        }

        public ObservableCollection<TeamResponse> Teams
        {
            get => _teams;
            set => SetProperty(ref _teams, value);
        }

        public ObservableCollection<LeagueResponse> Leagues
        {
            get => _leagues;
            set => SetProperty(ref _leagues, value);
        }

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

        private async void RegisterAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }
            IsRunning = true;
            IsEnabled = false;
            string url = App.Current.Resources["UrlAPI"].ToString();
            if (!_apiService.CheckConnection())
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }
            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

            User.TeamId = Team.Id;
            User.PictureArray = imageArray;



            Response response = await _apiService.RegisterUserAsync(url, "api", "/Account", User);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }

            await App.Current.MainPage.DisplayAlert(
                "Ok",
                response.Message,
                "Aceptar");
            await _navigationService.GoBackAsync();
        }


        private async Task<bool> ValidateDataAsync()
        {
            
            if (string.IsNullOrEmpty(User.FirstName))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Nombre",
                    "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.LastName))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Apellido",
                    "Aceptar");
                return false;
            }

            

            if (string.IsNullOrEmpty(User.Email) || !_regexHelper.IsValidEmail(User.Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Email",
                    "Aceptar");
                return false;
            }

            

            if (Team == null)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe seleccionar un Equipo",
                    "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Password) || User.Password?.Length < 6)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Password",
                    "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.PasswordConfirm))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar una Confirmación de Password",
                    "Aceptar");
                return false;
            }

            if (User.Password != User.PasswordConfirm)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "El Password y su confirmación no coinciden",
                    "Aceptar");
                return false;
            }

            return true;
        }

        private async void LoadLeaguesAsync()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            if (!_apiService.CheckConnection())
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }

            Response response = await _apiService.GetListAsync<LeagueResponse>(url, "api", "/Leagues");

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }
            List<LeagueResponse> list = (List<LeagueResponse>)response.Result;
            Leagues = new ObservableCollection<LeagueResponse>(list.OrderBy(t => t.Name));
        }

        public void LoadTeams(int Id)
        {

            var teams = Leagues.Where(l => l.Id == Id).FirstOrDefault().Teams;
            Teams.Clear();
            foreach (var team in teams.OrderBy(l => l.Name))
            {
                Teams.Add(new TeamResponse
                {
                    Id = team.Id,
                    Initials = team.Initials,
                    Name = team.Name,
                    LogoPath = team.LogoPath,
                    LeagueId = team.LeagueId,
                    LeagueName = team.LeagueName
                });
            }
        }

        private async void ChangeImageAsync()
        {
            await CrossMedia.Current.Initialize();

            string source = await Application.Current.MainPage.DisplayActionSheet(
                "¿De dónde quiere tomar la foto?:",
                "Cancelar",
                null,
                "Galería",
                "Cámara");

            if (source == "Cancelar")
            {
                _file = null;
                return;
            }

            if (source == "Cámara")
            {
                _file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                _file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (_file != null)
            {
                Image = ImageSource.FromStream(() =>
                {
                    System.IO.Stream stream = _file.GetStream();
                    return stream;
                });
            }
        }



    }
}
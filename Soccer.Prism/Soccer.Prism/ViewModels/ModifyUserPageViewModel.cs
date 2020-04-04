using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System.Threading.Tasks;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Soccer.Common.Services;
using System.Collections.Generic;
using System.Linq;
using Soccer.Prism.Views;
using Soccer.Prism.Helpers;
using System;

namespace Soccer.Prism.ViewModels
{
    public class ModifyUserPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private readonly IFilesHelper _filesHelper;
        private bool _isRunning;
        private bool _isEnabled;
        private ImageSource _image;
        private int _leagueId;
        private LeagueResponse _league;
        private ObservableCollection<LeagueResponse> _leagues;
        private PlayerResponse _player;
        private Sex _sex;
        private int _sexId;
        private ObservableCollection<Sex> _sexs;
        private MediaFile _file;
        private TeamResponse _team;
        private ObservableCollection<TeamResponse> _teams;
        private DelegateCommand _changeImageCommand;
        private DelegateCommand _saveCommand;
        private DelegateCommand _changePasswordCommand;

        public ModifyUserPageViewModel(INavigationService navigationService, IApiService apiService, IFilesHelper filesHelper)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _filesHelper = filesHelper;
            Player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            Title = "Modificar Usuario";
            Sexs = new ObservableCollection<Sex>(CombosHelper.GetSexs());
            Sex = Sexs.FirstOrDefault(pt => pt.Name == Player.Sex);
            LeagueId = Player.Team.LeagueId;
            IsEnabled = true;
            var tournamentsPageViewModel = TournamentsPageViewModel.GetInstance();
            Leagues = tournamentsPageViewModel.Leagues;
            //League = new LeagueResponse
            //{
            //    Id = Convert.ToInt32(Player.Team.LeagueId),
            //    Name = Player.Team.LeagueName
            //};
            Teams = new ObservableCollection<TeamResponse>();
            //LoadTeams(LeagueId);
            Image = Player.PictureFullPath;
        }

        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public DelegateCommand ChangePasswordCommand => _changePasswordCommand ?? (_changePasswordCommand = new DelegateCommand(ChangePasswordAsync));

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public TeamResponse Team
        {
            get => _team;
            set => SetProperty(ref _team, value);
        }
        public Sex Sex
        {
            get => _sex;
            set => SetProperty(ref _sex, value);
        }

        public int SexId
        {
            get => _sexId;
            set => SetProperty(ref _sexId, value);
        }

        public ObservableCollection<Sex> Sexs
        {
            get => _sexs;
            set => SetProperty(ref _sexs, value);
        }
        public int LeagueId
        {
            get => _leagueId;
            set => SetProperty(ref _leagueId, value);
        }
        public ObservableCollection<LeagueResponse> Leagues
        {
            get => _leagues;
            set => SetProperty(ref _leagues, value);
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

        public PlayerResponse Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
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

        private async void SaveAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

            Player.Sex = Sex.Name;
            Player.Team.Id = Team.Id;
            //Player.PictureArray = imageArray;

            UserRequest userRequest = new UserRequest
            {
                Address = Player.Address,
                Document = Player.Document,
                Email = Player.Email,
                FirstName = Player.FirstName,
                LastName = Player.LastName,
                Password = "123456", // It doesn't matter what is sent here. It is only for the model to be valid
                PasswordConfirm = "123456", // It doesn't matter what is sent here. It is only for the model to be valid
                Phone = Player.PhoneNumber,
                PictureArray = imageArray,
                Sex = Player.Sex,
                NickName = Player.NickName,
                TeamId = Player.Team.Id
            };

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.PutAsync(url, "api", "/Account", userRequest, "bearer", token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }

            Settings.Player = JsonConvert.SerializeObject(Player);
            SoccerMasterDetailPageViewModel.GetInstance().ReloadUser();
            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "El Usuario fue actualizado.",
                "Aceptar");
        }


        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(Player.Document))
            {
                await App.Current.MainPage.DisplayAlert(
                     "Error",
                    "Debe ingresar un Documento",
                    "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(Player.FirstName))
            {
                await App.Current.MainPage.DisplayAlert(
                     "Error",
                    "Debe ingresar un Nombre",
                    "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(Player.LastName))
            {
                await App.Current.MainPage.DisplayAlert(
                      "Error",
                    "Debe ingresar un Apellido",
                    "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(Player.Address))
            {
                await App.Current.MainPage.DisplayAlert(
                      "Error",
                    "Debe ingresar un Domicilio",
                    "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(Player.PhoneNumber))
            {
                await App.Current.MainPage.DisplayAlert(
                      "Error",
                    "Debe ingresar un Teléfono",
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

            return true;
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
                    LeagueName = team.LeagueName,

                });
            }
        }

        private async void ChangePasswordAsync()
        {
            await _navigationService.NavigateAsync(nameof(ChangePasswordPage));
        }
        private async void LoadLeaguesAsync()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
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
    }
}
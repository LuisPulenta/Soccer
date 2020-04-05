using Newtonsoft.Json;
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
    public class AddGroupBetPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private readonly IFilesHelper _filesHelper;
        private ImageSource _image;
        private TournamentResponse _tournament;
        private MediaFile _file;
        private GroupBetRequest _groupBet;
        private ObservableCollection<TournamentResponse> _tournaments;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _saveCommand;
        private DelegateCommand _changeImageCommand;
        private GroupBetResponse _groupBetResponse;

        public AddGroupBetPageViewModel(
            INavigationService navigationService,
            IApiService apiService,
            IFilesHelper filesHelper) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _filesHelper = filesHelper;
            Title = "Nuevo Grupo de Apuestas";
            Image = App.Current.Resources["UrlNoImage"].ToString();
            IsEnabled = true;
            Tournaments = new ObservableCollection<TournamentResponse>();
            GroupBet = new GroupBetRequest { CreationDate = DateTime.Today };
            LoadTournamentsAsync();
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));
        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public TournamentResponse Tournament
        {
            get => _tournament;
            set => SetProperty(ref _tournament, value);
        }

        public ObservableCollection<TournamentResponse> Tournaments
        {
            get => _tournaments;
            set => SetProperty(ref _tournaments, value);
        }

        public GroupBetRequest GroupBet
        {
            get => _groupBet;
            set => SetProperty(ref _groupBet, value);
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
            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

            var Admin = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);


            var groupBetRequest = new GroupBetRequest
            {
                Name = GroupBet.Name,
                TournamentId = Tournament.Id,
                CreationDate = DateTime.Today,
                PlayerEmail = Admin.Email,
                PictureArray = imageArray,
            };
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            Response response = await _apiService.AddGroupBetAsync(url, "api", "/GroupBets", groupBetRequest, "bearer", token.Token);
            

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }

            MyGroupsPageViewModel.GetInstance().ReloadGroups();

            await App.Current.MainPage.DisplayAlert(
                "Ok",
                response.Message,
                "Aceptar");
            await _navigationService.GoBackAsync();



        }


        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(GroupBet.Name))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar un Nombre",
                    "Aceptar");
                return false;
            }



            if (Tournament == null)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe seleccionar un Torneo",
                    "Aceptar");
                return false;
            }

            return true;
        }

        private async void LoadTournamentsAsync()
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

            Response response = await _apiService.GetListAsync<TournamentResponse>(url, "api", "/Tournaments");

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }
            List<TournamentResponse> list = (List<TournamentResponse>)response.Result;
            Tournaments = new ObservableCollection<TournamentResponse>(list.OrderBy(t => t.Name));
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
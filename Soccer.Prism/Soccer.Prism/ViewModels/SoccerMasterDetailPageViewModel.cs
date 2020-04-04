using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
using Soccer.Prism.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Soccer.Prism.ViewModels
{
    public class SoccerMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private PlayerResponse _player;
        private DelegateCommand _modifyUserCommand;
        private static SoccerMasterDetailPageViewModel _instance;

        public SoccerMasterDetailPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            LoadUser();
            LoadMenus();
        }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        public DelegateCommand ModifyUserCommand => _modifyUserCommand ?? (_modifyUserCommand = new DelegateCommand(ModifyUserAsync));

        public PlayerResponse Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        private void LoadUser()
        {
            if (Settings.IsLogin)
            {
                Player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            }
        }


        private void LoadMenus()
        {
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Icon = "tournament",
                    PageName = "TournamentsPage",
                    Title = "Torneos"
                },
                 new Menu
                {
                    Icon = "groups",
                    PageName = "MyGroupsPage",
                    Title = "Mis Grupos",
                    IsLoginRequired = true
                },
                new Menu
                {
                    Icon = "prediction",
                    PageName = "MyPredictionsPage",
                    Title = "Mis Predicciones",
                    IsLoginRequired = true
                },
                new Menu
                {
                    Icon = "medal",
                    PageName = "MyPositionsPage",
                    Title = "Mis Posiciones",
                    IsLoginRequired = true
                },
                new Menu
                {
                    Icon = "user",
                    PageName = "ModifyUserPage",
                    Title = "Modificar Usuario",
                    IsLoginRequired = true
                },
                new Menu
                {
                    Icon = "login",
                    PageName = "LoginPage",
                    Title = Settings.IsLogin ? "Cerrar sesión" : "Iniciar Sesión"
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title,
                    IsLoginRequired = m.IsLoginRequired
                }).ToList());
        }
        private async void ModifyUserAsync()
        {
            await _navigationService.NavigateAsync($"/SoccerMasterDetailPage/NavigationPage/{nameof(ModifyUserPage)}");
        }

        public static SoccerMasterDetailPageViewModel GetInstance()
        {
            return _instance;
        }

        public async void ReloadUser()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                return;
            }

            PlayerResponse player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            EmailRequest emailRequest = new EmailRequest
            {
                Email = player.Email
            };

            Response response = await _apiService.GetUserByEmail(url, "api", "/Account/GetUserByEmail", "bearer", token.Token, emailRequest);
            PlayerResponse userResponse = (PlayerResponse)response.Result;
            Settings.Player = JsonConvert.SerializeObject(userResponse);

            LoadUser();
        }

    }
}

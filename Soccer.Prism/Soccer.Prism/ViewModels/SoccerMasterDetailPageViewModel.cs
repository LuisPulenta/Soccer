using Newtonsoft.Json;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Soccer.Prism.ViewModels
{
    public class SoccerMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private PlayerResponse _player;

        public SoccerMasterDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            LoadUser();
            LoadMenus();
        }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

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
                    Title = "Mis Grupos"
                },
                new Menu
                {
                    Icon = "prediction",
                    PageName = "MyPredictionsPage",
                    Title = "Mis Predicciones"
                },
                new Menu
                {
                    Icon = "medal",
                    PageName = "MyPositionsPage",
                    Title = "Mis Posiciones"
                },
                new Menu
                {
                    Icon = "user",
                    PageName = "ModifyUserPage",
                    Title = "Modificar Usuario"
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
                    Title = m.Title
                }).ToList());
        }
    }
}

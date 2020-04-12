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
    public class MyGroupsPageViewModel : ViewModelBase
    {
        private DelegateCommand _addGroupBetCommand;
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private PlayerGroupBetResponse _playerGroupBet;
        private PlayerResponse _player;
        private PlayerResponse _playerLogueado;
        private int _cantPlayers;
        private bool _isEnabled;
        private bool _isRefreshing;
        private bool _isRunning;

        private ObservableCollection<PlayerGroupBetItemViewModel> _playerGroupBets;
        private ObservableCollection<GroupBetItemViewModel> _groupBets;

        private static MyGroupsPageViewModel _instance;
        private int _cantPlayer;
        private string _filter;
        private DelegateCommand _searchCommand;
        private DelegateCommand _refreshCommand;


        public int CantPlayers
        {
            get => _cantPlayers;
            set => SetProperty(ref _cantPlayers, value);
        }

        public PlayerResponse PlayerLogueado
        {
            get => _playerLogueado;
            set => SetProperty(ref _playerLogueado, value);
        }


        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        public DelegateCommand AddGroupBetCommand => _addGroupBetCommand ?? (_addGroupBetCommand = new DelegateCommand(AddGroupBet));
        public DelegateCommand SearchCommand => _searchCommand ?? (_searchCommand = new DelegateCommand(Search));
        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(Refresh));

        public ObservableCollection<PlayerGroupBetItemViewModel> PlayerGroupBets
        {
            get => _playerGroupBets;
            set => SetProperty(ref _playerGroupBets, value);
        }

        public ObservableCollection<GroupBetItemViewModel> GroupBets
        {
            get => _groupBets;
            set => SetProperty(ref _groupBets, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public List<PlayerGroupBetResponse> MyGroups { get; set; }

        public static MyGroupsPageViewModel GetInstance()
        {
            return _instance;
        }

        public int CantPlayer
        {
            get => _cantPlayer;
            set => SetProperty(ref _cantPlayer, value);
        }

        public MyGroupsPageViewModel(INavigationService navigationService, IApiService apiService)
           : base(navigationService)

        {

            _navigationService = navigationService;
            _apiService = apiService;
            _instance = this;
            LoadPlayer();
            PlayerGroupBets = new ObservableCollection<PlayerGroupBetItemViewModel>();
            GroupBets = new ObservableCollection<GroupBetItemViewModel>();
            Title = "Mis Grupos de Apuestas";
            if (Settings.IsLogin)
            {
                PlayerLogueado = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            }


        }

        private async void AddGroupBet()
        {
            await _navigationService.NavigateAsync(nameof(AddGroupBetPage));
        }

        private async void LoadPlayer()
        {
            _player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var url = App.Current.Resources["UrlAPI"].ToString();
            var controller = string.Format("/GroupBets/GetGroupBetsByEmail");
            var response = await _apiService.GetGroupBetsByEmail(
                url,
                "api",
                controller,
                _player.Email,
                "bearer",
                token.Token);
            IsRefreshing = false;
            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Problema para recuperar datos.", "Aceptar");
                return;
            }

            var player = (List<GroupBetResponse>)response.Result;


            GroupBets = new ObservableCollection<GroupBetItemViewModel>(player.Select(p => new GroupBetItemViewModel(_navigationService)
            {
                Id = p.Id,
                Admin = p.Admin,
                LogoPath = p.LogoPath,
                CreationDate = p.CreationDate,
                Name = p.Name,
                Tournament = p.Tournament,
                GroupBetPlayers = p.GroupBetPlayers,
                CantPlayers=p.GroupBetPlayers.Count()
            }).ToList());



            //RefreshList();
            IsRefreshing = false;

        }

        private async void Search()
        {
            //RefreshList();
        }

        private async void Refresh()
        {
            LoadPlayer();
        }

        public async void ReloadGroups()
        {
           LoadPlayer();
        }




        //public void RefreshList()
        //{
        //    if (string.IsNullOrEmpty(this.Filter))
        //    {

        //        var myListCableItemViewModel = MyCables.Select(a => new CableItemViewModel(_navigationService)
        //        {
        //            CantRem = a.CantRem,
        //            CAUSANTEC = a.CAUSANTEC,
        //            CLIENTE = a.CLIENTE,
        //            CodigoCierre = a.CodigoCierre,
        //            CP = a.CP,
        //            Descripcion = a.Descripcion,
        //            DOMICILIO = a.DOMICILIO,
        //            ENTRECALLE1 = a.ENTRECALLE1,
        //            ENTRECALLE2 = a.ENTRECALLE2,
        //            ESTADOGAOS = a.ESTADOGAOS,
        //            GRXX = a.GRXX,
        //            GRYY = a.GRYY,
        //            FechaAsignada = a.FechaAsignada,
        //            Novedades = a.Novedades,
        //            LOCALIDAD = a.LOCALIDAD,
        //            NOMBRE = a.NOMBRE,
        //            ObservacionCaptura = a.ObservacionCaptura,
        //            PROYECTOMODULO = a.PROYECTOMODULO,
        //            RECUPIDJOBCARD = a.RECUPIDJOBCARD,
        //            SUBCON = a.SUBCON,
        //            TELEFONO = a.TELEFONO,
        //            UserID = a.UserID,
        //            PROVINCIA = a.PROVINCIA,
        //            ReclamoTecnicoID = a.ReclamoTecnicoID,
        //            MOTIVOS = a.MOTIVOS,
        //            IDSuscripcion = a.IDSuscripcion,
        //        });
        //        Cables = new ObservableCollection<CableItemViewModel>(myListCableItemViewModel.OrderBy(o => o.FechaAsignada + o.NOMBRE));
        //        CantCables = Cables.Count();
        //    }
        //    else
        //    {
        //        var myListCableItemViewModel = MyCables.Select(a => new CableItemViewModel(_navigationService)
        //        {
        //            CantRem = a.CantRem,
        //            CAUSANTEC = a.CAUSANTEC,
        //            CLIENTE = a.CLIENTE,
        //            CodigoCierre = a.CodigoCierre,
        //            CP = a.CP,
        //            Descripcion = a.Descripcion,
        //            DOMICILIO = a.DOMICILIO,
        //            ENTRECALLE1 = a.ENTRECALLE1,
        //            ENTRECALLE2 = a.ENTRECALLE2,
        //            ESTADOGAOS = a.ESTADOGAOS,
        //            GRXX = a.GRXX,
        //            GRYY = a.GRYY,
        //            FechaAsignada = a.FechaAsignada,
        //            Novedades = a.Novedades,
        //            LOCALIDAD = a.LOCALIDAD,
        //            NOMBRE = a.NOMBRE,
        //            ObservacionCaptura = a.ObservacionCaptura,
        //            PROYECTOMODULO = a.PROYECTOMODULO,
        //            RECUPIDJOBCARD = a.RECUPIDJOBCARD,
        //            SUBCON = a.SUBCON,
        //            TELEFONO = a.TELEFONO,
        //            UserID = a.UserID,
        //            PROVINCIA = a.PROVINCIA,
        //            ReclamoTecnicoID = a.ReclamoTecnicoID,
        //            MOTIVOS = a.MOTIVOS,
        //            IDSuscripcion = a.IDSuscripcion,
        //        });
        //        Cables = new ObservableCollection<CableItemViewModel>(myListCableItemViewModel
        //            .OrderBy(o => o.FechaAsignada + o.NOMBRE)
        //            .Where(
        //                    o => (o.NOMBRE.ToLower().Contains(this.Filter.ToLower()))
        //                    ||
        //                    (o.CLIENTE.ToLower().Contains(this.Filter.ToLower()))
        //                  )
        //                                                                                       );




        //        CantCables = Cables.Count();
        //    }
        //}
    }
}
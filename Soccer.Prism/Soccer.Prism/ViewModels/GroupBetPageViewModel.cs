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
    public class GroupBetPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private GroupBetResponse _groupBet;
        private ObservableCollection<GroupBetPlayerItemViewModel> _groupBetPlayers;
        private bool _isEnabledAdmin;
        private bool _isEnabledPlayer;
        private PlayerResponse _player;
        private DelegateCommand _invitarCommand;
        private DelegateCommand _borrarGrupoCommand;
        private DelegateCommand _salirGrupoCommand;

        public DelegateCommand InvitarCommand => _invitarCommand ?? (_invitarCommand = new DelegateCommand(InvitarAsync));
        public DelegateCommand BorrarGrupoCommand => _borrarGrupoCommand ?? (_borrarGrupoCommand = new DelegateCommand(BorrarGrupoAsync));
        public DelegateCommand SalirGrupoCommand => _salirGrupoCommand ?? (_salirGrupoCommand = new DelegateCommand(SalirGrupoAsync));

        public GroupBetPageViewModel(INavigationService navigationService,IApiService apiService ) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Grupo de Apuestas:";
            Player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            LoadGroupBet();
            if (Player.UserId == GroupBet.Admin.UserId)
            {
                IsEnabledAdmin = true;
                IsEnabledPlayer = false;
            }
            if (!(Player.UserId == GroupBet.Admin.UserId))
            {
                IsEnabledAdmin = false;
                IsEnabledPlayer = true;
            }

        }

        public ObservableCollection<GroupBetPlayerItemViewModel> GroupBetPlayers
        {
            get => _groupBetPlayers;
            set => SetProperty(ref _groupBetPlayers, value);
        }

        public bool IsEnabledAdmin
        {
            get => _isEnabledAdmin;
            set => SetProperty(ref _isEnabledAdmin, value);
        }

        public bool IsEnabledPlayer
        {
            get => _isEnabledPlayer;
            set => SetProperty(ref _isEnabledPlayer, value);
        }

        public PlayerResponse Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        public GroupBetResponse GroupBet
        {
            get => _groupBet;
            set => SetProperty(ref _groupBet, value);
        }


      
        private void LoadGroupBet()
        {
            GroupBet = JsonConvert.DeserializeObject<GroupBetResponse>(Settings.GroupBet);
            Title = $"Grupo: {_groupBet.Name}";
            GroupBetPlayers = new ObservableCollection<GroupBetPlayerItemViewModel>(_groupBet.GroupBetPlayers.Select(p => new GroupBetPlayerItemViewModel(_navigationService, _apiService)
            {
                Id = p.Id,
                GroupBet=p.GroupBet,
                IsAccepted = p.IsAccepted,
                IsBlocked = p.IsBlocked,
                Player = p.Player,
                Points = p.Points,
            }).ToList());
            
        }

        private async void InvitarAsync()
        {
            
            await _navigationService.NavigateAsync("InvitarPage");
        }

        private async void BorrarGrupoAsync()
        {

        }

        private async void SalirGrupoAsync()
        {

        }
    }
}

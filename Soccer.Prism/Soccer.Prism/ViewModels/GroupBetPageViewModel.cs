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
        private bool _isRunning;
        private bool _isEnabled;

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
            var answer = await App.Current.MainPage.DisplayAlert(
                "Confirmar",
                "¿Está seguro de borrar este grupo?",
                "Si",
                "No");

            if (!answer)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;


            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);


            foreach (GroupBetPlayerResponse groupBetPlayer in GroupBet.GroupBetPlayers)
            {
                var responsex = await _apiService.DeleteAsync(
                url,
                "api",
                "/GroupBetPlayers",
                groupBetPlayer.Id,
                "bearer",
                token.Token);
            }
            

            var response = await _apiService.DeleteAsync(
               url,
               "api",
               "/GroupBets",
               GroupBet.Id,
               "bearer",
               token.Token);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "No se puedo borrar", //response.Message,
                    "Accept");
                return;
            }
            MyGroupsPageViewModel.GetInstance().ReloadGroups();
            IsRunning = false;
            IsEnabled = true;
            await _navigationService.GoBackAsync();
        }

        private async void SalirGrupoAsync()
        {
                DeleteAsync();
        }

        private async void DeleteAsync()
        {
            var answer = await App.Current.MainPage.DisplayAlert(
                "Confirmar",
                "¿Está seguro de salir de este grupo?",
                "Si",
                "No");

            if (!answer)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;


            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var groupBetRequest = new GroupBetPlayerRequest2
            {
                GroupBetId = GroupBet.Id,
                PlayerId = Player.Id
            };


            Response response2 = await _apiService.GetGroupBetPlayerByIds(
                url,
                "api",
                "/GroupBetPlayers/GetGroupBetPlayerByIds",
                "bearer",
                token.Token, groupBetRequest);

            GroupBetPlayerResponse groupBetPlayerResponse = (GroupBetPlayerResponse)response2.Result;
            

            var response = await _apiService.DeleteAsync(
                url,
                "api",
                "/GroupBetPlayers",
                groupBetPlayerResponse.Id, 
                "bearer",
                token.Token);

            MyGroupsPageViewModel.GetInstance().ReloadGroups();

            IsRunning = false;
            IsEnabled = true;
            await _navigationService.GoBackAsync();
        }
    }
}
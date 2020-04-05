using Newtonsoft.Json;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
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


        public GroupBetPageViewModel(INavigationService navigationService,IApiService apiService ) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Grupo de Apuestas:";
            LoadGroupBet();
        }

        public ObservableCollection<GroupBetPlayerItemViewModel> GroupBetPlayers
        {
            get => _groupBetPlayers;
            set => SetProperty(ref _groupBetPlayers, value);
        }




        public GroupBetResponse GroupBet
        {
            get => _groupBet;
            set => SetProperty(ref _groupBet, value);
        }


        //public override void OnNavigatedTo(INavigationParameters parameters)
        //{
        //    base.OnNavigatedTo(parameters);

        //    if (parameters.ContainsKey("groupBet"))
        //    {
        //        _groupBet = parameters.GetValue<GroupBetResponse>("groupBet");
        //        Title = _groupBet.Name;
        //        GroupBetPlayers = _transformHelper.ToGroupBetPlayers(_groupBet.GroupBetPlayers);

        //    }
        //}

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


    }
}

using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;

namespace Soccer.Prism.ViewModels
{
    public class GroupBetPlayer2ItemViewModel : PositionResponse
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private DelegateCommand _selectGroupBetPlayer2Command;

        public DelegateCommand SelectGroupBetPlayer2Command => _selectGroupBetPlayer2Command ?? (_selectGroupBetPlayer2Command = new DelegateCommand(SelectGroupBetPlayer));

        public GroupBetPlayer2ItemViewModel(INavigationService navigationService, IApiService apiService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
        }

        private async void SelectGroupBetPlayer()
        {

            NavigationParameters parameters = new NavigationParameters
            {
                { "groupBetPlayer", this }
            };


            var  este = new PositionResponse
            {
                Points = this.Points,
                Ranking = this.Ranking,
                PlayerResponse = this.PlayerResponse
            };


            
            Settings.GroupBetPlayer = JsonConvert.SerializeObject(este);
            await _navigationService.NavigateAsync("GroupBetPagePlayer", parameters);
        }
    }
}
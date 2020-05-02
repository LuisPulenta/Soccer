using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;

namespace Soccer.Prism.ViewModels
{
    public class GroupBetItemViewModel : GroupBetResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectGroupBetCommand;


        public GroupBetItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        public DelegateCommand SelectGroupBetCommand => _selectGroupBetCommand ?? (_selectGroupBetCommand = new DelegateCommand(SelectGroupBet));

        private async void SelectGroupBet()
        {

            NavigationParameters parameters = new NavigationParameters
            {
                { "groupBet", this }
            };
            Settings.Tournament = JsonConvert.SerializeObject(this.Tournament);
            Settings.GroupBet = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("GroupBetPage", parameters);
        }

    }
}
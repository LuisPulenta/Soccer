using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;

namespace Soccer.Prism.ViewModels
{
    public class PlayerGroupBetItemViewModel : PlayerGroupBetResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectGroupBetCommand;
        public PlayerGroupBetItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        public DelegateCommand SelectGroupBetCommand => _selectGroupBetCommand ?? (_selectGroupBetCommand = new DelegateCommand(SelectGroupBet));

        private async void SelectGroupBet()
        {
            Settings.GroupBet = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("GroupBetPage");
        }

    }
}
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Prism.Views;

namespace Soccer.Prism.ViewModels
{
    public class MyGroupsItemViewModel : GroupBetResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectGroupBetCommand;

        public MyGroupsItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectGroupBetCommand => _selectGroupBetCommand ?? (_selectGroupBetCommand = new DelegateCommand(SelectGroupBetAsync));


        private async void SelectGroupBetAsync()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "group", this }
            };

            Settings.GroupBet = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("GroupBetPage", parameters);
        }
    }
}
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Models;
using Soccer.Common.Services;

namespace Soccer.Prism.ViewModels
{
    public class GroupBetPlayerItemViewModel : GroupBetPlayerResponse
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private DelegateCommand _updatePredictionCommand;

        public GroupBetPlayerItemViewModel(INavigationService navigationService, IApiService apiService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
        }
    }
}
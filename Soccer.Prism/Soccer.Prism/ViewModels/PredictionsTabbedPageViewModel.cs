using Prism.Navigation;
using Soccer.Common.Models;

namespace Soccer.Prism.ViewModels
{
    public class PredictionsTabbedPageViewModel : ViewModelBase
    {
        private TournamentResponse _tournament;

        public PredictionsTabbedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Predicciones para...";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("tournament"))
            {
                _tournament = parameters.GetValue<TournamentResponse>("tournament");
                Title = $"{_tournament.Name}";
            }
        }
    }
}
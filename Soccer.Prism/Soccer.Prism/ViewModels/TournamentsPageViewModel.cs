using Prism.Navigation;
using Soccer.Common.Models;
using Soccer.Common.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Soccer.Prism.ViewModels
{
    public class TournamentsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private List<TournamentItemViewModel> _tournaments;
        private ObservableCollection<LeagueResponse> _leagues;
        private bool _isRunning;
        private static TournamentsPageViewModel _instance;

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public TournamentsPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _instance = this;
            Title = "Torneos";
            LoadTournamentsAsync();
            LoadLeaguesAsync();
        }

        public ObservableCollection<LeagueResponse> Leagues
        {
            get => _leagues;
            set => SetProperty(ref _leagues, value);
        }

        public List<TournamentItemViewModel> Tournaments
        {
            get => _tournaments;
            set => SetProperty(ref _tournaments, value);
        }

        private async void LoadTournamentsAsync()
        {
            IsRunning = true;
            string url = App.Current.Resources["UrlAPI"].ToString();

            if (!_apiService.CheckConnection())
            {
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "Revise su conexión a Internet.", "Aceptar");
                return;
            }

            Response response = await _apiService.GetListAsync<TournamentResponse>(
                url,
                "api",
                "/Tournaments");
            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }

            List<TournamentResponse> list = (List<TournamentResponse>)response.Result;
            Tournaments = list.Select(t => new TournamentItemViewModel(_navigationService)
            {
                EndDate = t.EndDate,
                Groups = t.Groups,
                Id = t.Id,
                IsActive = t.IsActive,
                LogoPath = t.LogoPath,
                Name = t.Name,
                StartDate = t.StartDate
            }).ToList();
        }

        private async void LoadLeaguesAsync()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            if (!_apiService.CheckConnection())
            {
                IsRunning = false;

                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }

            Response response = await _apiService.GetListAsync<LeagueResponse>(url, "api", "/Leagues");

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }
            List<LeagueResponse> list = (List<LeagueResponse>)response.Result;
            Leagues = new ObservableCollection<LeagueResponse>(list.OrderBy(t => t.Name));
        }

        public static TournamentsPageViewModel GetInstance()
        {
            return _instance;
        }
    }
}

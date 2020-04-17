using Newtonsoft.Json;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
using Soccer.Prism;
using Soccer.Prism.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Soccer.Prism.ViewModels
{
    public class ClosedPredictionsForTournamentPageViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private TournamentResponse _tournament;
        private bool _isRunning;
        private ObservableCollection<PredictionItemViewModel> _predictions;

        public ClosedPredictionsForTournamentPageViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _apiService = apiService;
            Title = "Cerrados";
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public ObservableCollection<PredictionItemViewModel> Predictions
        {
            get => _predictions;
            set => SetProperty(ref _predictions, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            _tournament = parameters.GetValue<TournamentResponse>("tournament");
            LoadPredictionsAsync();
        }

        private async void LoadPredictionsAsync()
        {
            IsRunning = true;
            var url = App.Current.Resources["UrlAPI"].ToString();

            if (!_apiService.CheckConnection())
            {
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }

            PlayerResponse player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var request = new PredictionsForUserRequest
            {
                TournamentId = _tournament.Id,
                UserId = new Guid(player.UserId),
            };

            Response response = await _apiService.GetPredictionsForUserAsync(url, "api", "/Predictions/GetPredictionsForUser", request, "bearer", token.Token);
            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                     "Error",
                     response.Message,
                    "Aceptar");
                return;
            }

            List<PredictionResponse> list = (List<PredictionResponse>)response.Result;
            Predictions = new ObservableCollection<PredictionItemViewModel>(list
                .Select(p => new PredictionItemViewModel(_apiService)
                {
                    GoalsLocal = p.GoalsLocal,
                    GoalsVisitor = p.GoalsVisitor,
                    Id = p.Id,
                    Match = p.Match,
                    Points = p.Points,
                    Player=p.Player
                })
                //.Where(p => p.Match.IsClosed || p.Match.DateLocal > DateTime.Now)
                .Where(p => p.Match.IsClosed)
                .OrderBy(p => p.Match.Date)
                .ToList());
        }
    }
}

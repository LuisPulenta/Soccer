using Newtonsoft.Json;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Soccer.Prism.ViewModels
{
    public class GroupBetPagePlayerViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private ObservableCollection<PredictionResponse3> _predictions;
        private readonly INavigationService _navigationService;
        private PositionResponse _groupBetPlayer;
        private TournamentResponse _tournament;
        private PlayerResponse _player;
        private bool _isRunning;
        private string _search;
        private List<PredictionResponse3> _myPredictions;


        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);
                ShowPredictions();
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public PlayerResponse Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }


        public TournamentResponse Tournament
        {
            get => _tournament;
            set => SetProperty(ref _tournament, value);
        }

        public PositionResponse GroupBetPlayer
        {
            get => _groupBetPlayer;
            set => SetProperty(ref _groupBetPlayer, value);
        }
        public GroupBetPagePlayerViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            GroupBetPlayer = JsonConvert.DeserializeObject<PositionResponse>(Settings.GroupBetPlayer);
            Tournament = JsonConvert.DeserializeObject<TournamentResponse>(Settings.Tournament);
            LoadPredictionsAsync();
            Title = $"Predicciones de {GroupBetPlayer.PlayerResponse.FullName}";
        }

        public ObservableCollection<PredictionResponse3> Predictions
        {
            get => _predictions;
            set => SetProperty(ref _predictions, value);
        }

        private async void LoadPredictionsAsync()
        {
            IsRunning = true;
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

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            Response response = await _apiService.GetListAsync<PredictionResponse3>(url, "api", $"/Predictions/GetPredictionsForUserInOneTournament/{Tournament.Id}/{GroupBetPlayer.PlayerResponse.Id}", "bearer", token.Token);
            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }

            _myPredictions = (List<PredictionResponse3>)response.Result;
            ShowPredictions();
        }

        private void ShowPredictions()
        {
            if (string.IsNullOrEmpty(Search))
            {
                var myListPredictions = _myPredictions.Select(aa => new PredictionResponse3()
                {
                   GoalsLocalPrediction=aa.GoalsLocalReal,
                   GoalsLocalReal=aa.GoalsLocalReal,
                   GoalsVisitorPrediction=aa.GoalsVisitorPrediction,
                   GoalsVisitorReal=aa.GoalsVisitorReal,
                   Id=aa.Id,
                   InitialsLocal= aa.InitialsLocal,
                   InitialsVisitor=aa.InitialsVisitor,
                   LogoPathLocal=aa.LogoPathLocal,
                   LogoPathVisitor=aa.LogoPathVisitor,
                   MatchDate=aa.MatchDate,
                   MatchId=aa.MatchId,
                   NameLocal=aa.NameLocal,
                   NameVisitor=aa.NameVisitor,
                   PlayerId= aa.PlayerId,
                   Points = aa.Points,
                   TournamentId = aa.PlayerId,
                });
                Predictions = new ObservableCollection<PredictionResponse3>(myListPredictions
                    .OrderBy(o => o.MatchDate));
            }
            else
            {
                var myListPredictions = _myPredictions.Select(aa => new PredictionResponse3()
                {
                    Points = aa.Points,
                });
                Predictions = new ObservableCollection<PredictionResponse3>(_myPredictions
                    .OrderBy(o => o.MatchDate)
                    .Where(p => p.NameLocal.ToUpper().Contains(Search.ToUpper()) 
                                || p.NameVisitor.ToUpper().Contains(Search.ToUpper())
                                || p.InitialsVisitor.ToUpper().Contains(Search.ToUpper())
                                || p.InitialsVisitor.ToUpper().Contains(Search.ToUpper())
                                ));
            }
        }
    }
}
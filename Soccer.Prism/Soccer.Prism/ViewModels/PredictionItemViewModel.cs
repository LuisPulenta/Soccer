using Newtonsoft.Json;
using Prism.Commands;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;
using Soccer.Prism;
using System;
using System.Threading.Tasks;

namespace Soccer.Prism.ViewModels
{
    public class PredictionItemViewModel : PredictionResponse
    {
        private readonly IApiService _apiService;
        private DelegateCommand _updatePredictionCommand;

        public PredictionItemViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public DelegateCommand UpdatePredictionCommand => _updatePredictionCommand ?? (_updatePredictionCommand = new DelegateCommand(UpdatePredictionAsync));

        private async void UpdatePredictionAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            string url = App.Current.Resources["UrlAPI"].ToString();
            if (!_apiService.CheckConnection())
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Verifique su conexión a Internet",
                    "Aceptar");
                return;
            }

            PlayerResponse player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            PredictionRequest request = new PredictionRequest
            {
                GoalsLocal = GoalsLocal.Value,
                GoalsVisitor = GoalsVisitor.Value,
                MatchId = Match.Id,
                UserId = new Guid(player.UserId),
            };

            Response response = await _apiService.MakePredictionAsync(url, "api", "/Predictions", request, "bearer", token.Token);

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
            }
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (GoalsLocal == null)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar los Goles del Local",
                    "Aceptar");
                return false;
            }

            if (GoalsVisitor == null)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe ingresar los Goles del Visitante",
                    "Aceptar");
                return false;
            }


            

            if (Match.DateLocal <= DateTime.Now)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Este Partido ya comenzó",
                    "Aceptar");
                return false;
            }

            return true;
        }
    }
}
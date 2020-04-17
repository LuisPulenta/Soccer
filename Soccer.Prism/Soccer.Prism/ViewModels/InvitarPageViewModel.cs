using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Soccer.Common.Helpers;
using Soccer.Common.Models;
using Soccer.Common.Services;

using System.Threading.Tasks;

namespace Soccer.Prism.ViewModels
{
    public class InvitarPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private readonly IRegexHelper _regexHelper;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _recoverCommand;
        private GroupBetResponse _groupBet;
        private PlayerResponse _player;
        private TokenResponse _token;


        public InvitarPageViewModel(INavigationService navigationService, IApiService apiService, IRegexHelper regexHelper)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _regexHelper = regexHelper;
            Player = JsonConvert.DeserializeObject<PlayerResponse>(Settings.Player);
            Token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            GroupBet = JsonConvert.DeserializeObject<GroupBetResponse>(Settings.GroupBet);
            Title = $"Invitar al Grupo {GroupBet.Name}";
            IsEnabled = true;
        }

        public DelegateCommand RecoverCommand => _recoverCommand ?? (_recoverCommand = new DelegateCommand(InvitarAsync));

        public string Email { get; set; }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public GroupBetResponse GroupBet
        {
            get => _groupBet;
            set => SetProperty(ref _groupBet, value);
        }

        public PlayerResponse Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        public TokenResponse Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private async void InvitarAsync()
        {
            bool isValid = await ValidateData();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            AddUserGroupBetRequest request = new AddUserGroupBetRequest
            {
                PlayerId = Player.Id,
                GroupBetId = GroupBet.Id,
                Email = Email
            };

            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.InviteAsync(url, "api", "/GroupBets/Invitar", request,"bearer",Token.Token);
            
            
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }

            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "La invitación fue enviada",
                "Aceptar");
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateData()
        {
            if (string.IsNullOrEmpty(Email) || !_regexHelper.IsValidEmail(Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Ingrese un mail válido.",
                    "Aceptar");
                return false;
            }
            return true;
        }
    }
}

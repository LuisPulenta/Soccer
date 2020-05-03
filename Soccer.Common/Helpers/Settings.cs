using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Soccer.Common.Helpers
{
    public static class Settings
    {
        private const string _groupBet = "groupBet";
        private const string _groupBetPlayer = "groupBetPlayer";
        private const string _player = "player";
        private const string _token = "token";
        private const string _tournament = "tournament";
        private const string _isLogin = "isLogin";
        private const string _isRemembered = "IsRemembered";
        private static readonly string _stringDefault = string.Empty;
        private static readonly bool _boolDefault = false;

        private static ISettings AppSettings => CrossSettings.Current;

        public static string GroupBet
        {
            get => AppSettings.GetValueOrDefault(_groupBet, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_groupBet, value);
        }

        public static string GroupBetPlayer
        {
            get => AppSettings.GetValueOrDefault(_groupBetPlayer, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_groupBetPlayer, value);
        }

        public static string Player
        {
            get => AppSettings.GetValueOrDefault(_player, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_player, value);
        }

        public static bool IsRemembered
        {
            get => AppSettings.GetValueOrDefault(_isRemembered, _boolDefault);
            set => AppSettings.AddOrUpdateValue(_isRemembered, value);
        }

        public static string Token
        {
            get => AppSettings.GetValueOrDefault(_token, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_token, value);
        }

        public static string Tournament
        {
            get => AppSettings.GetValueOrDefault(_tournament, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_tournament, value);
        }

        public static bool IsLogin
        {
            get => AppSettings.GetValueOrDefault(_isLogin, _boolDefault);
            set => AppSettings.AddOrUpdateValue(_isLogin, value);
        }
    }
}
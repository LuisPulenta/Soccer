using Prism;
using Prism.Ioc;
using Soccer.Common.Helpers;
using Soccer.Common.Services;
using Soccer.Prism.ViewModels;
using Soccer.Prism.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Soccer.Prism
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY2MzIyQDMxMzcyZTMzMmUzMFVnNW5KSnM2dTZmRDljWm1RYTduQXFwRmNKSzVPWk1lT1JGSFRySXZCUTA9");
            InitializeComponent();

            if (Settings.IsLogin)
            {
                await NavigationService.NavigateAsync("/SoccerMasterDetailPage/NavigationPage/TournamentsPage");
            }

            else
            {
                await NavigationService.NavigateAsync("/SoccerMasterDetailPage/NavigationPage/LoginPage");
            }
                
            
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.Register<ITransformHelper, TransformHelper>();
            containerRegistry.Register<IRegexHelper, RegexHelper>();
            containerRegistry.Register<IFilesHelper, FilesHelper>();
            containerRegistry.RegisterForNavigation<TournamentsPage, TournamentsPageViewModel>();
            containerRegistry.RegisterForNavigation<GroupsPage, GroupsPageViewModel>();
            containerRegistry.RegisterForNavigation<MatchesPage, MatchesPageViewModel>();
            containerRegistry.RegisterForNavigation<ClosedMatchesPage, ClosedMatchesPageViewModel>();
            containerRegistry.RegisterForNavigation<TournamentTabbedPage, TournamentTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<SoccerMasterDetailPage, SoccerMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<MyPredictionsPage, MyPredictionsPageViewModel>();
            containerRegistry.RegisterForNavigation<MyPositionsPage, MyPositionsPageViewModel>();
            containerRegistry.RegisterForNavigation<ModifyUserPage, ModifyUserPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<MyGroupsPage, MyGroupsPageViewModel>();

            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<RememberPasswordPage, RememberPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<AddGroupBetPage, AddGroupBetPageViewModel>();
            containerRegistry.RegisterForNavigation<PredictionsForTournamentPage, PredictionsForTournamentPageViewModel>();
            containerRegistry.RegisterForNavigation<GroupBetPage, GroupBetPageViewModel>();
            containerRegistry.RegisterForNavigation<InvitarPage, InvitarPageViewModel>();

            containerRegistry.RegisterForNavigation<ClosedPredictionsForTournamentPage, ClosedPredictionsForTournamentPageViewModel>();
            containerRegistry.RegisterForNavigation<PredictionsTabbedPage, PredictionsTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<GroupBetPagePlayer, GroupBetPagePlayerViewModel>();
        }
    }
}

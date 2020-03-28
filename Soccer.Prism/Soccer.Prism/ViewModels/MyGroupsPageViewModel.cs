using Prism.Navigation;

namespace Soccer.Prism.ViewModels
{
    public class MyGroupsPageViewModel : ViewModelBase
    {
        public MyGroupsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Mis Grupos";
        }
    }
}
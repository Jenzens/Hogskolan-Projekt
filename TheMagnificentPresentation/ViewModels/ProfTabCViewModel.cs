using Caliburn.Micro;

namespace TheMagnificentPresentation.ViewModels
{
    public class ProfTabCViewModel : Conductor<IScreen>.Collection.OneActive
    {
        protected override void OnInitialize()
        {
         
            ActivateItem(new ProfileViewModel
            {
                DisplayName = "Användaruppgifter"
            });

            ActivateItem(new UStatsViewModel
            {
                DisplayName = "Statistik"
            });

            ActivateItem(new UProvisionViewModel
            {
                DisplayName = "Provision"
            });   
        }
    }
}
using Caliburn.Micro;

namespace TheMagnificentPresentation.ViewModels
{
    public class IHTabCViewModel : Conductor<IScreen>.Collection.OneActive
    {

        private IScreen _selectedTab;
        public IScreen SelectedTab
        {
            get
            {
                return _selectedTab;
            }
            set
            {
                _selectedTab = value;
                NotifyOfPropertyChange();
            }
        }

        public PFormViewModel CreatePrivateInsuranceTab { get; set; }

        public CFormViewModel CreateCompanyInsuranceTab { get; set; }

        protected override void OnInitialize()
        {
            ActivateItem(SelectedTab = new InsuranceHViewModel(this)
            {
                DisplayName = "Försäkringstagare"
            });

            ActivateItem(new InsuredPersonsViewModel(this)
            {
                DisplayName = "Försäkrade"
            });

            ActivateItem(CreatePrivateInsuranceTab = new PFormViewModel
            {
                DisplayName = "Skapa försäljning (privat)"
            });

            ActivateItem(CreateCompanyInsuranceTab = new CFormViewModel
            {
                DisplayName = "Skapa försäljning (företag)"
            });

        }

    }
}

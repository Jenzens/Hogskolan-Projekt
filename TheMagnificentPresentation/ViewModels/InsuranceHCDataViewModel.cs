using Caliburn.Micro;
using System;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    public class InsuranceHCDataViewModel : Screen
    {

        #region Properties

        public CompanyData Company { get; set; }

        private BindableCollection<Sale> _insuranceList = new BindableCollection<Sale>();

        private object _selectRow;

        private IHTabCViewModel _tab;

        public object SelectedRow
        {
            get { return _selectRow; }
            set { _selectRow = value; NotifyOfPropertyChange(() => _selectRow); }
        }

        public BindableCollection<Sale> InsuranceList
        {
            get { return _insuranceList; }
            set { _insuranceList = value; NotifyOfPropertyChange(() => _insuranceList); }
        }

        #endregion

        public InsuranceHCDataViewModel(IScreen tab, CompanyData company)
        {
            _tab = (IHTabCViewModel) tab;
            Company = company;
            PopulateLists();
        }

        #region Events

        /// <summary>
        /// Event for doubleclicking column row to do event
        /// </summary>
        public void RowSelect_DoubleClick()
        {
            if (SelectedRow == null) return;
            OpenEditView();
        }

        #endregion

        #region Buttons

        public void CreateButton()
        {
            _tab.CreateCompanyInsuranceTab.ChangeCompany(this.Company);
            _tab.SelectedTab = _tab.CreateCompanyInsuranceTab;
            _tab.Refresh(); // force UI update for safty.
        }

        public void EditButton()
        {
            if (SelectedRow == null) return;
            OpenEditView();
        }

        public void ButtonCloseView()
        {
            _tab.CloseItem(this);
        }

        #endregion

        #region local methods 

        private void OpenEditView()
        {
            try
            {
                CFormViewModel newview = null, oldview = null;
                Sale row = ((Sale)SelectedRow);

                foreach (var tabitem in _tab.Items)
                {
                    if (tabitem is CFormViewModel)
                    {
                        if (((CFormViewModel)tabitem).Sale.InsuranceApplicationId == row.InsuranceApplicationId)
                        {
                            oldview = (CFormViewModel)tabitem;
                            break;
                        }
                    }
                }

                if (oldview == null) // create new tab, tab does not exist
                {
                    newview = new CFormViewModel(true, _tab, row)
                    {
                        DisplayName = "(Redigera) Företagsförsäkring: " + row.InsuranceTaker.GetCompanyData.Name,
                    };
                    newview.Refresh();
                    _tab.ActivateItem(newview);
                }
                else // else select the old tab
                {
                    newview = oldview;
                }

                if (newview != null) // if new view is not null, which should not happen in the first place, swap to the window
                    _tab.SelectedTab = newview;
                _tab.Refresh(); // force UI update for safty.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void PopulateLists()
        {
            try
            { 
                _insuranceList = new BindableCollection<Sale>(DataManager.GetSalesWithData(Company.CustomerID));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}

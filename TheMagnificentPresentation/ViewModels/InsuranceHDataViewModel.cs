using Caliburn.Micro;
using System;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    public class InsuranceHDataViewModel : Screen
    {

        #region Properties

        public PersonData Person { get; set; }

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

        public InsuranceHDataViewModel(IScreen tab, PersonData person)
        {
            _tab = (IHTabCViewModel) tab;
            Person = person;
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
            _tab.CreatePrivateInsuranceTab.ChangePerson(this.Person);
            _tab.SelectedTab = _tab.CreatePrivateInsuranceTab;
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
                PFormViewModel newview = null, oldview = null;
                Sale row = ((Sale)SelectedRow);

                foreach (var tabitem in _tab.Items)
                {
                    if (tabitem is PFormViewModel)
                    {
                        if (((PFormViewModel)tabitem).Sale.InsuranceApplicationId == row.InsuranceApplicationId)
                        {
                            oldview = (PFormViewModel)tabitem;
                            break;
                        }
                    }
                }

                if (oldview == null) // create new tab, tab does not exist
                {
                    newview = new PFormViewModel(true, _tab, row)
                    {
                        DisplayName = "(Redigera) Privatförsäkring: " + row.InsuranceTaker.GetPersonData.Name,
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
                _insuranceList = new BindableCollection<Sale>(DataManager.GetSalesWithData(Person.CustomerID));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}

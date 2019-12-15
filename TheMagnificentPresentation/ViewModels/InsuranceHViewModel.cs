using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using TheMagnificentPresentation.Utilities;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using System.Windows;
using System.Diagnostics;

namespace TheMagnificentPresentation.ViewModels
{
    public class InsuranceHViewModel : Screen
    {

        public bool PrintButtonEnabled { get; private set; }

        #region Fields

        BusinessManager businessManager = new BusinessManager();
        private readonly string DOCUMENTS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        #endregion

        #region Properties 

        private object _selectRow;

        private IScreen _tab;

        private string _searchBoxField;

        private BindableCollection<object> _insuranceTakers;

        private BindableCollection<object> _privateInsuranceTakers;

        private BindableCollection<object> _companyInsuranceTakers;

        private BindableCollection<object> _selectedList;

        private int _selectedIndex;

        public string SearchBoxField
        {
            get { return _searchBoxField; }
            set { _searchBoxField = value; SearchBoxField_TextChanged(); NotifyOfPropertyChange(() => _searchBoxField); }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; NotifyOfPropertyChange(() => _selectedIndex); }
        }

        private int search_text_length = 0;

        public BindableCollection<object> SelectedList
        {
            get { return _selectedList; }
            set { _selectedList = value; NotifyOfPropertyChange(() => _selectedList); }
        }

        public BindableCollection<object> CompanyInsuranceTakers
        {
            get { return _companyInsuranceTakers; }
            set { _companyInsuranceTakers = value; NotifyOfPropertyChange(() => _companyInsuranceTakers); }
        }

        public BindableCollection<object> PrivateInsuranceTakers
        {
            get { return _privateInsuranceTakers; }
            set { _privateInsuranceTakers = value; NotifyOfPropertyChange(() => _privateInsuranceTakers); }
        }

        public BindableCollection<object> InsuranceTakers
        {
            get { return _insuranceTakers; }
            set { _insuranceTakers = value; NotifyOfPropertyChange(() => _insuranceTakers);  }
        }

        public object SelectedRow
        {
            get { return _selectRow; }
            set { _selectRow = value; NotifyOfPropertyChange(() => _selectRow); }
        }

        #endregion

        #region Constructor

        public InsuranceHViewModel(IScreen screen)
        {
            _tab = screen;
            PopulateLists();
            SetupPermissions();
        }

        #endregion

        #region Buttons and Selectors

        public void EditButton()
        {
            if (SelectedRow == null) return;
            EditRow();
        }

        public void PrintButton()
        {
            if (SelectedRow == null)
            {
                MessageBox.Show("Vänligen välj en försäkringstagare vars info som skall skrivas ut");
            }
            else 
            {
                var FILENAME = DocumentManager.ExportCustomerInfoToPdf(SelectedRow);
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        Verb = "print",
                        FileName = DOCUMENTS + FILENAME

                    };
                    process.Start();
                    process.Close();
                }
            } 
        }

        public void ChangeIndex(int index)
        {
            _selectedIndex = index;

            if (index == 0)
            {
                _selectedList.Clear();
                _selectedList.AddRange(_insuranceTakers);
            }

            if (index == 1)
            {
                _selectedList.Clear();
                _selectedList.AddRange(_privateInsuranceTakers);
            }

            if (index == 2)
            {
                _selectedList.Clear();
                _selectedList.AddRange(_companyInsuranceTakers);
            }
        }
   
        public void CreateCompanyButtonClick()
        {
            ((IHTabCViewModel)_tab).SelectedTab = ((IHTabCViewModel)_tab).CreateCompanyInsuranceTab;
            ((IHTabCViewModel)_tab).Refresh();
        }

        public void CreatePrivateButtonClick()
        {
            ((IHTabCViewModel)_tab).SelectedTab = ((IHTabCViewModel)_tab).CreatePrivateInsuranceTab;
            ((IHTabCViewModel)_tab).Refresh();
        }

        #endregion

        #region local methods
        
        private void SetupPermissions()
        {
            if ((StaticSession.User.IsUserAuthorized("VD") || StaticSession.User.IsUserAuthorized("Admin")
                || StaticSession.User.IsUserAuthorized("Försäljningschef")))
            {
                PrintButtonEnabled = true;
            }
        }

        /// <summary>
        /// Create or move to the tab with editing the selected insurance
        /// </summary>
        private void EditRow()
        {
            try
            {
                IHTabCViewModel viewcontrol = (IHTabCViewModel)_tab;
                IScreen newview = null, oldview = null;

                if (SelectedRow is PersonData)
                {
                    foreach (var tabitem in viewcontrol.Items)
                    {
                        if (tabitem is InsuranceHDataViewModel)
                        {
                            if (((InsuranceHDataViewModel)tabitem).Person.CustomerID == ((PersonData)SelectedRow).CustomerID)
                            {
                                oldview = tabitem;
                                break;
                            }
                        }
                    }
                    if (oldview == null) // create new tab, tab does not exist
                    {
                        newview = new InsuranceHDataViewModel(_tab, (PersonData)SelectedRow) { DisplayName = "Försäkingstagare: " + ((PersonData)SelectedRow).Name };
                        viewcontrol.ActivateItem(newview);
                    }
                    else // else select the old tab
                    {
                        newview = oldview;
                    }
                }
                else if (SelectedRow is CompanyData)
                {
                    foreach (var tabitem in viewcontrol.Items)
                    {
                        if (tabitem is InsuranceHCDataViewModel)
                        {
                            if (((InsuranceHCDataViewModel)tabitem).Company.CustomerID == ((CompanyData)SelectedRow).CustomerID)
                            {
                                oldview = tabitem;
                                break;
                            }
                        }
                    }
                    if (oldview == null) // create new tab, tab does not exist
                    {
                        newview = new InsuranceHCDataViewModel(_tab, (CompanyData)SelectedRow) { DisplayName = "Försäkingstagare: " + ((CompanyData)SelectedRow).Name };
                        viewcontrol.ActivateItem(newview);
                    }
                    else // else select the old tab
                    {
                        newview = oldview;
                    }
                }
                if (newview != null) // if new view is not null, which should not happen in the first place, swap to the window
                    viewcontrol.SelectedTab = newview;
                viewcontrol.Refresh(); // force UI update for safty.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Söker upp object ur databasen och ändrar listan för att matcha
        /// </summary>
        public void SearchBoxField_TextChanged()
        {
            BindableCollection<object> filter = new BindableCollection<object>();
            if (_searchBoxField.Length > search_text_length)
            {
                search_text_length = _searchBoxField.Length;
            }
            else
            {
                search_text_length = _searchBoxField.Length;
                ChangeIndex(_selectedIndex);
            }
            int y = 0;
            int.TryParse(_searchBoxField, out y);
            foreach (var item in _selectedList)
            {
                if (item is PersonData)
                {
                    if (((PersonData)item).Name.ToLower().Contains(_searchBoxField.ToLower())) {
                        filter.Add(item);
                        continue;
                    }
                    if (y>0)
                    {
                        if (((PersonData)item).PersonNr.ToString().Contains(_searchBoxField))
                        {
                            filter.Add(item);
                        }
                    }
                }
                else if (item is CompanyData)
                {
                    if (((CompanyData)item).Name.ToLower().Contains(_searchBoxField.ToLower()))
                    {
                        filter.Add(item);
                    }
                    if (y > 0)
                    {
                        if (((CompanyData)item).OrgNr.ToString().Contains(_searchBoxField))
                        {
                            filter.Add(item);
                        }
                    }
                }
            }
            _selectedList.Clear();
            _selectedList.AddRange(filter);
        }

        private void PopulateLists()
        {
            _selectedList = new BindableCollection<object>();
            _insuranceTakers = new BindableCollection<object>();
            _privateInsuranceTakers = new BindableCollection<object>();
            _companyInsuranceTakers = new BindableCollection<object>();

            var insuranceTakers = DataManager.GetInsuranceTakers();
            if (insuranceTakers != null)
            {
                foreach (var item in insuranceTakers)
                {
                    if (item.PersonData.Count > 0)
                    {
                        _insuranceTakers.Add(item.PersonData.First());
                        _privateInsuranceTakers.Add(item.PersonData.First());
                    }
                    if (item.CompanyData.Count > 0)
                    {
                        _insuranceTakers.Add(item.CompanyData.First());
                        _companyInsuranceTakers.Add(item.CompanyData.First());
                    }
                }
            }
            _selectedList.Clear();
            _selectedList.AddRange(_insuranceTakers);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event for doubleclicking column row to do event
        /// </summary>
        public void RowSelect_DoubleClick()
        {
            if (SelectedRow == null) return;
            EditRow();
        }

        #endregion
    }
}

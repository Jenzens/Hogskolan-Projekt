using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    class InsuredPersonsViewModel : Screen
    {
        #region Fields

        BusinessManager businessManager = new BusinessManager();

        #endregion

        #region Properties 

        private object _selectRow;

        private IScreen _tab;

        private string _searchBoxField;

        private BindableCollection<object> _insuranceTakers;

        private BindableCollection<object> _adultInsuranceObjects;

        private BindableCollection<object> _childInsuranceObjects;

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

        public BindableCollection<object> SelectedList
        {
            get { return _selectedList; }
            set { _selectedList = value; NotifyOfPropertyChange(() => _selectedList); }
        }

        public BindableCollection<object> ChildInsuranceObject
        {
            get { return _childInsuranceObjects; }
            set { _childInsuranceObjects = value; NotifyOfPropertyChange(() => _childInsuranceObjects); }
        }

        public BindableCollection<object> AdultInsuranceObject
        {
            get { return _adultInsuranceObjects; }
            set { _adultInsuranceObjects = value; NotifyOfPropertyChange(() => _adultInsuranceObjects); }
        }

        public BindableCollection<object> InsuranceTakers
        {
            get { return _insuranceTakers; }
            set { _insuranceTakers = value; NotifyOfPropertyChange(() => _insuranceTakers); }
        }

        public object SelectedRow
        {
            get { return _selectRow; }
            set { _selectRow = value; NotifyOfPropertyChange(() => _selectRow); }
        }

        #endregion

        #region Constructor

        public InsuredPersonsViewModel(IScreen screen)
        {
            _tab = screen;
            PopulateLists();
        }

        #endregion

        #region Buttons and Selectors

        public void PrintProspectButton()
        {
            MessageBox.Show("Not done yet");
        }

        public void ChangeIndex(int index)
        {
            _selectedIndex = index;

            if (index == 0)
            {
                _selectedList.Clear();
                _selectedList.AddRange(_insuranceTakers);
                Console.WriteLine("Index changed");
            }

            if (index == 1)
            {
                _selectedList.Clear();
                _selectedList.AddRange(_childInsuranceObjects);
                Console.WriteLine("Index changed");
            }

            if (index == 2)
            {
                _selectedList.Clear();
                _selectedList.AddRange(_adultInsuranceObjects);
                Console.WriteLine("Index changed");
            }
        }

        public void CreateCompanyButtonClick()
        {
            Console.WriteLine("??");
        }

        public void CreatePrivateButtonClick()
        {
            Console.WriteLine("??");
        }

        #endregion

        #region search method

        private int search_text_length = 0;
        /// <summary>
        /// Looks for changes in the search field
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
                if (((PersonData)item).Name.ToLower().Contains(_searchBoxField.ToLower()))
                {
                    filter.Add(item);
                    continue;
                }
                if (y > 0)
                {
                    if (((PersonData)item).PersonNr.ToString().Contains(_searchBoxField))
                    {
                        filter.Add(item);
                    }
                }
            }
            _selectedList.Clear();
            _selectedList.AddRange(filter);
        }

        #endregion

        #region local methods
        /// <summary>
        /// Populates list
        /// </summary>
        private void PopulateLists()
        {
            _selectedList = new BindableCollection<object>();
            _insuranceTakers = new BindableCollection<object>();
            _adultInsuranceObjects = new BindableCollection<object>();
            _childInsuranceObjects = new BindableCollection<object>();

            var insuranceTakers = DataManager.GetInsuranceObjects();

            var currentTime = DateTime.Now;
            string day = currentTime.Day.ToString();
            if (day.Length == 1)
                day = "0" + currentTime.Day.ToString();
            string month = currentTime.Month.ToString();
            if (month.Length == 1)
                month = "0" + currentTime.Month.ToString();
            long adult_time = long.Parse((currentTime.Year - 18) + month + day);

            if(insuranceTakers != null)
            {
                foreach (var item in insuranceTakers)
                {
                    if (item.PersonData.Count > 0)
                    {
                        _insuranceTakers.Add(item.PersonData.First());
                        if (long.Parse(item.PersonData.First().PersonNr.ToString().Substring(0, item.PersonData.First().PersonNr.ToString().Length-4)) > adult_time)
                        {
                            _childInsuranceObjects.Add(item.PersonData.First());
                        }
                        else
                        {
                            _adultInsuranceObjects.Add(item.PersonData.First());
                        }
                    }
                }
            _selectedList.Clear();
            _selectedList.AddRange(_insuranceTakers);
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Event for doubleclicking column row to do event
        /// </summary>
        public void RowSelect_DoubleClick()
        {
            try
            {
                Console.WriteLine(SelectedRow.GetType());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}

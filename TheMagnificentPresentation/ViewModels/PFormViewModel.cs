using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    public class PFormViewModel : Screen
    {
        #region Fields

        BusinessManager businessManager = new BusinessManager();

        #endregion

        #region Properties

        private IHTabCViewModel _tab;

        private bool _edit;

        private bool _reverseedit;

        private int? _fixedCommision;

        private BindableCollection<Users> _users;

        private BindableCollection<Insurance> _insurances;

        private BindableCollection<Insurance> _insuranceAddons;

        private Users _user;

        private Sale _sale;

        private PaymentTypes _selectedPaymentType;

        private Insurance _selectedInsurance;

        private Insurance _selectedAddonInsurance;

        private InsuranceObject _insuranceObject;

        private InsuranceTaker _insuranceTaker;

        private InsuranceApplicationRow _insuranceAppRow;

        private InsuranceApplicationRow _insuranceAddonAppRow;

        private PersonData _personData;

        private long _customerPersonNumberSearch;

        private BindableCollection<PaymentTypes> _paymentTypes;

        private PersonData _insuredPersonData;

        private InsuranceBaseValues _selectedBaseValue;

        private InsuranceBaseValues _selectedAddonBaseValue;

        private BindableCollection<InsuranceBaseValues> _currentInsuranceBaseValues;

        private BindableCollection<InsuranceBaseValues> _currentInsuranceAddonBaseValues;

        public bool HasCurrentAddonInsuranceBaseValues
        {
            get {
                if (_currentInsuranceAddonBaseValues == null) return false;
                return _currentInsuranceAddonBaseValues.Count() > 0; }
        }

        public BindableCollection<InsuranceBaseValues> CurrentInsuranceAddonBaseValues
        {
            get { return _currentInsuranceAddonBaseValues; }
            set { _currentInsuranceAddonBaseValues = value; NotifyOfPropertyChange(() => _currentInsuranceAddonBaseValues); }
        }

        public BindableCollection<InsuranceBaseValues> CurrentInsuranceBaseValues
        {
            get { return _currentInsuranceBaseValues; }
            set { _currentInsuranceBaseValues = value; NotifyOfPropertyChange(() => _currentInsuranceBaseValues); }
        }

        public InsuranceBaseValues SelectedAddonBaseValue
        {
            get { return _selectedAddonBaseValue; }
            set { _selectedAddonBaseValue = value; NotifyOfPropertyChange(() => _selectedAddonBaseValue); }
        }

        public InsuranceBaseValues SelectedBaseValue
        {
            get { return _selectedBaseValue; }
            set { _selectedBaseValue = value; NotifyOfPropertyChange(() => _selectedBaseValue); }
        }

        public Insurance SelectedInsurance
        {
            get { return _selectedInsurance; }
            set { _selectedInsurance = value; NotifyOfPropertyChange(() => _selectedInsurance); }
        }

        public PaymentTypes SelectedPaymentType
        {
            get { return _selectedPaymentType; }
            set { _selectedPaymentType = value; NotifyOfPropertyChange(() => _selectedPaymentType); }
        }

        public Insurance SelectedAddonInsurance
        {
            get { return _selectedAddonInsurance; }
            set { _selectedAddonInsurance = value; NotifyOfPropertyChange(() => _selectedAddonInsurance); }
        }

        public PersonData InsuredPersonData
        {
            get { return _insuredPersonData; }
            set { _insuredPersonData = value; NotifyOfPropertyChange(() => _insuredPersonData); }
        }

        public BindableCollection<PaymentTypes> PaymentTypes
        {
            get { return _paymentTypes; }
            set { _paymentTypes = value; NotifyOfPropertyChange(() => _paymentTypes); }
        }

        public BindableCollection<Insurance> Insurances
        {
            get { return _insurances; }
            set { _insurances = value; NotifyOfPropertyChange(() => _insurances); }
        }

        public BindableCollection<Insurance> InsuranceAddons
        {
            get { return _insuranceAddons; }
            set { _insuranceAddons = value; NotifyOfPropertyChange(() => _insuranceAddons); }
        }

        public BindableCollection<Users> Users
        {
            get { return _users; }
            set { _users = value; NotifyOfPropertyChange(() => _users); }
        }

        public long CustomerPersonNumberSearch
        {
            get { return _customerPersonNumberSearch; }
            set { _customerPersonNumberSearch = value; NotifyOfPropertyChange(() => _customerPersonNumberSearch); }
        }

        public PersonData PersonData
        {
            get { return _personData; }
            set { _personData = value; NotifyOfPropertyChange(() => _personData); }
        }

        public InsuranceTaker InsuranceTaker
        {
            get { return _insuranceTaker; }
            set { _insuranceTaker = value; NotifyOfPropertyChange(() => _insuranceTaker); }
        }

        public InsuranceObject InsuranceObject
        {
            get { return _insuranceObject; }
            set { _insuranceObject = value; NotifyOfPropertyChange(() => _insuranceObject); }
        }

        public Sale Sale
        {
            get { return _sale; }
            set { _sale = value; NotifyOfPropertyChange(() => _sale); }
        }

        public Users User
        {
            get { return _user; }
            set { _user = value; NotifyOfPropertyChange(() => _user); }
        }

        public InsuranceApplicationRow InsuranceAppRow
        {
            get { return _insuranceAppRow; }
            set { _insuranceAppRow = value; NotifyOfPropertyChange(() => _insuranceAppRow); }
        }

        public InsuranceApplicationRow InsuranceAddonAppRow
        {
            get { return _insuranceAddonAppRow; }
            set { _insuranceAddonAppRow = value; NotifyOfPropertyChange(() => _insuranceAddonAppRow); }
        }

        public bool Edit
        {
            get { return _edit; }
            set { _edit = value; NotifyOfPropertyChange(() => _edit); }
        }

        public bool ReversedEdit
        {
            get { return _reverseedit; }
            set { _reverseedit = value; NotifyOfPropertyChange(() => _reverseedit); }
        }

        public int? FixedCommision
        {
            get { return _fixedCommision; }
            set { _fixedCommision = value; NotifyOfPropertyChange(() => _fixedCommision); }
        }

        #endregion

        #region Constructor

        public PFormViewModel()
        {
            _edit = false;
            _reverseedit = !_edit;
            PopulateLists();
            ResetModelData();
        }

        public PFormViewModel(bool edit, IHTabCViewModel tab, Sale row)
        {
            _edit = edit;
            _reverseedit = !_edit;
            _tab = tab;
            PopulateLists();
            ResetModelData();

            // set value of row
            ChangeUser(row.Users);
            Sale = row;
            ChangePerson(row.InsuranceTaker.GetPersonData);
            InsuredPersonData = row.InsuranceObject.FirstPerson;

            InsuranceAppRow = row.FirstAPItem;
            ChangeSelectedInsurance(Insurances.Where(i => i.InsuranceId == InsuranceAppRow.InsuranceId).First());
            SelectedBaseValue = new InsuranceBaseValues
            {
                BaseValue = row.FirstAPItem.BaseValue
            };

            if (row.SecondAPItem != null)
            {
                InsuranceAddonAppRow = row.SecondAPItem;
                ChangeSelectedInsuranceAddon(InsuranceAddons.Where(i => i.InsuranceId == InsuranceAddonAppRow.InsuranceId).First());
                SelectedAddonBaseValue = new InsuranceBaseValues
                {
                    BaseValue = row.SecondAPItem.BaseValue
                };
            }

            if (row.InsuranceFixedComision != null)
            {
                FixedCommision = row.InsuranceFixedComision.Commision;
            }

            ChangePaymentType(PaymentTypes.Where(i => i.PaymentId == row.PaymentType).First());
        }

        public void ResetModelData()
        {
            _sale = new Sale();
            ChangeUser(StaticSession.User);
            _personData = new PersonData
            {
                ZipCity = new ZipCity()
            };
            _insuredPersonData = new PersonData();
            _insuranceTaker = new InsuranceTaker();
            _insuranceObject = new InsuranceObject();
            _insuranceAppRow = new InsuranceApplicationRow();
            _insuranceAddonAppRow = new InsuranceApplicationRow();
            _insurances = new BindableCollection<Insurance>(DataManager.GetMainInsurances());
            _insuranceAddons = new BindableCollection<Insurance>(DataManager.GetAddonInsurances());
            _paymentTypes = new BindableCollection<PaymentTypes>(DataManager.GetPaymentTypes());
            _selectedPaymentType = null;
            _selectedInsurance = null;
            _selectedAddonInsurance = null;
            _fixedCommision = null;
            this.Refresh();
        }

        #endregion

        #region Change Events

        public void PersonNrChange(System.Windows.Controls.TextBox data)
        {
            if (data.Text.Length == 12)
            {
                long number = 0;
                long.TryParse(data.Text, out number);
                if (number > 0)
                {
                    PersonData t = DataManager.GetPerson(number);
                    if (t != null)
                    {
                        ChangePerson(t);
                        this.Refresh();
                    }
                }
            }
        }

        public void PersonObjectNrChange(System.Windows.Controls.TextBox data)
        {
            if (data.Text.Length == 12)
            {
                long number = 0;
                long.TryParse(data.Text, out number);
                if (number > 0)
                {
                    PersonData t = DataManager.GetPerson(number);
                    if (t != null)
                    {
                        InsuredPersonData = t;
                        this.Refresh();
                    }
                }
            }
        }

        #endregion  

        #region Methods & Buttons
        /// <summary>
        /// Changes selected person
        /// </summary>
        /// <param name="person"></param>
        public void ChangePerson(PersonData person)
        {
            PersonData = person;
        }
        /// <summary>
        /// Changes user
        /// </summary>
        /// <param name="_touser"></param>
        public void ChangeUser(Users _touser)
        {
            if (_touser == null)
            {
                _user = new Users();
                return;
            }
            var u = _users.Where(x => x.AgentNumber == _touser.AgentNumber);
            if (u.Count() > 0)
            {
                _user = u.First();
            }
            else
            {
                _user = new Users();
            }
        }
        /// <summary>
        /// Savebutton
        /// 
        /// </summary>
        public void SaveButton()
        {
            _insuranceAppRow.Insurance = _selectedInsurance;
            if (_selectedBaseValue != null)
            {
                if (_selectedBaseValue.BaseValue.HasValue)
                    _insuranceAppRow.BaseValue = _selectedBaseValue.BaseValue.Value;
            }
            _insuranceAddonAppRow.Insurance = _selectedAddonInsurance;
            if (_selectedAddonBaseValue != null)
            {
                if (_selectedAddonBaseValue.BaseValue.HasValue)
                    _insuranceAddonAppRow.BaseValue = _selectedAddonBaseValue.BaseValue.Value;
            }

            List<InsuranceApplicationRow> _appRows = new List<InsuranceApplicationRow>
            {
                _insuranceAppRow,
                _insuranceAddonAppRow
            };

            _insuranceObject.PersonData.Add(_insuredPersonData);
            _insuranceTaker.PersonData.Add(_personData);

            if (_selectedPaymentType != null)
                _sale.PaymentType = _selectedPaymentType.PaymentId;

            if (_fixedCommision >= 0)
            {
                _sale.InsuranceFixedComision = new InsuranceFixedComision
                {
                    Commision = (int)_fixedCommision
                };
            }

            if (Edit)
            {
                EditSale(_appRows);
            }
            else
            {
                CreateNewSale(_appRows);
            }
        }

        public void EditSale(List<InsuranceApplicationRow> _appRows)
        {
            var errors = businessManager.UpdateSale(_user, _sale, _appRows, _insuranceTaker, _insuranceObject);
            if (errors.Count == 0)
            {
                MessageBox.Show("Försäljning uppdaterad!");
            }
            else
            {
                MessageBox.Show(String.Join("\n", errors.ToArray()));
            }

            _insuranceObject.PersonData.Clear();
            _insuranceTaker.PersonData.Clear();
        }

        public void CreateNewSale(List<InsuranceApplicationRow> _appRows)
        {

            var errors = businessManager.CreateSale(_user, _sale, _appRows, _insuranceTaker, _insuranceObject);
            if (errors.Count == 0)
            {
                MessageBox.Show("Försäljning registrerad!");
                ResetModelData();
            }
            else
            {
                MessageBox.Show(String.Join("\n", errors.ToArray()));
            }
        }

        public void ChangeSelectedInsuranceAddon(Insurance addon)
        {
            _selectedAddonInsurance = addon;
            ChangeSelectedAddonBaseValue(new InsuranceBaseValues());
            CurrentInsuranceAddonBaseValues = new BindableCollection<InsuranceBaseValues>();
            if (SelectedAddonInsurance != null)
            {
                if (Sale.StartDate.HasValue)
                {
                    CurrentInsuranceAddonBaseValues.AddRange(SelectedAddonInsurance.InsuranceBaseValues.Where(x => x.StartDate == Sale.StartDate.Value.Year).ToList());
                }
                else
                {
                    CurrentInsuranceAddonBaseValues.AddRange(SelectedAddonInsurance.InsuranceBaseValues.Where(x => x.StartDate == DateTime.Now.Year).ToList());
                }
            }
            this.Refresh();
        }
        
        public void ChangeSelectedInsurance(Insurance insurance)
        {
            _selectedInsurance = insurance;
            ChangeSelectedBaseValue(null);
            CurrentInsuranceBaseValues = new BindableCollection<InsuranceBaseValues>();
            if (SelectedInsurance != null)
            {
                if (Sale.StartDate.HasValue)
                {
                    CurrentInsuranceBaseValues.AddRange(SelectedInsurance.InsuranceBaseValues.Where(x => x.StartDate == Sale.StartDate.Value.Year).ToList());
                }
                else
                {
                    CurrentInsuranceBaseValues.AddRange(SelectedInsurance.InsuranceBaseValues.Where(x => x.StartDate == DateTime.Now.Year).ToList());
                }
            }
            this.Refresh();
        }

        public void ChangeSelectedBaseValue(InsuranceBaseValues basevalue)
        {
            _selectedBaseValue = basevalue;
            this.Refresh();
        }

        public void ChangeSelectedAddonBaseValue(InsuranceBaseValues basevalue)
        {
            _selectedAddonBaseValue = basevalue;
            this.Refresh();
        }

        public void ChangePaymentType(PaymentTypes paymenttype)
        {
            _selectedPaymentType = paymenttype;
        }

        public void ResetFormButton()
        {
            if (Edit)
            {
                _tab.CloseItem(this);
            }
            else
            {
                ResetModelData();
            }
        }

        #endregion

        #region private local methods
        /// <summary>
        /// Populates list
        /// </summary>
        private void PopulateLists()
        {
            _users = new BindableCollection<Users>();
            foreach (var user in DataManager.GetUsers())
            {
                if (user.Roles.Count > 0)
                {
                    foreach (var role in user.Roles)
                    {
                        if (role.Permission.Equals("Säljare"))
                        {
                            _users.Add(user);
                        }
                    }
                }
            }

        }

        #endregion

    }
}

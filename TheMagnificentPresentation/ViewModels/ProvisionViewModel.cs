using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    public class ProvisionViewModel : Screen
    {
        #region Fields

        private readonly string DOCUMENTS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        #endregion

        #region Properties

        private int _childSumAcqValue;

        private int _adultSumAcqValue;

        private int _sumOfChildAdult;

        private int _lifeSumAcqValue;

        private int _provSo;

        private int _provLife;

        private int _provOther;

        private int _sumProv;

        private int _vacationPercentage;

        private int _vacationValue;

        private int _perliminaryTaxes;

        private int _otherSumAcqValue;

        private int _deductTax;

        private int _payout;

        private Users _user;

        private BindableCollection<Users> _userList;

        private int _selectedIndex;

        private List<int> _dateYearPeriod;

        private List<KeyValuePair<string, int>> _datePeriod;

        public List<KeyValuePair<string, int>> DatePeriod
        {
            get { return _datePeriod; }
            set { _datePeriod = value; NotifyOfPropertyChange( ()=> _datePeriod); }
        }

        public List<int> DateYearPeriod
        {
            get { return _dateYearPeriod; }
            set { _dateYearPeriod = value; NotifyOfPropertyChange(()=> _dateYearPeriod); }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; NotifyOfPropertyChange(() => _selectedIndex); }
        }

        public BindableCollection<Users> UserList
        {
            get { return _userList; }
            set { _userList = value; NotifyOfPropertyChange(() => _userList); }
        }

        public Users User
        {
            get { return _user; }
            set { _user = value; NotifyOfPropertyChange(() => _user); }
        }

        public int Payout
        {
            get { return _payout; }
            set { _payout = value; NotifyOfPropertyChange(() => _payout); }
        }

        public int DeductTax
        {
            get { return _deductTax; }
            set { _deductTax = value; NotifyOfPropertyChange(() => _deductTax); }
        }

        public int PerliminaryTaxes // User.tax! 
        {
            get { return _perliminaryTaxes; }
            set { _perliminaryTaxes = value; NotifyOfPropertyChange(() => _perliminaryTaxes); }
        }

        public int VacationValue
        {
            get { return _vacationValue; }
            set { _vacationValue = value; NotifyOfPropertyChange(() => _vacationValue); }
        }

        public int VacationPercentage
        {
            get { return _vacationPercentage; }
            set { _vacationPercentage = value; NotifyOfPropertyChange(() => _vacationPercentage); }
        }

        public int SumProv
        {
            get { return _sumProv; }
            set { _sumProv = value; NotifyOfPropertyChange(() => _sumProv); }
        }

        public int ProvOther
        {
            get { return _provOther; }
            set { _provOther = value; NotifyOfPropertyChange(() => _provOther ); }
        }

        public int ProvLife
        {
            get { return _provLife; }
            set { _provLife = value; NotifyOfPropertyChange(() => _provLife); }
        }

        public int ProvSo
        {
            get { return _provSo; }
            set { _provSo = value; NotifyOfPropertyChange(() => _provSo); }
        }

        public int OtherSumAcqValue
        {
            get { return _otherSumAcqValue; }
            set { _otherSumAcqValue = value; NotifyOfPropertyChange(() => _otherSumAcqValue); }
        }

        public int LifeSumAcqValue
        {
            get { return _lifeSumAcqValue; }
            set { _lifeSumAcqValue = value; NotifyOfPropertyChange(() => _lifeSumAcqValue); }
        }

        public int SumOfChildAdult
        {
            get { return _sumOfChildAdult; }
            set { _sumOfChildAdult = value ; NotifyOfPropertyChange(() => _sumOfChildAdult); }
        }

        public int AdultSumAcqValue
        {
            get { return _adultSumAcqValue; }
            set { _adultSumAcqValue = value; NotifyOfPropertyChange(() => _adultSumAcqValue); }
        }

        public int ChildSumAcqValue
        {
            get { return _childSumAcqValue; }
            set { _childSumAcqValue = value; NotifyOfPropertyChange(() => _childSumAcqValue); }
        }

        public int SelectedYear { get; private set; }
        public int SelectedMonth { get; private set; }
        public bool PrintButtonEnabled { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for ProvisionViewModel
        /// </summary>
        public ProvisionViewModel()
        {
            _user = new Users();
            _dateYearPeriod = new List<int>();

            PopulateYears();
            PopulateMonths();
            PopulateLists();
            if (_userList.First() != null)
            {
                _user = _userList.First();
            }
            SelectedYear = 2019;
            SelectedMonth = 1;
            CalculateCommission();
            SetupPermissions();

        }

        #endregion

        #region Buttons & Selectors

        public void PrintProvisionButton()
        {
            var file = DocumentManager.ExportProvisionToPdf(_user, ChildSumAcqValue, AdultSumAcqValue, LifeSumAcqValue, OtherSumAcqValue, ProvSo, ProvLife, ProvOther, VacationValue, VacationPercentage, PerliminaryTaxes, DeductTax, SelectedMonth, SelectedYear, SumProv, Payout);

            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    Verb = "print",
                    FileName = DOCUMENTS + file

                };
                process.Start();
                process.Close();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Permission to print commission for sales
        /// </summary>
        private void SetupPermissions()
        {
            if ((StaticSession.User.IsUserAuthorized("VD") || StaticSession.User.IsUserAuthorized("Admin")
                || StaticSession.User.IsUserAuthorized("Ekonomiassistent")))
            {
                PrintButtonEnabled = true;
            }
        }

        /// <summary>
        /// Calculates commission and populates fields
        /// </summary>
        /// <param name="user"></param>
        public void CalculateCommission()
        {
            var commissionValues = DataManager.GetCommissionValues();

            var vacrate = DataManager.GetVactionRateOfYear(SelectedYear);
            var vacationMargin = 0.0;
            var vacationPercentage = 0.0;

            if (vacrate != null)
            {
                vacationMargin = vacrate.VacationVariable / 100;
                vacationPercentage = vacrate.VacationRate1 / 100;
                _vacationPercentage = _vacationPercentage = (int)vacrate.VacationRate1;
            }

            _childSumAcqValue = CommissionManager.GetUserAcqSumOfInsuranceType("Barn", _user, SelectedMonth, SelectedYear);
            _adultSumAcqValue = CommissionManager.GetUserAcqSumOfInsuranceType("Vuxen", _user, SelectedMonth, SelectedYear);
            _otherSumAcqValue = CommissionManager.GetUserAcqSumOfInsuranceType("other", _user, SelectedMonth, SelectedYear);
            _lifeSumAcqValue = CommissionManager.GetUserLifeBaseSum(_user, SelectedMonth, SelectedYear);
            _sumOfChildAdult = _childSumAcqValue + _adultSumAcqValue;

            var commissionChildPerMonthTotAcc = 0.0;
            var commissionAdultPerMonthTotAcc = 0.0;

            var total_akv = (_childSumAcqValue + _adultSumAcqValue);

            var commissions = commissionValues.Where(x => x.LowestAcqValue <= _sumOfChildAdult && x.HighestAcqValue >= _sumOfChildAdult && x.CommisionYear == SelectedYear);
            if (commissions.Count() > 0)
            {
                commissionChildPerMonthTotAcc = commissions.First().ChildAcqVariable;
                commissionAdultPerMonthTotAcc = commissions.First().AdultAcqVariable;
            }

            var tax = User.Taxrate / 100;

            var child = (_childSumAcqValue * commissionChildPerMonthTotAcc) * (1 - vacationMargin);
            var adult = (_adultSumAcqValue * commissionAdultPerMonthTotAcc) * (1 - vacationMargin);
            var total = (child + adult);
            _provSo = (int)total;
            var life = CommissionManager.GetUserAcqSumOfInsuranceType("Liv", _user, SelectedMonth, SelectedYear) * (1 - vacationMargin);
            _provLife = (int)life;
            var misc = _otherSumAcqValue * (1 - vacationMargin);
            _provOther = (int)misc;
            var commission = total + life + misc;
            _sumProv = (int)commission;
           
            var vacation = commission * vacationPercentage;
            _vacationValue = (int)vacation;
            var taxes = (commission + vacation) * tax;
            _deductTax = (int)taxes;
            var payout = (commission + vacation - taxes);
            _payout = (int)payout;
            Refresh();
        }

        /// <summary>
        /// Changes index
        /// </summary>
        /// <param name="index"></param>
        public void ChangeIndex(int index)
        {
            _selectedIndex = index;
            CalculateCommission();
            Refresh();
        }
        /// <summary>
        /// Changes selected user
        /// </summary>
        public void ChangeUser(int index)
        {
            _user = _userList[index];
            _user.ZipCity = _userList[index].ZipCity;
            CalculateCommission();
            Refresh();
        }
        /// <summary>
        /// Changes selected year
        /// </summary>
        /// <param name="index"></param>
        public void ChangeYear(int index)
        {
            SelectedYear = DateYearPeriod[index];
            Console.WriteLine(SelectedYear);
            CalculateCommission();
            Refresh();
        }
        /// <summary>
        /// Changes selected month
        /// </summary>
        /// <param name="index"></param>
        public void ChangeMonth(int index)
        {
            SelectedMonth = DatePeriod[index].Value;
            Console.WriteLine(SelectedMonth);
            CalculateCommission();
            Refresh();
        }
        /// <summary>
        /// Populates the list with users
        /// </summary>
        public void PopulateLists()
        {
            _userList = new BindableCollection<Users>();
            foreach (var user in DataManager.GetUsersWithSale())
            {
                _userList.Add(user);
            }

        }
        /// <summary>
        /// Adds years to the year selector
        /// </summary>
        public void PopulateYears()
        {
            for (int i = DateTime.UtcNow.Year - 2; i <= DateTime.UtcNow.Year + 2; i++)
            {
                _dateYearPeriod.Add(i);
            }
        }
        /// <summary>
        /// Adds months to the month selector
        /// </summary>
        public void PopulateMonths()
        {
            _datePeriod = new List<KeyValuePair<string, int>>()
            {
                new KeyValuePair<string, int>("Jan", 1),
                new KeyValuePair<string, int>("Feb", 2),
                new KeyValuePair<string, int>("Mar", 3),
                new KeyValuePair<string, int>("Apr", 4),
                new KeyValuePair<string, int>("Maj", 5),
                new KeyValuePair<string, int>("Juni", 6),
                new KeyValuePair<string, int>("Juli", 7),
                new KeyValuePair<string, int>("Aug", 8),
                new KeyValuePair<string, int>("Sep", 9),
                new KeyValuePair<string, int>("Okt", 10),
                new KeyValuePair<string, int>("Nov", 11),
                new KeyValuePair<string, int>("Dec", 12),
            };
        }

        #endregion
    }
}

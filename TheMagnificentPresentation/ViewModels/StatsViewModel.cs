using Caliburn.Micro;
using System;
using System.Windows;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;
using static TheMagnificentModels.Utilities.SellingStatistics;

namespace TheMagnificentPresentation.ViewModels
{
    class StatsViewModel : Screen
    {
        public bool PrintAllButtonEnabled { get; private set; }
        public bool PrintSelectedButtonEnabled { get; private set; }
        public bool ExportTrendIsEnabled { get; private set; }

        private BusinessManager businessManager = new BusinessManager();

        #region Properties 

        private BindableCollection<StatisticSalesPerMonth> _sellingstatistics;

        private BindableCollection<Users> _sellers;

        private BindableCollection<int> _years;

        private int _selectedYear;

        private int _selectedTrendYear;

        private Users _selectedUser;

        private Users _selectedTrendUser;

        public BindableCollection<StatisticSalesPerMonth> SellingStatistics
        {
            get { return _sellingstatistics; }
            set { _sellingstatistics = value; NotifyOfPropertyChange(() => _sellingstatistics); }
        }

        public int SelectedYear
        {
            get { return _selectedYear; }
            set { _selectedYear = value; NotifyOfPropertyChange(() => _selectedYear); }
        }

        public int SelectedTrendYear
        {
            get { return _selectedTrendYear; }
            set { _selectedTrendYear = value; NotifyOfPropertyChange(() => _selectedTrendYear); }
        }

        public Users SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; NotifyOfPropertyChange(() => _selectedUser); }
        }

        public Users SelectedTrendUser
        {
            get { return _selectedTrendUser; }
            set { _selectedTrendUser = value; NotifyOfPropertyChange(() => _selectedTrendUser); }
        }

        public BindableCollection<Users> SellerList
        {
            get { return _sellers; }
            set { _sellers = value; NotifyOfPropertyChange(() => _sellers); }
        }

        public BindableCollection<int> YearList
        {
            get { return _years; }
            set { _years = value; NotifyOfPropertyChange(() => _years); }
        }

        #endregion

        public StatsViewModel()
        {
            _sellers    = new BindableCollection<Users>();
            _years      = new BindableCollection<int>();
            _sellingstatistics = new BindableCollection<StatisticSalesPerMonth>();
            SetupPermissions();

            int startYear = 2018;
            for (int i = startYear; i <= DateTime.Now.Year; i++)
            {
                _years.Add(i);
            }

            var sellers = businessManager.GetSellers();
            foreach(var seller in sellers)
            {
                _sellers.Add(seller);
            }
        }

        private void SetupPermissions()
        {
            if ((StaticSession.User.IsUserAuthorized("VD") || StaticSession.User.IsUserAuthorized("Admin")
                || StaticSession.User.IsUserAuthorized("Försäljningschef")))
            {
                PrintAllButtonEnabled = true;
                PrintSelectedButtonEnabled = true;
                ExportTrendIsEnabled = true;
            }
        }

        public void ChangeSelectedYear(int index)
        {
            _selectedYear = index;
            _sellingstatistics.Clear();
            if (_selectedUser != null)
            {
                _sellingstatistics.AddRange(businessManager.GetStatisticsOfUser(_selectedUser, _selectedYear));
            }
            else
            {
                _sellingstatistics.AddRange(businessManager.GetStatisticsOfAllUsers(_selectedYear));
            }
        }

        public void ChangeSelectedUser(Users index)
        {
            _selectedUser = index;
            if (_selectedYear > 0)
            {
                _sellingstatistics.Clear();
                _sellingstatistics.AddRange(businessManager.GetStatisticsOfUser(_selectedUser, _selectedYear));
            }
        }

        public void ChangeSelectedTrendUser(Users index)
        {
            _selectedTrendUser = index;
        }

        public void ChangeSelectedTrendYear(int index)
        {
            _selectedTrendYear = index;
        }

        public void ExportTrend()
        {
            if (_selectedTrendUser != null && _selectedTrendYear > 0)
                MessageBox.Show(GenerateTrend(_selectedTrendUser, _selectedTrendYear));
            else
                MessageBox.Show("Period eller säljare inte valts");
        }

        public void PrintStatisticsOfYearOfSelectedUser()
        {
            if (_selectedUser != null && _selectedYear > 0)
                businessManager.PrintStatisticOfUser(_selectedUser, _selectedYear);
            else
                MessageBox.Show("Period eller säljare inte valts");
        }
        
        public void PrintStatisticsOfYearAllUsers()
        {
            if (_selectedYear > 0)
                businessManager.PrintStatisticOfAllUsers(_selectedYear);
            else
                MessageBox.Show("Period måste väljas");
        }
    }
}

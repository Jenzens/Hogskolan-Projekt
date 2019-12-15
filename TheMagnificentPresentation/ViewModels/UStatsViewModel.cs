using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;
using static TheMagnificentModels.Utilities.SellingStatistics;

namespace TheMagnificentPresentation.ViewModels
{
    public class UStatsViewModel : Screen
    {
        private BusinessManager businessManager = new BusinessManager();

        #region Properties 

        private BindableCollection<StatisticSalesPerMonth> _sellingstatistics;

        private BindableCollection<int> _years;

        private int _selectedYear;

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

        public BindableCollection<int> YearList
        {
            get { return _years; }
            set { _years = value; NotifyOfPropertyChange(() => _years); }
        }

        #endregion

        public UStatsViewModel()
        {
            _years = new BindableCollection<int>();
            _sellingstatistics = new BindableCollection<StatisticSalesPerMonth>();

            int startYear = 2018;
            for (int i = startYear; i <= DateTime.Now.Year; i++)
            {
                _years.Add(i);
            }
        }
        /// <summary>
        /// Changes the selected year
        /// </summary>
        /// <param name="index"></param>
        public void ChangeSelectedYear(int index)
        {
            if (!StaticSession.User.IsUserAuthorized("säljare"))
            {
                MessageBox.Show("Du har inte en säljar roll och kan därför inte se någon statistik.");
                return;
            }
            _selectedYear = index;
            if (StaticSession.User != null)
            {
                _sellingstatistics.Clear();
                _sellingstatistics.AddRange(businessManager.GetStatisticsOfUser(StaticSession.User, _selectedYear));
            }
        }

    }
}

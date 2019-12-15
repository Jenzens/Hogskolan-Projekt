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
using static TheMagnificentPresentation.ViewModels.DefaultValuesViewModel;

namespace TheMagnificentPresentation.ViewModels
{
    public class DefaultValuesViewModel : Screen
    {
        #region Fields

        private BusinessManager businessManager;

        #endregion

        #region Props

        public bool EditBtnEnabled { get; private set; }
        public bool SaveBtnEnabled { get; private set; }
        public bool CancelBtnEnabled { get; private set; }

        private InsuranceDataValues _selectedRow;

        private Insurance _selectedInsurance;

        private InsuranceDataValues _insuranceDataValue;

        private BindableCollection<InsuranceDataValues> _insuranceValues;

        private BindableCollection<Insurance> _insurances;

        public BindableCollection<InsuranceDataValues> InsuranceValues
        {
            get { return _insuranceValues; }
            set { _insuranceValues = value; NotifyOfPropertyChange(() => _insuranceValues); }
        }

        public InsuranceDataValues InsuranceDataValue
        {
            get { return _insuranceDataValue; }
            set { _insuranceDataValue = value; NotifyOfPropertyChange(() => _insuranceDataValue); }
        }

        public Insurance SelectedInsurance
        {
            get { return _selectedInsurance; }
            set { _selectedInsurance = value; NotifyOfPropertyChange(() => _selectedInsurance); }
        }

        public BindableCollection<Insurance> Insurances
        {
            get { return _insurances; }
            set { _insurances = value; NotifyOfPropertyChange(() => _insurances); }
        }

        public InsuranceDataValues SelectedRow
        {
            get { return _selectedRow; }
            set { _selectedRow = value; NotifyOfPropertyChange(() => _selectedRow); }
        }

        #endregion

        #region Constructor 

        public DefaultValuesViewModel()
        {
            businessManager = new BusinessManager();
            InsuranceDataValue = new InsuranceDataValues();
            PopulateLists();
            SetupPermissions();
            _insuranceDataValue = new InsuranceDataValues();
        }

        #endregion  

        #region Buttons

        /// <summary>
        /// Try save the edit or new made insurancebasevalues data
        /// </summary>
        public void SaveButton()
        {
            if (SelectedInsurance != null)
                _insuranceDataValue.Insurance = SelectedInsurance;

            if (_insuranceDataValue.InsuranceAcqVariables != null && _insuranceDataValue.Year.HasValue)
                _insuranceDataValue.InsuranceAcqVariables.Startdate = _insuranceDataValue.Year.Value;

            if (_insuranceDataValue.InsuranceBaseValues != null && _insuranceDataValue.Year.HasValue)
                _insuranceDataValue.InsuranceBaseValues.StartDate = _insuranceDataValue.Year.Value;

            Console.WriteLine(_insuranceDataValue.InsuranceAcqVariables.Startdate);
            Console.WriteLine(_insuranceDataValue.InsuranceBaseValues.StartDate);

            var errors = businessManager.TryRegisterBaseValues(_insuranceDataValue.Insurance, _insuranceDataValue.InsuranceBaseValues, _insuranceDataValue.InsuranceAcqVariables);
            if (errors.Count == 0)
            {
                MessageBox.Show("Grundvärde uppdaterad/registrerad!");
                _insuranceDataValue = new InsuranceDataValues();
                PopulateLists();
                this.Refresh();
            }
            else
            {
                MessageBox.Show(String.Join("\n", errors.ToArray()));
            }
        }

        /// <summary>
        /// When the edit button is pressed in the UI
        /// </summary>
        public void EditButton()
        {
            if (SelectedRow == null) return;
            ChangeSelectedInsurance(Insurances.Where(i => i.InsuranceId == SelectedRow.Insurance.InsuranceId).First());
            InsuranceDataValue = SelectedRow;
            this.Refresh();
        }

        /// <summary>
        /// When the cancel button is pressed in the UI
        /// </summary>
        public void CancelButton()
        {
            InsuranceDataValue = new InsuranceDataValues();
            PopulateLists();
            this.Refresh();
        }

        #endregion

        #region Events 

        /// <summary>
        /// Event when the insurance is selected in list
        /// </summary>
        /// <param name="insurance"></param>
        public void ChangeSelectedInsurance(Insurance insurance)
        {
            SelectedInsurance = insurance;
        }

        /// <summary>
        /// Event for doubleclicking column row to do event
        /// </summary>
        public void RowSelect_DoubleClick()
        {
            if (SelectedRow == null) return;
            ChangeSelectedInsurance(Insurances.Where(i => i.InsuranceId == SelectedRow.Insurance.InsuranceId).First());
            InsuranceDataValue = SelectedRow;
            this.Refresh();
        }

        #endregion  

        #region local methods 
        /// <summary>
        /// 
        /// </summary>
        public void PopulateLists()
        {
            try
            {
                _insurances = new BindableCollection<Insurance>(DataManager.GetInsurances());
                _insuranceValues = new BindableCollection<InsuranceDataValues>();
                foreach (var insurance in DataManager.GetInsuranceBaseValues())
                {
                    foreach (var baseValue in insurance.InsuranceBaseValues)
                    {
                        _insuranceValues.Add(new InsuranceDataValues
                        {
                            Insurance = insurance,
                            InsuranceBaseValues = baseValue,
                            Year = baseValue.StartDate

                        });
                    }
                    foreach (var acqVar in insurance.InsuranceAcqVariables)
                    {
                        var q = _insuranceValues.Where(x => x.Insurance.InsuranceId == acqVar.InsuranceId && x.Year == acqVar.Startdate);
                        foreach( var row in q )
                        {
                            row.InsuranceAcqVariables = acqVar;
                        }
                        if (q.Count() == 0)
                        {
                            _insuranceValues.Add(new InsuranceDataValues
                            {
                                Insurance = insurance,
                                InsuranceAcqVariables = acqVar,
                                Year = acqVar.Startdate
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SetupPermissions()
        {
            if ((StaticSession.User.IsUserAuthorized("VD") || StaticSession.User.IsUserAuthorized("Admin")
                || StaticSession.User.IsUserAuthorized("Försäljningsassistent") || StaticSession.User.IsUserAuthorized("Försäljningschef")))
            {
                EditBtnEnabled = true;
                SaveBtnEnabled = true;
                CancelBtnEnabled = true;
            }
        }

        #endregion

        #region Costum value holder

        /// <summary>
        /// To be able to list the data in XAML datagrid
        /// </summary>
        public class InsuranceDataValues : Screen
        {
            public Insurance Insurance { get; set; }
            public InsuranceBaseValues InsuranceBaseValues { get; set; }
            public InsuranceAcqVariables InsuranceAcqVariables { get; set; }

            public InsuranceDataValues()
            {
                InsuranceBaseValues = new InsuranceBaseValues();
                InsuranceAcqVariables = new InsuranceAcqVariables();
            }

            /// <summary>
            /// Enable & disable "insurance" on edits
            /// </summary>
            public bool Enabled
            {
                get
                {
                    if (InsuranceBaseValues != null)
                        if (InsuranceBaseValues.Id > 0)
                            return false;

                    if (InsuranceAcqVariables != null)
                        if (InsuranceAcqVariables.Id > 0)
                            return false;

                    return true;
                }
            }

            private int? _year;

            public int? Year
            {
                get { return _year; }
                set { _year = value; NotifyOfPropertyChange(() => _year); }
            }
        }

        #endregion 
    }
}

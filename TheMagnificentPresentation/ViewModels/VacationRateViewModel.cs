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
    public class VacationRateViewModel : Screen
    {

        #region Fields

        private BusinessManager businessManager;

        public bool EditBtnEnabled { get; private set; }
        public bool SaveBtnEnabled { get; private set; }
        public bool CancelBtnEnabled { get; private set; }

        #endregion

        #region Properties

        private VacationRate _selectedVacationRate;

        private VacationRate _vactionRate;

        private BindableCollection<VacationRate> _vacationRates;

        public BindableCollection<VacationRate> VacationRates {
            get { return _vacationRates; }
            set { _vacationRates = value; NotifyOfPropertyChange(() => _vacationRates); }
        }

        public VacationRate VacationRate
        {
            get { return _vactionRate; }
            set { _vactionRate = value; NotifyOfPropertyChange(() => _vactionRate); }
        }

        public VacationRate SelectedVacationRate
        {
            get { return _selectedVacationRate; }
            set { _selectedVacationRate = value; NotifyOfPropertyChange(() => _selectedVacationRate); }
        }

        #endregion 

        #region Constructor

        public VacationRateViewModel()
        {
            _vactionRate = new VacationRate();
            _vacationRates =  new BindableCollection<VacationRate>();
            businessManager = new BusinessManager();
            PopulateLists();
            SetupPermissions();
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Try save the edit or new made vacationrate data
        /// </summary>
        public void SaveButton()
        {
            var errors = businessManager.TryRegisterVacationRate(_vactionRate);
            if (errors.Count == 0)
            {
                MessageBox.Show("Semesterersättning uppdaterad/registrerad!");
                _vactionRate = new VacationRate();
                PopulateLists();
                this.Refresh();
            }
            else
            {
                MessageBox.Show(String.Join("\n", errors.ToArray()));
            }
        }

        /// <summary>
        /// Copies values to fields to edit
        /// </summary>
        public void EditButton()
        {
            VacationRate = SelectedVacationRate;
            this.Refresh();
        }

        /// <summary>
        /// When the cancel button is pressed in the UI
        /// </summary>
        public void CancelButton()
        {
            VacationRate = new VacationRate();
            PopulateLists();
            this.Refresh();
        }

        #endregion

        #region Events

        /// <summary>
        /// Event for doubleclicking column row to do event
        /// </summary>
        public void RowSelect_DoubleClick()
        {
            if (SelectedVacationRate == null) return;
            ChangeSelectedVacationRate(SelectedVacationRate);
            _vactionRate = SelectedVacationRate;
            this.Refresh();
        }

        #endregion

        #region local methods 
        /// <summary>
        /// Byter vald semesterersättning
        /// </summary>
        /// <param name="rate"></param>
        private void ChangeSelectedVacationRate(VacationRate rate)
        {
            _selectedVacationRate = rate;
        }
        /// <summary>
        /// populerarlistor
        /// </summary>
        private void PopulateLists()
        {
            _vacationRates = new BindableCollection<VacationRate>();
            var vacrates = DataManager.GetVacationRates();
            _vacationRates.AddRange(vacrates);
        }
        /// <summary>
        /// Sätter upp permissions
        /// </summary>
        private void SetupPermissions()
        {
            if ((StaticSession.User.IsUserAuthorized("VD") || StaticSession.User.IsUserAuthorized("Admin")
                || StaticSession.User.IsUserAuthorized("Ekonomiassistent")))
            {
                EditBtnEnabled = true;
                SaveBtnEnabled = true;
                CancelBtnEnabled = true;
            }
        }

        #endregion  

    }
}

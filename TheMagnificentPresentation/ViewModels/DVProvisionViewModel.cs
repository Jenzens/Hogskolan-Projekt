using Caliburn.Micro;
using System;
using System.Windows;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    public class DVProvisionViewModel : Screen
    {
        #region Fields

        private BusinessManager businessManager;

        #endregion

        #region Props

        public bool EditBtnEnabled { get; private set; }
        public bool SaveBtnEnabled { get; private set; }
        public bool CancelBtnEnabled { get; private set; }

        private CommisionValues _selectedRow;

        private CommisionValues _commissionValue;

        private BindableCollection<CommisionValues> _commissionValues;

        public BindableCollection<CommisionValues> CommissionValues
        {
            get { return _commissionValues; }
            set { _commissionValues = value; NotifyOfPropertyChange(() => _commissionValues); }
        }

        public CommisionValues CommissionValue
        {
            get { return _commissionValue; }
            set { _commissionValue = value; NotifyOfPropertyChange(() => _commissionValue); }
        }

        public CommisionValues SelectedRow
        {
            get { return _selectedRow; }
            set { _selectedRow = value; NotifyOfPropertyChange(() => _selectedRow); }
        }

        #endregion
        
        #region Constructor

        public DVProvisionViewModel()
        {
            businessManager = new BusinessManager();
            CommissionValue = new CommisionValues();
            SetupPermissions();
            PopulateLists();
        }

        #endregion

        #region Buttons

        /// <summary>
        /// When save button is pressed in UI
        /// </summary>
        public void SaveButton()
        {
            var errors = businessManager.TryRegisterCommissionValues(CommissionValue);
            if (errors.Count == 0)
            {
                MessageBox.Show("Provisionsvärden uppdaterad/registrerad!");
                _commissionValue = new CommisionValues();
                PopulateLists();
                this.Refresh();
            }
            else
            {
                MessageBox.Show(String.Join("\n", errors.ToArray()));
            }
        }

        /// <summary>
        /// When edit button is pressed in UI
        /// </summary>
        public void EditButton()
        {
            if (SelectedRow == null) return;
            CommissionValue = SelectedRow;
            this.Refresh();
        }

        /// <summary>
        /// When cancel button is pressed in UI
        /// </summary>
        public void CancelButton()
        {
            CommissionValue = new CommisionValues();
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
            if (SelectedRow == null) return;
            CommissionValue = SelectedRow;
            this.Refresh();
        }

        #endregion  

        #region local methods

        /// <summary>
        /// Setup permissions for the current UI
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

        /// <summary>
        /// Populate lists for the UI
        /// </summary>
        private void PopulateLists()
        {
            try
            {
                _commissionValues = new BindableCollection<CommisionValues>(DataManager.GetCommissionValues());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

    }
}

using Caliburn.Micro;
using System;
using System.Windows;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    class UserViewModel : Conductor<object>
    {

        #region Properties

        private UserTabCViewModel _tab;
        private Users _selectUser;

        private BindableCollection<Users> _users;

        public BindableCollection<Users> Users
        {
            get { return _users; }
            set { _users = value; NotifyOfPropertyChange(() => _users); }
        }

        public Users SelectedUser
        {
            get { return _selectUser; }
            set { _selectUser = value; NotifyOfPropertyChange(() => _selectUser); }
        }

        #endregion

        #region Constructor

        public UserViewModel(UserTabCViewModel tab)
        {
            _tab = tab;
            _users = new BindableCollection<Users>();
            PopulateList();  
        }

        #endregion

        #region Buttons and selectors

        /// <summary>
        /// Opens create new user window
        /// </summary>
        public void CreateNewUserButton()
        {
            try
            {
                UserFormViewModel tab = (UserFormViewModel)_tab.Create_Tab;
                _tab.ShowCreateTab();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Opens edit window with selected users information
        /// </summary>
        public void EditUserButton()
        {
            if (_selectUser is Users && _selectUser.AgentNumber > 0)
            {
                try
                {
                    UserFormViewModel tab = (UserFormViewModel)_tab.Edit_Tab;
                    tab.User = SelectedUser;
                    _tab.ShowEditTab();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                MessageBox.Show("Välj en användare att editera");
            }
        }

        /// <summary>
        /// Event for doubleclicking column row to do event
        /// </summary>
        public void RowSelect_DoubleClick()
        {
            if (SelectedUser == null) return;
            try
            {
                UserFormViewModel tab = (UserFormViewModel) _tab.Edit_Tab;
                tab.User = SelectedUser;
                _tab.ShowEditTab();
                Console.WriteLine(SelectedUser.AgentNumber);
            }
            catch ( Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region local methods

        /// <summary>
        /// Gets a list of users from the database and adds it to the viewmodel
        /// </summary>
        public void PopulateList()
        {
            var userlist = DataManager.GetUsers();
            _users.AddRange(userlist);
        }

        #endregion

    }
}

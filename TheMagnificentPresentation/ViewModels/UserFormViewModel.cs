using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    class UserFormViewModel : Screen
    {

        #region Fields

        private BusinessManager businessManager;
        
        #endregion
        
        #region Properties

        private BindableCollection<Roles> _roles;

        private string _newPassword;

        private Users _user;

        private string _confirmPassword;

        private int _selectedIndex = 0; // 0 index default

        private int _searchAgentNumber;

        private bool _newuser;

        private BindableCollection<Users> _users;

        public BindableCollection<Users> Users
        {
            get { return _users; }
            set { _users = value; NotifyOfPropertyChange(() => _users); }
        }

        public int SearchAgentNumber
        {
            get { return _searchAgentNumber; }
            set { _searchAgentNumber = value; NotifyOfPropertyChange(() => _searchAgentNumber); }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; NotifyOfPropertyChange(() => _selectedIndex); }
        }

        public BindableCollection<Roles> Roles
        {
            get { return _roles; }
            set { _roles = value; NotifyOfPropertyChange(() => _roles); }
        }

        public Users User
        {
            get { return _user; }
            set
            {
                _user = value;
                UserChanged();
                NotifyOfPropertyChange(() => _user);
            }
        }

        public string NewPassword
        {
            get { return _newPassword; }
            set { _newPassword = value; NotifyOfPropertyChange(() => _newPassword); }
        }

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value; NotifyOfPropertyChange(() => _confirmPassword); }
        }

        #endregion

        #region Constructor

        public UserFormViewModel(bool newuser)
        {
            _newuser = newuser;
            businessManager = new BusinessManager();
            _users = new BindableCollection<Users>();
            _roles = new BindableCollection<Roles>();
            _roles.AddRange(DataManager.GetRoles());

            _user = new Users();
            _user.ZipCity = new ZipCity();
        }

        #endregion

        #region Buttons and selectors
        /// <summary>
        /// Creates a new user from the information provided. 
        /// If the form is not in edit mode it updates the existing user.
        /// </summary>
        public void CreateNewUserButton()
        {
            _user.Roles = new Collection<Roles> // Get the selected role.
            {
                Roles[_selectedIndex]
            };
            if (_newuser == true)
            {
                if (_confirmPassword.Equals(_newPassword))
                {
                    _user.Roles.Add(Roles[_selectedIndex]);
                    _user.Password = _newPassword;
                }
                else
                {
                    MessageBox.Show("Lösenorden stämmer inte överens.");
                }
                var errors = businessManager.CreateNewUser(User);
                if (errors.Count > 0)
                {
                    MessageBox.Show(String.Join("\n", errors.ToArray()));
                }
                else
                {
                    MessageBox.Show("Användare skapad.");
                }
            }
            else if (_newuser == false)
            {
                var errors = businessManager.UpdateUser(_user);
                if(errors.Count > 0)
                {
                    MessageBox.Show(String.Join("\n", errors.ToArray()));
                }
                else
                {
                    MessageBox.Show("Användarens uppgifter har uppdaterats.");

                }

                if (_newPassword != null && _confirmPassword != null)
                {
                    if (_newPassword.Length > 0 && _confirmPassword.Length > 0)
                    {
                        MessageBox.Show(_user.ChangePassword(_newPassword, _confirmPassword));
                    }
                }
            }
        }

        /// <summary>
        /// Cleares the fields
        /// </summary>
        public void ClearFieldsButton()
        {
            _user = new Users();
            _user.ZipCity = new ZipCity();
            Refresh();
        }

        public void UserChanged()
        {
            if (_user.Roles.Count() > 0)
            {
                var role = _user.Roles.First();
                for (int i = 0; i < Roles.Count(); i++)
                {
                    if (role.PermissionId == Roles[i].PermissionId)
                    {
                        _selectedIndex = i;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Håller reda på vilken roll som är vald
        /// </summary>
        /// <param name="index"></param>
        public void ChangeIndex(int index)
        {
            _selectedIndex = index;
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    class ProfileViewModel : Screen
    {

        #region Fields

        private BusinessManager businessManager = new BusinessManager();

        #endregion

        #region Properties

        private Users _user;

        private string _passwordMessageLabel;

        private string _oldPassword;

        private string _newPassword;

        private string _confirmPassword;

        public Users User
        {
            get { return _user; }
            set { _user = value;NotifyOfPropertyChange(() => _user); }
        }

        public string PasswordMessageLabel
        {
            get { return _passwordMessageLabel; }
            set { _passwordMessageLabel = value; NotifyOfPropertyChange(() => _passwordMessageLabel); }
        }

        public string OldPassword
        {
            get { return _oldPassword; }
            set { _oldPassword = value; NotifyOfPropertyChange(() => _oldPassword); }
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

        public ProfileViewModel()
        {
            User = StaticSession.User;
        }

        #region Buttons

        /// <summary>
        /// Change the profiles data (most fields are locked)
        /// </summary>
        public void SaveUserChanges()
        {
            List<string> errors = businessManager.UpdateUser(_user);
            if (errors.Count() > 0)
            {
                MessageBox.Show(String.Join("\n", errors.ToArray()));
            }
            else
            {
                MessageBox.Show("Ändringar sparade!");
                this.Refresh();
            }
        }

        /// <summary>
        /// Change the password of the current profile
        /// </summary>
        public void SavePasswordButton()
        {         
            MessageBox.Show(_passwordMessageLabel = businessManager.ChangeUserPassword(this._user, _oldPassword, _newPassword, _confirmPassword));
        }

        #endregion

    }
}

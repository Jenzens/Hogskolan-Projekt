using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    public class LoginViewModel : Screen
    {
        #region Fields

        IWindowManager windowManger = new WindowManager();
        BusinessManager businessManager = new BusinessManager();

        #endregion

        #region Properties

        private string _agentNumber = "";

        public string AgentNumber
        {
            get { return _agentNumber; }
            set { _agentNumber = value; NotifyOfPropertyChange(() => _agentNumber); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; NotifyOfPropertyChange(() => _password); }
        }

        #endregion

        #region Buttons
        /// <summary>
        /// Loggar in användaren
        /// </summary>
        public void LogInButton()
        {
            
            if(String.IsNullOrEmpty(_password) || String.IsNullOrEmpty(_agentNumber))
            {
                MessageBox.Show("Fält får inte lämnas tomma");
                return;
            }

            int x;
            if(int.TryParse(_agentNumber, out x))
            {
                Users user = businessManager.Login(x, _password);
                if (user.AgentNumber > 0)
                {
                    if ((user.IsUserAuthorized("INAKTIV") || user.Roles.Count == 0))
                    {
                        MessageBox.Show("Detta kontot är inaktiverat eller saknar en roll, kontakta administratör.");
                        return;
                    }
                    windowManger.ShowWindow(new MainViewModel(user), null, null);
                    TryClose();
                    return;
                }
            }
            MessageBox.Show("Användaruppgifter vart felaktiga, försök igen");
        }

        public void CloseWindow()
        {
            TryClose();
        }

        #endregion

        #region Events 

        /// <summary>
        /// When pressing enter in the username/password box
        /// </summary>
        /// <param name="sender">What sent the event</param>
        /// <param name="e">Incoming key event</param>
        public void OnKeyDown(KeyEventArgs keyArgs)
        {
            if (keyArgs.Key == Key.Enter)
            {
                LogInButton();
            }
        }

        #endregion
    }
}

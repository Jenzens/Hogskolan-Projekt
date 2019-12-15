using Caliburn.Micro;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    public class MainViewModel : Conductor<object>
    {

        #region Fields

        public int ButtonCloseMenuIsVisible { get; set; }
        private readonly string DOCUMENTS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private readonly string FILENAME = "\\prospects.pdf";

        //If buttons are enabled or not
        public bool ProfileEnabled          { get; private set; }

        public bool InsurancesEnabled       { get; private set; }  

        public bool InsuranceDataEnabled    { get; private set; } 

        public bool BaseValuesEnabled       { get; private set; }
        
        public bool StatisticsEnabled       { get; private set; } 

        public bool UsersEnabled            { get; private set; }

        public bool CommissionEnabled       { get; private set; }  

        public bool PrintEnabled            { get; private set; }

        #endregion

        #region Constructor

        public MainViewModel(Users user)
        {
            StaticSession.User = new Users();
            StaticSession.User = user; // Keep track of the login user and set it to static so we can reach it from anywhere in the front-end
            ActivateItem(new PresentationViewModel()); // Activate "first screen"
            SetupPermissions();
        }

        #endregion

        #region Buttons

        public void CloseWindow()
        {
            TryClose();
        }

        public void LogOut()
        {
            new WindowManager().ShowWindow(new LoginViewModel());
            TryClose();
        }

        public void Profile()
        { 
            ActivateItem(new ProfTabCViewModel());
        }

        public void InsuranceH()
        {
            ActivateItem(new IHTabCViewModel()); 
        }

        public void Insurance()
        {
            ActivateItem(new InTabCViewModel());
        }

        public void DeValues()
        {
            ActivateItem(new DVTabCViewModel());
        }

        public void StatsView()
        {
            ActivateItem(new StatsViewModel());
        }

        public void UserView()
        {
            ActivateItem(new UserTabCViewModel());
        }

        public void ProView()
        {
            ActivateItem(new ProvisionViewModel());
        }

        /// <summary>
        /// Prints all the prospect to default printer
        /// </summary>
        public void PrintButton()
        {
            DocumentManager.ExportProspectsToPdf();
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    Verb = "print",
                    FileName = DOCUMENTS + FILENAME

                };
                process.Start();
                process.Close();
            }               
        }

        #endregion

        #region Methods 

        private void SetupPermissions()
        {
            if (StaticSession.User.IsUserAuthorized("VD") || StaticSession.User.IsUserAuthorized("Admin"))
            {
                ProfileEnabled          = true;
                InsurancesEnabled       = true;
                InsuranceDataEnabled    = true;
                BaseValuesEnabled       = true;
                StatisticsEnabled       = true;
                UsersEnabled            = true;           
                CommissionEnabled       = true;
                PrintEnabled            = true;
            }
            else if (StaticSession.User.IsUserAuthorized("Ekonomiassistent"))
            {
                ProfileEnabled          = true;
                InsurancesEnabled       = true;
                InsuranceDataEnabled    = true;
                BaseValuesEnabled       = true;    
                StatisticsEnabled       = true;    
                UsersEnabled            = true;     
                CommissionEnabled       = true;    
            }
            else if (StaticSession.User.IsUserAuthorized("Försäljningschef"))
            {
                ProfileEnabled          = true;
                InsurancesEnabled       = true;
                InsuranceDataEnabled    = true;
                BaseValuesEnabled       = true;
                StatisticsEnabled       = true;       
                PrintEnabled            = true;
            }
            else if (StaticSession.User.IsUserAuthorized("Försäljningsassistent"))
            {
                ProfileEnabled          = true;
                InsurancesEnabled       = true;    
                InsuranceDataEnabled    = true;
                BaseValuesEnabled       = true;    
                StatisticsEnabled       = true;    
            }
            else if (StaticSession.User.IsUserAuthorized("Säljare"))
            {
                ProfileEnabled          = true;
                InsurancesEnabled       = true; 
                InsuranceDataEnabled    = true;
                BaseValuesEnabled       = true;    
            }
        }

        #endregion 

    }
}

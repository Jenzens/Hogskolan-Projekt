using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMagnificentModels;
using TheMagnificentModels.Utilities;
using TheMagnificentPresentation.Utilities;

namespace TheMagnificentPresentation.ViewModels
{
    public class InsuranceViewModel : Screen
    {
        #region Fields

        public bool AddBtnEnabled { get; set; }
        public bool EditBtnEnabled { get; set; }

        #endregion

        #region Properties
        private BindableCollection<Insurance> _insurances;

        public BindableCollection<Insurance> Insurances
        {
            get { return _insurances; }
            set { _insurances = value; NotifyOfPropertyChange(() => _insurances); }
        }

        #endregion

        #region Constructor
        public InsuranceViewModel()
        {
            PopulateLists();
            SetupPermissions();
        }
        #endregion

        #region Buttons and selectors

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void SetupPermissions()
        {
            if ((StaticSession.User.IsUserAuthorized("VD") || StaticSession.User.IsUserAuthorized("Admin")
                || StaticSession.User.IsUserAuthorized("Försäljningsassistent") || StaticSession.User.IsUserAuthorized("Försäljningschef")))
            {
                AddBtnEnabled = true;
                EditBtnEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PopulateLists()
        {
            _insurances = new BindableCollection<Insurance>();
            var getInsurances = DataManager.GetInsuranceWithData();
            foreach (var insurance in getInsurances)
            {
                _insurances.Add(insurance);
            }
            foreach (var item in _insurances)
            {
                Console.WriteLine(item.InsuranceBaseValues.Count);
            }
        }
        #endregion
    }
}

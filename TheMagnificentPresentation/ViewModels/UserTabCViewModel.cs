using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagnificentPresentation.ViewModels
{
    class UserTabCViewModel : Conductor<IScreen>.Collection.OneActive
    {

        public Screen Edit_Tab { get; private set; }
        public Screen Create_Tab { get; private set; }

        private IScreen _selectedTab;
        public IScreen SelectedTab {
            get
            {
                return _selectedTab;
            }
            set
            {
                _selectedTab = value;
                NotifyOfPropertyChange();
            }
        }

        protected override void OnInitialize()
        {
            ActivateItem(SelectedTab = new UserViewModel(this)
            {
                DisplayName = "Användare"
            });

            ActivateItem(Create_Tab =new UserFormViewModel(true)
            {
                DisplayName = "Skapa"
            });

            ActivateItem(Edit_Tab = new UserFormViewModel(false)
            {
                DisplayName = "Redigera"
            });
        }

        public void ShowEditTab()
        {
            SelectedTab = Edit_Tab;
            Edit_Tab.Refresh();
        }

        public void ShowCreateTab()
        {
            SelectedTab = Create_Tab;
            Create_Tab.Refresh();
        }

    }
}

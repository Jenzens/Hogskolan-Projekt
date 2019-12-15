using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagnificentPresentation.ViewModels
{
    public class DVTabCViewModel : Conductor<IScreen>.Collection.OneActive
    {


        protected override void OnInitialize()
        {
            ActivateItem(new DefaultValuesViewModel
            {
                DisplayName = "Grundvärden"
            });

            ActivateItem(new DVProvisionViewModel
            {
                DisplayName = "Provision"
            });

            ActivateItem(new VacationRateViewModel
            {
                DisplayName = "Semesterersättning"
            });
        }
    }
}

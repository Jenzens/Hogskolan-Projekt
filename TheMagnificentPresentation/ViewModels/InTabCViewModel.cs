using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagnificentPresentation.ViewModels
{
    class InTabCViewModel : Conductor<IScreen>.Collection.OneActive
    {
        protected override void OnInitialize()
        {
            ActivateItem(new InsuranceViewModel
            {
                DisplayName = "Försäkringar"
            });

            ActivateItem(new DefaultValuesViewModel
            {
                DisplayName = "Redigera"
            });

        }
    }
}


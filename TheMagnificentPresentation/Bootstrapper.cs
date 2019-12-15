using Caliburn.Micro;
using System.Windows;
using TheMagnificentPresentation.ViewModels;

namespace TheMagnificentPresentation
{
    class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        // override OnStartup för att visa ShellViewModel
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<LoginViewModel>();
        }
    }
}

/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Emulator"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Emulator.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<MemoryVisualViewModel>();
        }

        /// <summary>
        /// The MainViewModel Instance
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /// <summary>
        /// The SettingsViewModel Instance
        /// </summary>
        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        /// <summary>
        /// The SaveFileViewModel Instance
        /// </summary>
        public MemoryVisualViewModel MemoryVisual
        {
            get { return ServiceLocator.Current.GetInstance<MemoryVisualViewModel>(); }
        }

        /// <summary>
        /// The Cleanup Method
        /// </summary>
        public static void Cleanup()
        {
            /// <todo>
            /// Clear the ViewModels
            /// </todo>
        }
    }
}
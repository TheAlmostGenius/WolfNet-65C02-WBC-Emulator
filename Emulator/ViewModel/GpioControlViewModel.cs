using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace Emulator.ViewModel
{
    /// <summary>
    /// The ViewModel Used by the GpioControl
    /// </summary>
    public class GpioControlViewModel : ViewModelBase
    {
        #region Properties
        #endregion

        #region Public Methods
        /// <summary>
        /// Instantiates a new instance of the GpioControlViewModel. This is used by the IOC to create the default instance.
        /// </summary>
        [PreferredConstructor]
        public GpioControlViewModel()
        {

        }
        #endregion

        #region Private Methods
        private static void Close()
        {
            Messenger.Default.Send(new NotificationMessage("CloseGpioControlWindow"));
        }
        #endregion
    }
}

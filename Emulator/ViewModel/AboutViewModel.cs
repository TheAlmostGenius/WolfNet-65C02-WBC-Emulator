using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace Emulator.ViewModel
{
	/// <summary>
	/// The ViewModel Used by the SaveFileView
	/// </summary>
	public class AboutViewModel : ViewModelBase
	{
        #region Properties
		public string Name
        {
            get
            {
                return Versioning.Product.Name;
            }
        }

        public string Description
        {
            get
            {
                return Versioning.Product.Description;
            }
        }

        public string Version
        {
            get
            {
                return Versioning.Product.Version;
            }
        }

        public RelayCommand CloseCommand { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Instantiates a new instance of the SettingsViewModel. This is used by the IOC to create the default instance.
        /// </summary>
        [PreferredConstructor]
		public AboutViewModel()
		{
            CloseCommand = new RelayCommand(Close);
		}
        #endregion

        #region Private Methods
		private static void Close()
		{
			Messenger.Default.Send(new NotificationMessage("CloseAboutWindow"));
		}
		#endregion
	}
}

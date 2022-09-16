using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Emulator.Model;

namespace Emulator.ViewModel
{
	/// <summary>
	/// The ViewModel Used by the SaveFileView
	/// </summary>
	public class SettingsViewModel : ViewModelBase
	{
        #region Properties
        /// <summary>
        /// The Relay Command called when saving a file
        /// </summary>
        public RelayCommand ApplyCommand { get; set; }
		
		/// <summary>
		/// The Relay Command called when closing a file
		/// </summary>
		public RelayCommand CloseCommand { get; set; }

		/// <summary>
		/// Tells the UI that that a file has been selected and can be saved.
		/// </summary>
		public bool ApplyEnabled { get { return !string.IsNullOrEmpty(Emulator.FileLocations.SettingsFile); } }

        /// <summary>
        /// Creates a new instance of PortList, the list of all COM ports available to the computer
        /// </summary>
        /// 
        public ObservableCollection<string> PortList { get { return _PortList; } }
        private readonly ObservableCollection<string> _PortList = new ObservableCollection<string>();

        public static string ComPortSelection { get; set; }
        public static SettingsModel SettingsModel { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Instantiates a new instance of the SettingsViewModel. This is used by the IOC to create the default instance.
        /// </summary>
        [PreferredConstructor]
		public SettingsViewModel()
		{

		}

		/// <summary>
		/// Instantiates a new instance of the SettingsViewModel
		/// </summary>
		/// <param name="settingsModel">The SettingsFileModel to be serialized to a file</param>
		public SettingsViewModel(SettingsModel settingsModel)
		{
			ApplyCommand = new RelayCommand(Apply);
			CloseCommand = new RelayCommand(Close);
            ComPortSelection = settingsModel.ComPortName;

            UpdatePortList();
        }

        /// <summary>
        /// Updates PortList with the COM ports available to the computer
        /// </summary>
        public void UpdatePortList()
        {
            PortList.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                PortList.Add(s);
            }
            RaisePropertyChanged("PortList");
        }
        #endregion

        #region Private Methods
        private void Apply()
		{
            Messenger.Default.Send(new NotificationMessage<SettingsModel>(new SettingsModel
            {
                SettingsVersion = Versioning.SettingsFile,
                ComPortName = ComPortSelection,
            }, "SettingsApplied"));
            Messenger.Default.Send(new NotificationMessage("CloseSettingsWindow"));
        }

		private static void Close()
		{
			Messenger.Default.Send(new NotificationMessage("CloseSettingsWindow"));
		}
		#endregion
	}
}

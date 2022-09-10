using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Simulator.Model;

namespace Simulator.ViewModel
{
	/// <summary>
	/// The ViewModel Used by the SaveFileView
	/// </summary>
	public class SettingsViewModel : ViewModelBase
	{
		private static SettingsModel _settingsModel;

        #region Properties
        /// <summary>
        /// The Relay Command called when saving a file
        /// </summary>
        public RelayCommand SaveFileCommand { get; set; }
		
		/// <summary>
		/// The Relay Command called when closing a file
		/// </summary>
		public RelayCommand CloseCommand { get; set; }

		/// <summary>
		/// Tells the UI that that a file has been selected and can be saved.
		/// </summary>
		public bool SaveEnabled { get { return !string.IsNullOrEmpty(Hardware.Hardware.SettingsFile); } }

        /// <summary>
        /// Creates a new instance of PortList, the list of all COM ports available to the computer
        /// </summary>
        /// 
        public ObservableCollection<string> PortList { get { return _PortList; } }
        private readonly ObservableCollection<string> _PortList = new ObservableCollection<string>();

        public static string ComPortSelection { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Instantiates a new instance of the SaveFileViewModel. This is used by the IOC to create the default instance.
        /// </summary>
        [PreferredConstructor]
		public SettingsViewModel()
		{

		}

		/// <summary>
		/// Instantiates a new instance of the SaveFileViewModel
		/// </summary>
		/// <param name="settingsModel">The StateFIleModel to be serialized to a file</param>
		public SettingsViewModel(SettingsModel settingsModel)
		{
			SaveFileCommand = new RelayCommand(Save);
			CloseCommand = new RelayCommand(Close);
			_settingsModel = settingsModel;

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
        private void Save()
		{
			Stream stream = new FileStream(Hardware.Hardware.SettingsFile, FileMode.Create, FileAccess.Write, FileShare.None);
            XmlSerializer XmlFormatter = new XmlSerializer(typeof(SettingsModel));
            XmlFormatter.Serialize(stream, _settingsModel);
			stream.Flush();
			stream.Close();
			Close();
		}

		private static void Close()
		{
			Messenger.Default.Send(new NotificationMessage("CloseSettingsWindow"));
		}
		#endregion
	}
}

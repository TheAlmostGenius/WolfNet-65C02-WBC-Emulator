using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Simulator.Model;

namespace Simulator.ViewModel
{
	/// <summary>
	/// The ViewModel Used by the OpenFileView
	/// </summary>
	public class OpenFileViewModel : ViewModelBase
	{
        #region Properties
        /// <summary>
        /// The Relay Command used to Load a Program
        /// </summary>
        public RelayCommand LoadProgramCommand { get; set; }

        /// <summary>
        /// The Relay Command used to close the dialog
        /// </summary>
        public RelayCommand CloseCommand { get; set; }

        /// <summary>
        /// The Relay Command used to select a file
        /// </summary>
        public RelayCommand SelectBiosFileCommand { get; set; }

        /// <summary>
        /// The Relay Command used to select a file
        /// </summary>
        public RelayCommand SelectRomFileCommand { get; set; }

        /// <summary>
        /// The Name of the file being opened
        /// </summary>
        public string BiosFilename { get; set; }

        /// <summary>
        /// The Name of the file being opened
        /// </summary>
        public string RomFilename { get; set; }

        /// <summary>
        /// The Initial Program Counter, used only when opening a Binary File. Not used when opening saved state.
        /// </summary>
        public string InitalProgramCounter { get; set; }

		/// <summary>
		/// The inital memory offset. Determines where in memory the program begins loading to.
		/// </summary>
		public string MemoryOffset { get; set; }

        /// <summary>
        /// Tells the UI if the file has been selected succesfully
        /// </summary>
        public bool LoadEnabled { get { return (BiosLoadEnabled && RomLoadEnabled) || (!IsNotStateFile); } }

        /// <summary>
        /// Tells the UI if the file has been selected succesfully
        /// </summary>
        public bool BiosLoadEnabled { get { return !string.IsNullOrEmpty(BiosFilename); } }

        /// <summary>
        /// Tells the UI if the file has been selected succesfully
        /// </summary>
        public bool RomLoadEnabled { get { return !string.IsNullOrEmpty(RomFilename); } }

        /// <summary>
        /// Tells the UI if the file type is not a state file. This Property prevents the InitialProgram Counter and Memory Offset from being enabled.
        /// </summary>
        public bool IsNotStateFile
		{
			get
			{
				if (string.IsNullOrEmpty(BiosFilename))
					return true;

				return !BiosFilename.EndsWith(".6502");
			}
        }

        /// <summary>
        /// Creates a new instance of PortList, the list of all COM ports available to the computer
        /// </summary>
        /// 
        public ObservableCollection<string> PortList { get { return _PortList; } }
        private ObservableCollection<string> _PortList = new ObservableCollection<string>();

        public static string ComPortSelection { get; set; }
        #endregion

        #region Public Methods
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

        /// <summary>
        /// Creates a new instance of the OpenFileViewModel
        /// </summary>
        public OpenFileViewModel()
        {
            LoadProgramCommand = new RelayCommand(Load);
			CloseCommand = new RelayCommand(Close);
            SelectBiosFileCommand = new RelayCommand(Select);
            SelectRomFileCommand = new RelayCommand(RomSelect);

            UpdatePortList();
        }
        #endregion

        #region Private Methods
        private void Load()
        {
            var extension1 = Path.GetExtension(BiosFilename);
            var extension2 = Path.GetExtension(RomFilename);
            if (extension1 != null && extension1.ToUpper() == ".BIN" && extension2 != null && extension2.ToUpper() == ".BIN" && !TryLoadBinFile())
				return;

			if (extension1 != null && extension1.ToUpper() == ".6502" && !TryLoad6502File())
				return;

			Close();
		}
        private bool TryLoad6502File()
		{
			var formatter = new BinaryFormatter();
			Stream stream = new FileStream(BiosFilename, FileMode.Open);

			var fileModel = (StateFileModel)formatter.Deserialize(stream);

			stream.Close();

			Messenger.Default.Send(new NotificationMessage<StateFileModel>(fileModel, "FileLoaded"));

			return true;
		}

		private bool TryLoadBinFile()
		{
			byte[] bios;
			try
			{
				bios = File.ReadAllBytes(BiosFilename);
			}
			catch (Exception)
			{
				MessageBox.Show("Unable to Open BIOS Binary");
				return false;
			}

            byte[] rom;
            try
            {
                rom = File.ReadAllBytes(RomFilename);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to Open ROM Binary");
                return false;
            }

            Messenger.Default.Send(new NotificationMessage<AssemblyFileModel>(new AssemblyFileModel
            {
                Bios = bios,
                Rom = rom,
                BiosFilePath = BiosFilename,
                RomFilePath = RomFilename,
                ComPort = ComPortSelection,
            }, "FileLoaded"));

			return true;
		}

        private static void Close()
		{
			Messenger.Default.Send(new NotificationMessage("CloseFileWindow"));
		}

		private void Select()
		{
			var dialog = new OpenFileDialog { DefaultExt = ".bin", Filter = "All Files (*.bin, *.6502)|*.bin;*.6502|Binary Assembly (*.bin)|*.bin|WolfNet W65C02 Emulator Save State (*.6502)|*.6502" };

			var result = dialog.ShowDialog();

			if (result != true)
				return;

			BiosFilename = dialog.FileName;
			RaisePropertyChanged("BiosFilename");
            RaisePropertyChanged("BiosLoadEnabled");
            RaisePropertyChanged("LoadEnabled");
            RaisePropertyChanged("IsNotStateFile");
        }

        private void RomSelect()
        {
            var dialog = new OpenFileDialog { DefaultExt = ".bin", Filter = "All Files (*.bin)|*.bin|Binary Assembly (*.bin)|*.bin" };

            var result = dialog.ShowDialog();

            if (result != true)
                return;

            RomFilename = dialog.FileName;
            RaisePropertyChanged("RomFilename");
            RaisePropertyChanged("RomLoadEnabled");
            RaisePropertyChanged("LoadEnabled");
            RaisePropertyChanged("IsNotStateFile");
        }
        #endregion
    }
}

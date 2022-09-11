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
using ResDict = System.Windows.ResourceDictionary;

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
        /// The path of the file being opened
        /// </summary>
        public string BiosFilePath { get; set; }

        /// <summary>
        /// The path of the file being opened
        /// </summary>
        public string RomFilePath { get; set; }

        /// <summary>
        /// The name of the file being opened
        /// </summary>
        public string BiosFileName { get; set; }

        /// <summary>
        /// The name of the file being opened
        /// </summary>
        public string RomFileName { get; set; }

        /// <summary>
        /// The Initial Program Counter, used only when opening a Binary File. Not used when opening saved state.
        /// </summary>
        public string InitalProgramCounter { get; set; }

		/// <summary>
		/// The inital memory offset. Determines where in memory the program begins loading to.
		/// </summary>
		public string MemoryOffset { get; set; }

        /// <summary>
        /// Tells the UI if the file(s) are OK for loading.
        /// </summary>
        public bool LoadEnabled
        {
            get
            {
                if (BiosLoadable && IsBiosOnly)
                {
                    return true;
                }
                else if (BiosLoadable && RomLoadable && IsNotStateFile)
                {
                    return true;
                }
                else if (!IsNotStateFile)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
        }

        /// <summary>
        /// Tells the UI if the ROM file is to be loaded.
        /// </summary>
        public bool IsRomEnabled
        {
            get
            {
                return !(IsBiosOnly || !IsNotStateFile);
            }
        }

        /// <summary>
        /// Tells the UI if the file has been selected succesfully.
        /// </summary>
        public bool BiosLoadable { get { return !string.IsNullOrEmpty(BiosFilePath); } }

        /// <summary>
        /// Tells the UI if the file has been selected succesfully.
        /// </summary>
        public bool RomLoadable { get { return !string.IsNullOrEmpty(RomFilePath); } }

        /// <summary>
        /// Tells the UI if the file type is not a state file. This Property prevents the Initial Program Counter and Memory Offset from being enabled.
        /// </summary>
        public bool IsNotStateFile
        {
            get
            {
                if (string.IsNullOrEmpty(BiosFilePath))
                    return true;

                return !BiosFilePath.EndsWith(".6502");
            }
        }

        /// <summary>
        /// Tells the UI to only load the BIOS
        /// </summary>
        public static bool IsBiosOnly { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new instance of the OpenFileViewModel
        /// </summary>
        public OpenFileViewModel()
        {
            LoadProgramCommand = new RelayCommand(Load);
			CloseCommand = new RelayCommand(Close);
            SelectBiosFileCommand = new RelayCommand(Select);
            SelectRomFileCommand = new RelayCommand(RomSelect);
        }
        #endregion

        #region Private Methods
        private void Load()
        {
            var extension1 = Path.GetExtension(BiosFilePath);
            var extension2 = Path.GetExtension(RomFilePath);
            if (extension1 != null && extension1.ToUpper() == ".BIN" && extension2 != null && extension2.ToUpper() == ".BIN" && !TryLoadBinFile())
				return;

			if (extension1 != null && extension1 == ".6502" && !TryLoad6502File())
				return;

			Close();
		}
        private bool TryLoad6502File()
		{
			var formatter = new BinaryFormatter();
			Stream stream = new FileStream(BiosFilePath, FileMode.Open);

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
				bios = File.ReadAllBytes(BiosFilePath);
			}
			catch (Exception)
			{
				MessageBox.Show("Unable to Open BIOS Binary");
				return false;
			}

            byte[] rom = null;
            if (!IsBiosOnly)
            {
                try
                {
                    rom = File.ReadAllBytes(RomFilePath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to Open ROM Binary");
                    return false;
                }
            }
            Messenger.Default.Send(new NotificationMessage<AssemblyFileModel>(new AssemblyFileModel
            {
                Bios = bios,
                Rom = rom,
                IsBiosOnly = IsBiosOnly,
                BiosFilePath = BiosFilePath,
                BiosFileName = BiosFileName,
                RomFilePath = RomFilePath,
                RomFileName = RomFileName,
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

			BiosFilePath = dialog.FileName;
            BiosFileName = Path.GetFileName(BiosFilePath);
            RaisePropertyChanged("BiosFilePath");
            RaisePropertyChanged("BiosFileName");
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

            RomFilePath = dialog.FileName;
            RomFileName = Path.GetFileName(RomFilePath);
            RaisePropertyChanged("RomFilePath");
            RaisePropertyChanged("RomFileName");
            RaisePropertyChanged("RomLoadEnabled");
            RaisePropertyChanged("LoadEnabled");
            RaisePropertyChanged("IsNotStateFile");
        }
        #endregion
    }
}

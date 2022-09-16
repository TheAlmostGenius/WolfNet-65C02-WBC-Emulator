using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Emulator.Model;
using Hardware;

namespace Emulator.ViewModel
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
        public RelayCommand LoadRomCommand { get; set; }

        /// <summary>
        /// The Relay Command used to close the dialog
        /// </summary>
        public RelayCommand CloseCommand { get; set; }

        /// <summary>
        /// The Relay Command used to select a file.
        /// </summary>
        public RelayCommand SelectFileCommand { get; set; }

        /// <summary>
        /// The data from the ROM file.
        /// </summary>
        private byte[][] Rom { get; set; }

        /// <summary>
        /// The path of the file being opened.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The name of the file being opened.
        /// </summary>
        public string FileName { get; set; }

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
                if (IsLoadable && IsNotStateFile)
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
        public bool IsEnabled
        {
            get { return IsNotStateFile; }
        }

        /// <summary>
        /// Tells the UI if the file has been selected succesfully.
        /// </summary>
        public bool IsLoadable { get { return !string.IsNullOrEmpty(FilePath); } }

        /// <summary>
        /// Tells the UI if the file type is not a state file. This Property prevents the Initial Program Counter and Memory Offset from being enabled.
        /// </summary>
        public bool IsNotStateFile
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                    return true;

                return !FilePath.EndsWith(".6502");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new instance of the OpenFileViewModel.
        /// </summary>
        public OpenFileViewModel()
        {
            LoadRomCommand = new RelayCommand(Load);
            CloseCommand = new RelayCommand(Close);
            SelectFileCommand = new RelayCommand(Select);
        }
        #endregion

        #region Private Methods
        private void Load()
        {
            var extension1 = Path.GetExtension(FilePath);
            if (extension1 != null && extension1.ToUpper() == ".BIN" && !TryLoadBinFile())
				return;

			if (extension1 != null && extension1 == ".6502" && !TryLoad6502File())
				return;

			Close();
		}

        private bool TryLoad6502File()
		{
			var formatter = new BinaryFormatter();
			Stream stream = new FileStream(FilePath, FileMode.Open);

			var fileModel = (StateFileModel)formatter.Deserialize(stream);

			stream.Close();

			Messenger.Default.Send(new NotificationMessage<StateFileModel>(fileModel, "FileLoaded"));

			return true;
		}

        private bool TryLoadBinFile()
        {
            try
            {
                FileStream _file = new FileStream(FilePath, FileMode.Open);
                if (_file.Length > (MemoryMap.BankedRom.TotalBanks * MemoryMap.BankedRom.BankSize))
                {
                    MessageBox.Show("File length is greater than the total size of the ROM!");
                }
                for (ushort i = 0; i < MemoryMap.BankedRom.TotalBanks; i++)
                {
                    for (ushort j = 0; j < MemoryMap.BankedRom.BankSize; j++)
                    {
                        Rom[i][j] = (byte)_file.ReadByte();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to Open ROM Binary");
                return false;
            }

            Messenger.Default.Send(new NotificationMessage<RomFileModel>(new RomFileModel
            {
                Rom = Rom,
                RomFilePath = FilePath,
                RomFileName = FileName,
            }, "FileLoaded"));

			return true;
		}

        private static void Close()
		{
			Messenger.Default.Send(new NotificationMessage("CloseFileWindow"));
        }

        private void Select()
        {
            var dialog = new OpenFileDialog { DefaultExt = ".bin", Filter = "All Files (*.bin, *.6502)|*.bin;*.6502|Binary Assembly (*.bin)|*.bin|WolfNet 65C02 Emulator Save State (*.6502)|*.6502" };

            var result = dialog.ShowDialog();

            if (result != true)
                return;

            FilePath = dialog.FileName;
            FileName = Path.GetFileName(FilePath);
            RaisePropertyChanged("FilePame");
            RaisePropertyChanged("FileName");
            RaisePropertyChanged("LoadEnabled");
            RaisePropertyChanged("IsNotStateFile");
        }
        #endregion
    }
}

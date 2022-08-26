using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Simulator.Model;
using COM = COMIO.COMIO;

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
		/// The Relay Command used to select a BIOS file
		/// </summary>
		public RelayCommand SelectBiosFileCommand { get; set; }

		/// <summary>
		/// The Relay Command used to select a Banked ROM file
		/// </summary>
		public RelayCommand SelectRomFileCommand { get; set; }

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
        public bool LoadEnabled => BiosLoadEnabled && RomLoadEnabled;

        /// <summary>
        /// Tells the UI if the file has been selected succesfully
        /// </summary>
        public bool BiosLoadEnabled { get { return !string.IsNullOrEmpty(BiosFilename); } }

		/// <summary>
		/// Tells the UI if the file has been selected succesfully
		/// </summary>
		public bool RomLoadEnabled { get { return !string.IsNullOrEmpty(RomFilename); } }

		/// <summary>
		/// The Name of the BIOS file being opened
		/// </summary>
		public string BiosFilename { get; set; }

		/// <summary>
		/// The Name of the Banked ROM file being opened
		/// </summary>
		public string RomFilename { get; set; }

		/// <summary>
		/// The port list used in selecting a COM port for I/O
		/// </summary>
		public List<string> PortList { get; set; }
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates a new instance of the OpenFileViewModel
		/// </summary>
		public OpenFileViewModel()
		{
			LoadProgramCommand = new RelayCommand(Load);
			CloseCommand = new RelayCommand(Close);
			SelectBiosFileCommand = new RelayCommand(BiosSelect);
			SelectRomFileCommand = new RelayCommand(RomSelect);

			var Comio = new COM();
			Comio.UpdatePortList();
			PortList = Comio.GetPortList();
		}
		#endregion

		#region Private Methods
		private void Load()
		{
			if (!TryLoadBinFile())
				return;

			Close();
		}

		private bool TryLoadBinFile()
		{
			int programCounter;
			try
			{
				programCounter = Convert.ToInt32(InitalProgramCounter, 16);
			}
			catch (Exception)
			{
				MessageBox.Show("Unable to Parse ProgramCounter into int");
				return false;
			}

			int memoryOffset;
			try
			{
				memoryOffset = Convert.ToInt32(MemoryOffset, 16);
			}
			catch (Exception)
			{
				MessageBox.Show("Unable to Parse Memory Offset into int");
				return false;
			}

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
				InitialProgramCounter = programCounter,
				MemoryOffset = memoryOffset,
				Bios = bios,
				ProgramRom = rom,
				BiosPath = BiosFilename,
				RomPath = RomFilename
			}, "FileLoaded"));

			return true;
		}

		private static void Close()
		{
			Messenger.Default.Send(new NotificationMessage("CloseFileWindow"));
		}

		private void BiosSelect()
		{
			var dialog = new OpenFileDialog { DefaultExt = ".bin", Filter = "All Files (*.bin)|*.bin|Binary Assembly (*.bin)|*.bin" };

			var result = dialog.ShowDialog();

			if (result != true)
				return;

            BiosFilename = dialog.FileName;
            RaisePropertyChanged("biosFilename");
            RaisePropertyChanged("biosLoadEnabled");
		}

		private void RomSelect()
		{
			var dialog = new OpenFileDialog { DefaultExt = ".bin", Filter = "All Files (*.bin)|*.bin|Binary Assembly (*.bin)|*.bin" };

			var result = dialog.ShowDialog();

			if (result != true)
				return;

			RomFilename = dialog.FileName;
            RaisePropertyChanged("romFilename");
            RaisePropertyChanged("romLoadEnabled");
		}
		#endregion
	}
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hardware;
using Emulator.Model;
using W65C02 = Hardware.W65C02;
using W65C22 = Hardware.W65C22;
using W65C51 = Hardware.W65C51;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Interop;

namespace Emulator.ViewModel
{
	/// <summary>
	/// The Main ViewModel
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		#region Fields
		private int _memoryPageOffset;
		private readonly BackgroundWorker _backgroundWorker;
		private bool _breakpointTriggered;
        #endregion

        #region Properties
        /// <summary>
        /// The 62256 RAM.
        /// </summary>
        private HM62256 HM62256 { get; set; }

        /// <summary>
        /// The 65C02 Processor.
        /// </summary>
        public W65C02 W65C02 { get; private set; }

        /// <summary>
        /// General Purpose I/O, Shift Registers and Timers.
        /// </summary>
        public W65C22 W65C22 { get; private set; }

        /// <summary>
        /// Memory management and 65SIB.
        /// </summary>
        public W65C22 MM65SIB { get; private set; }

        /// <summary>
        /// The ACIA serial interface.
        /// </summary>
        public W65C51 W65C51 { get; private set; }

        /// <summary>
        /// The AT28C010 ROM.
        /// </summary>
        public AT28CXX AT28C64 { get; private set; }

        /// <summary>
        /// The AT28C010 ROM.
        /// </summary>
        public AT28CXX AT28C010 { get; private set; }

        /// <summary>
        /// The Current Memory Page
        /// </summary>
        public MultiThreadedObservableCollection<MemoryRowModel> MemoryPage { get; set; }

		/// <summary>
		/// The output log
		/// </summary>
		public MultiThreadedObservableCollection<OutputLog> OutputLog { get; private set; }

		/// <summary>
		/// The Breakpoints
		/// </summary>
		public MultiThreadedObservableCollection<Breakpoint> Breakpoints { get; set; }

		/// <summary>
		/// The Currently Selected Breakpoint
		/// </summary>
		public Breakpoint SelectedBreakpoint { get; set; }

        /// <summary>
        /// The currently loaded binary file. (If it is indeed loaded, that is.)
        /// </summary>
        public RomFileModel RomFile { get; set; }

        /// <summary>
        /// The Current Disassembly
        /// </summary>
        public string CurrentDisassembly
		{
			get
			{
				if (W65C02.CurrentDisassembly != null)
				{
					return string.Format("{0} {1}", W65C02.CurrentDisassembly.OpCodeString, W65C02.CurrentDisassembly.DisassemblyOutput);
				}
				else
				{
					return string.Empty;
				}
			}
		}

		/// <summary>
		/// The number of cycles.
		/// </summary>
		public int NumberOfCycles { get; private set; }

		/// <summary>
		/// The Memory Page number.
		/// </summary>
		public string MemoryPageOffset
		{
			get { return _memoryPageOffset.ToString("X"); }
			set
			{
				if (string.IsNullOrEmpty(value))
					return;
				try
				{
					_memoryPageOffset = Convert.ToInt32(value, 16);
				}
				catch { }
			}
		}

		/// <summary>
		///  Is the Prorgam Running
		/// </summary>
		public bool IsRunning
		{
			get { return W65C02.isRunning; }
			set
			{
				W65C02.isRunning = value;
				RaisePropertyChanged("IsRunning");
			}
		}

        /// <summary>
        /// Is the banked ROM Loaded.
        /// </summary>
        public bool IsRomLoaded { get; set; }

        /// <summary>
        /// The Slider CPU Speed
        /// </summary>
        public int CpuSpeed { get; set; }

        /// <summary>
        /// The Model used for saving, loading and using data from Settings.xml
        /// </summary>
        public static SettingsModel SettingsModel { get; set; }

        /// <summary>
        /// RelayCommand for Stepping through the progam one instruction at a time.
        /// </summary>
        public RelayCommand StepCommand { get; set; }

        /// <summary>
        /// Relay Command to Reset the Program back to its initial state.
        /// </summary>
        public RelayCommand ResetCommand { get; set; }

		/// <summary>
		/// Relay Command that Run/Pauses Execution
		/// </summary>
		public RelayCommand RunPauseCommand { get; set; }

		/// <summary>
		/// Relay Command that updates the Memory Map when the Page changes
		/// </summary>
		public RelayCommand UpdateMemoryMapCommand { get; set; }

        /// <summary>
        /// The Relay Command that adds a new breakpoint
        /// </summary>
        public RelayCommand AddBreakPointCommand { get; set; }

        /// <summary>
        /// The Relay Command that opens the About window.
        /// </summary>
        public RelayCommand AboutCommand { get; set; }

        /// <summary>
        /// The Relay Command that Removes an existing breakpoint.
        /// </summary>
        public RelayCommand RemoveBreakPointCommand { get; set; }

        /// <summary>
        /// The Command that loads or saves the settings.
        /// </summary>
        public RelayCommand SettingsCommand { get; set; }

        /// <summary>
        /// The Command that loads or saves the settings.
        /// </summary>
        public RelayCommand<IClosable> CloseCommand { get; private set; }

        /// <summary>
        /// The current serial port object name.
        /// </summary>
        public string CurrentSerialPort
		{
			get
			{
				return W65C51.ObjectName;
			}
		}
		#endregion

        #region public Methods
        /// <summary>
        /// Creates a new Instance of the MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            var _formatter = new XmlSerializer(typeof(SettingsModel));
            Stream _stream = new FileStream(FileLocations.SettingsFile, FileMode.OpenOrCreate);
            if (!((_stream == null) || (0 >= _stream.Length)))
            {
                SettingsModel = (SettingsModel)_formatter.Deserialize(_stream);
            }
            else
            {
                MessageBox.Show("Creating new settings file...");
                SettingsModel = new SettingsModel
                {
                    SettingsVersion = Versioning.SettingsFile,
                    ComPortName = "COM10",
                };
                Messenger.Default.Send("SaveSettings");
            }
            _stream.Close();

            HM62256 = new HM62256(MemoryMap.BankedRam.TotalBanks, MemoryMap.BankedRam.Offset, MemoryMap.BankedRam.Length);
            W65C51 = new W65C51(W65C02, MemoryMap.Devices.ACIA.Offset, MemoryMap.Devices.ACIA.Length);
            W65C51.Init(SettingsModel.ComPortName.ToString());
            W65C22 = new W65C22(W65C02, MemoryMap.Devices.GPIO.Offset, MemoryMap.Devices.GPIO.Length);
            W65C22.Init(1000);
            MM65SIB = new W65C22(W65C02, MemoryMap.Devices.MM65SIB.Offset, MemoryMap.Devices.MM65SIB.Length);
            MM65SIB.Init(1000);
            AT28C64 = new AT28CXX(MemoryMap.SharedRom.Offset, MemoryMap.SharedRom.Length, 1);
            AT28C010 = new AT28CXX(MemoryMap.BankedRom.Offset, MemoryMap.BankedRom.Length, MemoryMap.BankedRom.TotalBanks);

            W65C02 = new W65C02();
            MemoryMap.Init(W65C02, W65C22, MM65SIB, W65C51, HM62256, AT28C010, AT28C64);

            // Now we can load the BIOS.
            AT28C64.Load(AT28C64.TryRead(FileLocations.BiosFile));

            AboutCommand = new RelayCommand(About);
            AddBreakPointCommand = new RelayCommand(AddBreakPoint);
            CloseCommand = new RelayCommand<IClosable>(Close);
			RemoveBreakPointCommand = new RelayCommand(RemoveBreakPoint);
            ResetCommand = new RelayCommand(Reset);
			RunPauseCommand = new RelayCommand(RunPause);
            SettingsCommand = new RelayCommand(Settings);
			StepCommand = new RelayCommand(Step);
			UpdateMemoryMapCommand = new RelayCommand(UpdateMemoryPage);

            Messenger.Default.Register<NotificationMessage>(this, GenericNotifcation);
            Messenger.Default.Register<NotificationMessage<RomFileModel>>(this, BinaryLoadedNotification);
            Messenger.Default.Register<NotificationMessage<SettingsModel>>(this, SettingsAppliedNotifcation);
            Messenger.Default.Register<NotificationMessage<StateFileModel>>(this, StateLoadedNotifcation);

            MemoryPage = new MultiThreadedObservableCollection<MemoryRowModel>();
			OutputLog = new MultiThreadedObservableCollection<OutputLog>();
			Breakpoints = new MultiThreadedObservableCollection<Breakpoint>();

			UpdateMemoryPage();

			_backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = false };
			_backgroundWorker.DoWork += BackgroundWorkerDoWork;
            System.Windows.Application.Current.MainWindow.Closing += new CancelEventHandler(OnClose);

            Reset();
        }

        public void OnClose(Object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            if (IsRunning)
            {
                e.Cancel = true;
                return;
            }
			else
			{
				var result = MessageBox.Show("Are you sure you want to quit the emulator?", "To quit, or not to quit.", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.No)
				{
					e.Cancel = true;
					return;
				}
			}
            Stream stream = new FileStream(FileLocations.SettingsFile, FileMode.Create, FileAccess.Write, FileShare.None);
            XmlSerializer XmlFormatter = new XmlSerializer(typeof(SettingsModel));
            XmlFormatter.Serialize(stream, MainViewModel.SettingsModel);
            stream.Flush();
            stream.Close();
            W65C51.Fini();
        }

        #endregion

        #region Private Methods
        private void Close(IClosable window)
        {
            if ((window != null) && (!IsRunning))
            {
                Environment.Exit(ExitCodes.NO_ERROR);
            }
        }

        private void BinaryLoadedNotification(NotificationMessage<RomFileModel> notificationMessage)
        {
            if (notificationMessage.Notification != "FileLoaded")
            {
                return;
            }

			//Initialize the RomFile.Rom memory area.
			RomFile = notificationMessage.Content;

            // Load Banked ROM
            AT28C010.Load(notificationMessage.Content.Rom);

            IsRomLoaded = true;
            RaisePropertyChanged("IsRomLoaded");

            Reset();
        }

        private void StateLoadedNotifcation(NotificationMessage<StateFileModel> notificationMessage)
        {
            if (notificationMessage.Notification != "StateLoaded")
            {
                return;
            }

            Reset();

            OutputLog = new MultiThreadedObservableCollection<OutputLog>(notificationMessage.Content.OutputLog);
            RaisePropertyChanged("OutputLog");

            NumberOfCycles = notificationMessage.Content.NumberOfCycles;

            W65C02 = notificationMessage.Content.W65C02;
            W65C22 = notificationMessage.Content.W65C22;
            MM65SIB = notificationMessage.Content.MM65SIB;
            W65C51 = notificationMessage.Content.W65C51;
            AT28C010 = notificationMessage.Content.AT28C010;
            AT28C64 = notificationMessage.Content.AT28C64;
            UpdateMemoryPage();
            UpdateUi();

            IsRomLoaded = true;
            RaisePropertyChanged("IsRomLoaded");
        }

        private void GenericNotifcation(NotificationMessage notificationMessage)
        {
			if (notificationMessage.Notification == "LoadFile")
			{
                var dialog = new OpenFileDialog { DefaultExt = ".bin", Filter = "All Files (*.bin, *.65C02)|*.bin;*.65C02|Binary Assembly (*.bin)|*.bin|WolfNet 65C02 Emulator Save State (*.65C02)|*.65C02" };
                var result = dialog.ShowDialog();
                if (result != true)
                {
                    return;
                }

                if (Path.GetExtension(dialog.FileName.ToUpper()) == ".BIN")
                {
                    byte[][] _rom = AT28C010.DumpMemory();

                    Messenger.Default.Send(new NotificationMessage<RomFileModel>(new RomFileModel
                    {
                        Rom = _rom,
                        RomBanks = AT28C010.Banks,
                        RomBankSize = AT28C010.Length,
                        RomFilePath = dialog.FileName,
                        RomFileName = Path.GetFileName(dialog.FileName),
                    }, "FileLoaded"));
                }
                else if (Path.GetExtension(dialog.FileName.ToUpper()) == ".6502")
                {
                    var formatter = new BinaryFormatter();
                    Stream stream = new FileStream(dialog.FileName, FileMode.Open);
                    var fileModel = (StateFileModel)formatter.Deserialize(stream);

                    stream.Close();

                    Messenger.Default.Send(new NotificationMessage<StateFileModel>(fileModel, "StateLoaded"));
                }
            }
            else if (notificationMessage.Notification == "SaveState")
            {
                var dialog = new SaveFileDialog { DefaultExt = ".65C02", Filter = "WolfNet W65C02 Emulator Save State (*.65C02)|*.65C02" };
                var result = dialog.ShowDialog();

                if (result != true)
                {
                    return;
                }

                var formatter = new BinaryFormatter();
                Stream stream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None);

                formatter.Serialize(stream, new StateFileModel
                {
                    NumberOfCycles = NumberOfCycles,
                    OutputLog = OutputLog,
                    W65C02 = W65C02,
                    W65C22 = W65C22,
                    MM65SIB = MM65SIB,
                    W65C51 = W65C51,
                    AT28C010 = AT28C010,
                    AT28C64 = AT28C64,
            });
                stream.Close();
            }
			else
			{
                return;
            }
        }

        private void SettingsAppliedNotifcation(NotificationMessage<SettingsModel> notificationMessage)
        {
            if (notificationMessage.Notification != "SettingsApplied")
            {
                return;
            }
			SettingsModel = notificationMessage.Content;
            W65C51.Init(notificationMessage.Content.ComPortName);
			RaisePropertyChanged("SettingsModel");
			UpdateUi();
		}

		private void UpdateMemoryPage()
		{
			MemoryPage.Clear();
			var offset = (_memoryPageOffset * 256);
			
			var multiplyer = 0;
			for (ushort i = (ushort)offset; i < 256 * (_memoryPageOffset + 1); i++)
			{

				MemoryPage.Add(new MemoryRowModel
				{
					Offset = ((16 * multiplyer) + offset).ToString("X").PadLeft(4, '0'),
					Location00 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location01 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location02 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location03 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location04 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location05 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location06 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location07 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location08 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location09 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location0A = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location0B = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location0C = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location0D = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location0E = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
					Location0F = MemoryMap.ReadWithoutCycle(i).ToString("X").PadLeft(2, '0'),
				});
				multiplyer++;
			}
		}

		private void Reset()
		{
			IsRunning = false;

			if (_backgroundWorker.IsBusy)
				_backgroundWorker.CancelAsync();

			// "Reset" the Hardware...
            W65C02.Reset();
            RaisePropertyChanged("W65C02");
			W65C22.Reset();
            RaisePropertyChanged("W65C22");
			MM65SIB.Reset();
			RaisePropertyChanged("MM65SIB");
			W65C51.Reset();
			RaisePropertyChanged("W65C51");
			HM62256.Reset();
			RaisePropertyChanged("HM62256");

            IsRunning = false;
			NumberOfCycles = 0;
			RaisePropertyChanged("NumberOfCycles");

			UpdateMemoryPage();
			RaisePropertyChanged("MemoryPage");

			OutputLog.Clear();
			RaisePropertyChanged("CurrentDisassembly");

			OutputLog.Insert(0, GetOutputLog());
			UpdateUi();
		}

		private void Step()
		{
			IsRunning = false;

			if (_backgroundWorker.IsBusy)
				_backgroundWorker.CancelAsync();

			StepProcessor();
			UpdateMemoryPage();

			OutputLog.Insert(0, GetOutputLog());
			UpdateUi();
		}

		private void UpdateUi()
		{
			RaisePropertyChanged("W65C02");
			RaisePropertyChanged("NumberOfCycles");
			RaisePropertyChanged("CurrentDisassembly");
			RaisePropertyChanged("MemoryPage");
		}

		private void StepProcessor()
		{
			W65C02.NextStep();
			NumberOfCycles = W65C02.GetCycleCount();
		}

		private OutputLog GetOutputLog()
		{
			if (W65C02.CurrentDisassembly == null)
			{
				return new OutputLog(new Disassembly());
			}

			return new OutputLog(W65C02.CurrentDisassembly)
			{
				XRegister = W65C02.XRegister.ToString("X").PadLeft(2, '0'),
				YRegister = W65C02.YRegister.ToString("X").PadLeft(2, '0'),
				Accumulator = W65C02.Accumulator.ToString("X").PadLeft(2, '0'),
				NumberOfCycles = NumberOfCycles,
				StackPointer = W65C02.StackPointer.ToString("X").PadLeft(2, '0'),
				ProgramCounter = W65C02.ProgramCounter.ToString("X").PadLeft(4, '0'),
				CurrentOpCode = W65C02.CurrentOpCode.ToString("X").PadLeft(2, '0')
			};
		}

		private void RunPause()
		{
			var isRunning = !IsRunning;

			if (isRunning)
				_backgroundWorker.RunWorkerAsync();
			else
				_backgroundWorker.CancelAsync();

			IsRunning = !IsRunning;
		}

		private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			var outputLogs = new List<OutputLog>();

			while (true)
			{
				if (worker != null && worker.CancellationPending || IsBreakPointTriggered())
				{
					e.Cancel = true;

					RaisePropertyChanged("W65C02");

					foreach (var log in outputLogs)
						OutputLog.Insert(0, log);

					UpdateMemoryPage();
					return;
				}

				StepProcessor();
				outputLogs.Add(GetOutputLog());

				if (NumberOfCycles % GetLogModValue() == 0)
				{
					foreach (var log in outputLogs)
						OutputLog.Insert(0, log);

					outputLogs.Clear();
					UpdateUi();
				}
				Thread.Sleep(GetSleepValue());
			}
		}

		private bool IsBreakPointTriggered()
		{
			//This prevents the Run Command from getting stuck after reaching a breakpoint
			if (_breakpointTriggered)
			{
				_breakpointTriggered = false;
				return false;
			}

			foreach (var breakpoint in Breakpoints.Where(x => x.IsEnabled))
			{
				if (!int.TryParse(breakpoint.Value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int value))
					continue;

				if (breakpoint.Type == BreakpointType.NumberOfCycleType && value == NumberOfCycles)
				{
					_breakpointTriggered = true;
					RunPause();
					return true;
				}

				if (breakpoint.Type == BreakpointType.ProgramCounterType && value == W65C02.ProgramCounter)
				{
					_breakpointTriggered = true;
					RunPause();
					return true;
				}
			}

			return false;
		}

		private int GetLogModValue()
		{
			switch (CpuSpeed)
			{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
					return 1;
				case 6:
					return 5;
				case 7:
					return 20;
				case 8:
					return 30;
				case 9:
					return 40;
				case 10:
					return 50;
				default:
					return 5;
			}
		}

		private int GetSleepValue()
		{
			switch (CpuSpeed)
			{
				case 0:
					return 550;
				case 1:
					return 550;
				case 2:
					return 440;
				case 3:
					return 330;
				case 4:
					return 220;
				case 5:
					return 160;
				case 6:
					return 80;
				case 7:
					return 40;
				case 8:
					return 20;
				case 9:
					return 10;
				case 10:
					return 5;
				default:
					return 5;
			}
		}

        private void About()
        {
            IsRunning = false;

            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();

			MessageBox.Show(string.Format("{0}\n{1}\nVersion: {2}\nCompany: {3}", Versioning.Product.Name, Versioning.Product.Description, Versioning.Product.Version, Versioning.Product.Company), Versioning.Product.Title);
        }

        private void Settings()
        {
            IsRunning = false;

            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();

            Messenger.Default.Send(new NotificationMessage<SettingsModel>(SettingsModel, "SettingsWindow"));
        }

        private void AddBreakPoint()
		{
			Breakpoints.Add(new Breakpoint());
			RaisePropertyChanged("Breakpoints");
		}

		private void RemoveBreakPoint()
		{
			if (SelectedBreakpoint == null)
				return;

			Breakpoints.Remove(SelectedBreakpoint);
			SelectedBreakpoint = null;
			RaisePropertyChanged("SelectedBreakpoint");
		}
		#endregion
	}
}
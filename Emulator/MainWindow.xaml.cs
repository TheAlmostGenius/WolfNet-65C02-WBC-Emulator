using GalaSoft.MvvmLight.Messaging;
using Emulator.Model;
using Emulator.ViewModel;
using Hardware;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;

namespace Emulator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IClosable
    {
        public MainWindow()
		{
			InitializeComponent();
			Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
			Messenger.Default.Register<NotificationMessage<StateFileModel>>(this, NotificationMessageReceived);
            Messenger.Default.Register<NotificationMessage<SettingsModel>>(this, NotificationMessageReceived);
            Initialized += new EventHandler(OnLoad);
            Closing += new CancelEventHandler(OnClose);
            DataContext = new MainViewModel();
        }

        private void OnLoad(Object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { DefaultExt = ".bin", Filter = "All Files (*.bin, *.6502)|*.bin;*.6502|Binary Assembly (*.bin)|*.bin|WolfNet 65C02 Emulator Save State (*.6502)|*.6502" };

            var result = dialog.ShowDialog();

            if (result != true)
            {
                return;
            }
            if (Path.GetExtension(dialog.FileName.ToUpper()) == ".BIN")
            {
                byte[][] _rom = ConvertByteArrayToJagged(MemoryMap.BankedRom.TotalBanks, MemoryMap.BankedRom.BankSize, File.ReadAllBytes(dialog.FileName));
                
                Messenger.Default.Send(new NotificationMessage<RomFileModel>(new RomFileModel
                {
                    Rom = _rom,
                    RomBanks = (byte)_rom.GetLength(0),
                    RomBankSize = (ushort)_rom[0].Length,
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

                Messenger.Default.Send(new NotificationMessage<StateFileModel>(fileModel, "FileLoaded"));
            }
            
        }

        private byte[][] ConvertByteArrayToJagged(ushort elements, ushort bytesPerElement, byte[] array)
        {
            byte[][] jagged = new byte[elements][];
            int k = 0;

            for (int i = 0; i < jagged.Length; i++)
            {
                jagged[i] = new byte[bytesPerElement];
                for (int j = 0; j < jagged[i].Length; j++)
                {
                    if (k == array.Length) { break; }
                    jagged[i][j] = array[k];
                    k++;
                }
            }

            return jagged;
        }

        private void OnClose(Object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            if (e.Cancel)
            {
                return;
            }
            Hardware.W65C51.Fini();
            Stream stream = new FileStream(Emulator.FileLocations.SettingsFile, FileMode.Create, FileAccess.Write, FileShare.None);
            XmlSerializer XmlFormatter = new XmlSerializer(typeof(SettingsModel));
            XmlFormatter.Serialize(stream, MainViewModel.SettingsModel);
            stream.Flush();
            stream.Close();
            Hardware.W65C02.ClearMemory();
        }

        private void NotificationMessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "OpenFileWindow")
            {
                var openFile = new OpenFile();
                openFile.ShowDialog();
            }
            else if (notificationMessage.Notification == "AboutWindow")
            {
                var openAbout = new About();
                openAbout.ShowDialog();
            }
        }

        private void NotificationMessageReceived(NotificationMessage<StateFileModel> notificationMessage)
        {
            if (notificationMessage.Notification == "SaveFileWindow")
            {
                var saveFile = new SaveFile { DataContext = new SaveFileViewModel(notificationMessage.Content) };
                saveFile.ShowDialog();
            }
        }

        private void NotificationMessageReceived(NotificationMessage<SettingsModel> notificationMessage)
        {
            if (notificationMessage.Notification == "SettingsWindow")
            {
                var settingsFile = new Settings { DataContext = new SettingsViewModel(notificationMessage.Content) };
                settingsFile.ShowDialog();
            }
        }
    }
}

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
            Closing += new CancelEventHandler(OnClose);
            //DataContext = new MainViewModel();
        }

        private void OnClose(Object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            if (e.Cancel)
            {
                return;
            }
            W65C51.Fini();
            Stream stream = new FileStream(FileLocations.SettingsFile, FileMode.Create, FileAccess.Write, FileShare.None);
            XmlSerializer XmlFormatter = new XmlSerializer(typeof(SettingsModel));
            XmlFormatter.Serialize(stream, MainViewModel.SettingsModel);
            stream.Flush();
            stream.Close();
            W65C02.ClearMemory();
        }

        private void LoadFile(Object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("LoadFile"));
        }

        private void SaveFile(Object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("SaveState"));
        }

        private void NotificationMessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "AboutWindow")
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

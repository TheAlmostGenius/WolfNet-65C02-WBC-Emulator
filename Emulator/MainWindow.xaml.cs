using GalaSoft.MvvmLight.Messaging;
using Emulator.Model;
using Emulator.ViewModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

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
            DataContext = new MainViewModel();
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

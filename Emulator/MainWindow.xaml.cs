using GalaSoft.MvvmLight.Messaging;
using Emulator.Model;
using Emulator.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using Hardware;
using System.IO;
using System.Xml.Serialization;

namespace Emulator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IClosable
    {
        public DependencyProperty CurrentHeight
        {
            get { return Window.ActualHeightProperty; }
        }

        public DependencyProperty CurrentWidth
        {
            get { return Window.ActualWidthProperty; }
        }

        public MainWindow()
		{
			InitializeComponent();
			Messenger.Default.Register<NotificationMessage<StateFileModel>>(this, NotificationMessageReceived);
            Messenger.Default.Register<NotificationMessage<SettingsModel>>(this, NotificationMessageReceived);
        }

        private void LoadFile(Object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("LoadFile"));
        }

        private void SaveFile(Object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("SaveState"));
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

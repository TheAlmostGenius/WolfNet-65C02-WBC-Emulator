using GalaSoft.MvvmLight.Messaging;
using Emulator.Model;
using Emulator.ViewModel;
using System;
using System.Windows;

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
            Messenger.Default.Register<NotificationMessage<SettingsModel>>(this, NotificationMessageReceived);
            Closing += new CancelEventHandler(OnClose);
        }

        private void OnClose(Object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            if (e.Cancel)
            {
                return;
            }
            Messenger.Default.Send("Closing");
<<<<<<< HEAD
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
=======
>>>>>>> parent of 74ec302 (Finished handling on closure of window...)
        }

        private void LoadFile(Object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("LoadFile"));
        }

        private void SaveFile(Object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("SaveState"));
        }

        private void CloseFile(Object sender, EventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("CloseFile"));
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

using GalaSoft.MvvmLight.Messaging;
using Simulator.Model;
using Simulator.ViewModel;
using System;
using System.ComponentModel;

namespace Simulator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
			Messenger.Default.Register<NotificationMessage<StateFileModel>>(this, NotificationMessageReceived);
            Messenger.Default.Register<NotificationMessage<SettingsModel>>(this, NotificationMessageReceived);
            this.Closing += new CancelEventHandler(this.OnClose);
        }

		private void OnClose(Object sender, CancelEventArgs e)
		{
			e.Cancel = false;
			Hardware.W65C51.Fini();
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

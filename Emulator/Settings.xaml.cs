using GalaSoft.MvvmLight.Messaging;
using Simulator.Model;
using Simulator.ViewModel;
using System;

namespace Simulator
{
	/// <summary>
	/// Interaction logic for SaveState.xaml
	/// </summary>
	public partial class Settings
	{
		public Settings()
		{
			InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            Messenger.Default.Register<NotificationMessage<SettingsModel>>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "CloseSettingsWindow")
            {
                Close();
            }
        }

        private void NotificationMessageReceived(NotificationMessage<SettingsModel> notificationMessage)
        {
            if (notificationMessage.Notification == "SettingsWindow")
            {
                SettingsViewModel.SettingsModel = notificationMessage.Content;
                ComPortCombo.SelectedItem = notificationMessage.Content.ComPortName;
            }
        }

        private void PortSelectionDropDownClosed(object sender, EventArgs e)
        {
            string port = ComPortCombo.SelectedValue.ToString();
            SettingsViewModel.ComPortSelection = port;
        }
    }
}

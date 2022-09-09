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
		}

		private void NotificationMessageReceived(NotificationMessage notificationMessage)
		{
			if (notificationMessage.Notification == "CloseSettingsWindow")
                Close();
        }

        private void PortSelectionDropDownClosed(object sender, EventArgs e)
        {
            string port = ComPortCombo.SelectedValue.ToString();
            SettingsViewModel.ComPortSelection = port;
        }
    }
}

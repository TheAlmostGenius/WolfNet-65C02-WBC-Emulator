using Emulator.Model;
using Emulator.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace Emulator
{
    /// <summary>
    /// Interaction logic for Settings.xaml
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
            else if (notificationMessage.Notification == "CloseAll")
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
            if (!(ComPortCombo.SelectedValue == null))
            {
                string port = ComPortCombo.SelectedValue.ToString();
                SettingsViewModel.ComPortSelection = port;
            }
        }
    }
}

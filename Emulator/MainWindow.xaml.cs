using Emulator.Model;
using Emulator.ViewModel;
using GalaSoft.MvvmLight.Messaging;
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
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            Messenger.Default.Register<NotificationMessage<SettingsModel>>(this, NotificationMessageReceived);
        }

        private void ToClose(Object sender, EventArgs e)
        {
            Close();
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

        private void NotificationMessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "CloseWindow")
            {
                Close();
            }
            else if (notificationMessage.Notification == "MemoryVisualWindow")
            {
                var memoryVisual = new MemoryVisual { DataContext = new MemoryVisualViewModel() };
                memoryVisual.Show();
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

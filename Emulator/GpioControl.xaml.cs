using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace Emulator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class GpioControl : Window
    {
        public GpioControl()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "CloseGpioControlWindow")
            {
                Close();
            }
            else if (notificationMessage.Notification == "CloseAll")
            {
                Close();
            }
        }
    }
}

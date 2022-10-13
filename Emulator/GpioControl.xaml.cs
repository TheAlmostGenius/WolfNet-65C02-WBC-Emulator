using GalaSoft.MvvmLight.Messaging;
using Hardware;
using System.Windows;

namespace Emulator
{
    /// <summary>
    /// Interaction logic for GpioControl.xaml
    /// </summary>
    public partial class GpioControl : Window
    {
        W65C22 W65C22 { get; set; }

        public GpioControl()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            Messenger.Default.Register<NotificationMessage<W65C22>>(this, NotificationMessageReceived);
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

        private void NotificationMessageReceived(NotificationMessage<W65C22> notificationMessage)
        {
            if (notificationMessage.Notification == "GpioControlWindow")
            {
                W65C22 = notificationMessage.Content;
            }
        }
    }
}

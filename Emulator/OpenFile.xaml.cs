using GalaSoft.MvvmLight.Messaging;
using Emulator.ViewModel;
using System;

namespace Emulator
{
	/// <summary>
	/// Interaction logic for OpenFile.xaml
	/// </summary>
	public partial class OpenFile
	{
		public OpenFile()
		{
			InitializeComponent();
			Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
		}

		private void NotificationMessageReceived(NotificationMessage notificationMessage)
		{
			if (notificationMessage.Notification == "CloseFileWindow")
				Close();
		}
    }
}

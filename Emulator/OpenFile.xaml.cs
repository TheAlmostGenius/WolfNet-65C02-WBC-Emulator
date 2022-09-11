using GalaSoft.MvvmLight.Messaging;
using Simulator.ViewModel;
using System;

namespace Simulator
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
        private void HandleBiosOnlyChecked(object sender, EventArgs e)
        {
            OpenFileViewModel.IsBiosOnly = true;
        }

        private void HandleBiosOnlyUnchecked(object sender, EventArgs e)
        {
            OpenFileViewModel.IsBiosOnly = false;
        }
    }
}

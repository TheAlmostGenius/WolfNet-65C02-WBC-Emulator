using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;
using System;
using OpenFileView = Simulator.ViewModel.OpenFileViewModel;

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

        private void PortSelectionDropDownClosed(object sender, EventArgs e)
        {
            string port = ComPortCombo.SelectedValue.ToString();
			OpenFileView.ComPortSelection = port; 
        }
    }
}

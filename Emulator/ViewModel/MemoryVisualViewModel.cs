using Emulator.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hardware;
using System;

namespace Emulator.ViewModel
{
    /// <summary>
    /// The Main ViewModel
    /// </summary>
    public class MemoryVisualViewModel : ViewModelBase
    {
        #region Fields
        private int _memoryPageOffset;
        #endregion

        #region Properties
        /// <summary>
        /// The Current Memory Page
        /// </summary>
        public MultiThreadedObservableCollection<MemoryRowModel> MemoryPage { get; set; }

        /// <summary>
        /// The Memory Page number.
        /// </summary>
        public string MemoryPageOffset
        {
            get { return _memoryPageOffset.ToString("X"); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                try
                {
                    _memoryPageOffset = Convert.ToInt32(value, 16);
                }
                catch { }
            }
        }

        /// <summary>
        /// Relay Command that updates the Memory Map when the Page changes
        /// </summary>
        public RelayCommand UpdateMemoryMapCommand { get; set; }
        #endregion

        #region public Methods
        /// <summary>
        /// Creates a new Instance of the MemoryVisualViewModel.
        /// </summary>
        public MemoryVisualViewModel()
        {
            UpdateMemoryMapCommand = new RelayCommand(UpdateMemoryPage);

            Messenger.Default.Register<NotificationMessage>(this, GenericNotifcation);

            MemoryPage = new MultiThreadedObservableCollection<MemoryRowModel>();

            UpdateMemoryPage();
        }

        private void GenericNotifcation(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "UpdateMemoryPage")
            {
                UpdateMemoryPage();
            }
        }

        public void UpdateMemoryPage()
        {
            MemoryPage.Clear();
            var offset = _memoryPageOffset * 256;

            var multiplyer = 0;
            for (ushort i = (ushort)offset; i < 256 * (_memoryPageOffset + 1); i++)
            {

                MemoryPage.Add(new MemoryRowModel
                {
                    Offset = ((16 * multiplyer) + offset).ToString("X").PadLeft(4, '0'),
                    Location00 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location01 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location02 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location03 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location04 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location05 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location06 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location07 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location08 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location09 = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location0A = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location0B = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location0C = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location0D = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location0E = MemoryMap.ReadWithoutCycle(i++).ToString("X").PadLeft(2, '0'),
                    Location0F = MemoryMap.ReadWithoutCycle(i).ToString("X").PadLeft(2, '0'),
                });
                multiplyer++;
            }
        }
        #endregion

        #region Private Methods
        private void UpdateUi()
        {
            RaisePropertyChanged("W65C02");
            RaisePropertyChanged("NumberOfCycles");
            RaisePropertyChanged("CurrentDisassembly");
            RaisePropertyChanged("MemoryPage");
        }
        #endregion
    }
}
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Hardware;

namespace Emulator.ViewModel
{
    /// <summary>
    /// The ViewModel Used by the GpioControl
    /// </summary>
    public class GpioControlViewModel : ViewModelBase
    {
        W65C22 W65C22 { get; set; }

        public bool PA0_State
        {
            get
            {
                if (W65C22.DDRA == 0x01)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) | 0x01));
                }
                else
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) & ~0x01));
                }
            }
        }

        public bool PA1_State
        {
            get
            {
                if (W65C22.DDRA == 0x02)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) | 0x02));
                }
                else
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) & ~0x02));
                }
            }
        }

        public bool PA2_State
        {
            get
            {
                if (W65C22.DDRA == 0x04)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) | 0x04));
                }
                else
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) & ~0x04));
                }
            }
        }

        public bool PA3_State
        {
            get
            {
                if (W65C22.DDRA == 0x08)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) | 0x08));
                }
                else
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) & ~0x08));
                }
            }
        }

        public bool PA4_State
        {
            get
            {
                if (W65C22.DDRA == 0x10)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) | 0x10));
                }
                else
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) & ~0x10));
                }
            }
        }

        public bool PA5_State
        {
            get
            {
                if (W65C22.DDRA == 0x20)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) | 0x20));
                }
                else
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) & ~0x20));
                }
            }
        }

        public bool PA6_State
        {
            get
            {
                if (W65C22.DDRA == 0x40)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) | 0x40));
                }
                else
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) & ~0x40));
                }
            }
        }

        public bool PA7_State
        {
            get
            {
                if (W65C22.DDRA == 0x01)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) | 0x01));
                }
                else
                {
                    W65C22.Write(W65C22.IORA, (byte)(W65C22.Read(W65C22.IORA) & ~0x01));
                }
            }
        }

        public bool PB0_State
        {
            get
            {
                if (W65C22.DDRB == 0x01)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) | 0x01));
                }
                else
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) & ~0x01));
                }
            }
        }

        public bool PB1_State
        {
            get
            {
                if (W65C22.DDRB == 0x02)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) | 0x02));
                }
                else
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) & ~0x02));
                }
            }
        }

        public bool PB2_State
        {
            get
            {
                if (W65C22.DDRB == 0x04)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) | 0x04));
                }
                else
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) & ~0x04));
                }
            }
        }

        public bool PB3_State
        {
            get
            {
                if (W65C22.DDRB == 0x08)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) | 0x08));
                }
                else
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) & ~0x08));
                }
            }
        }

        public bool PB4_State
        {
            get
            {
                if (W65C22.DDRB == 0x10)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) | 0x10));
                }
                else
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) & ~0x10));
                }
            }
        }

        public bool PB5_State
        {
            get
            {
                if (W65C22.DDRB == 0x20)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) | 0x20));
                }
                else
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) & ~0x20));
                }
            }
        }

        public bool PB6_State
        {
            get
            {
                if (W65C22.DDRB == 0x40)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) | 0x40));
                }
                else
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) & ~0x40));
                }
            }
        }

        public bool PB7_State
        {
            get
            {
                if (W65C22.DDRB == 0x80)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value == true)
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) | 0x80));
                }
                else
                {
                    W65C22.Write(W65C22.IORB, (byte)(W65C22.Read(W65C22.IORB) & ~0x80));
                }
            }
        }

        #region Public Methods
        /// <summary>
        /// Instantiates a new instance of the GpioControlViewModel. This is used by the IOC to create the default instance.
        /// </summary>
        [PreferredConstructor]
        public GpioControlViewModel()
        {
            Messenger.Default.Register<NotificationMessage<W65C22>>(this, NotificationMessageReceived);
        }
        #endregion

        #region Private Methods
        private static void Close()
        {
            Messenger.Default.Send(new NotificationMessage("CloseGpioControlWindow"));
        }

        private void NotificationMessageReceived(NotificationMessage<W65C22> notificationMessage)
        {
            if (notificationMessage.Notification == "GpioControlWindow")
            {
                W65C22 = notificationMessage.Content;
            }
        }
        #endregion
    }
}

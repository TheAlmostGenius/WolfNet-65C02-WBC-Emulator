using System;
using System.IO;
using System.Xml.Schema;

namespace Hardware
{
	public class MemoryMap
	{
		public static class BankedRam
		{
            private static ushort _Offset = 0x0000;
            private static ushort _Length = 0x7FFF;

            public static ushort CurrentBank { get; set; }
            public static ushort Offset => _Offset;
            public static ushort Length => _Length;
        }

		public static class DeviceArea
		{
			private static ushort _Offset = 0xD000;
			private static ushort _Length = 0x00FF;

			public static ushort Offset => _Offset;
			public static ushort Length => _Length;
		}

		public static class BankedRom
		{
			private static ushort _Offset = 0x8000;
            private static ushort _Length = 0x3FFF;

            public static int TotalLength = (BankSize * TotalBanks);
            public static byte TotalBanks = 16;
			public static ushort BankSize = (ushort)(Length + 1);

            public static ushort CurrentBank { get; set; }
			public static ushort Offset => _Offset;
            public static ushort Length => _Length;
        }

		public static class SharedRom
		{
			private static ushort _Offset = 0xE000;
            private static ushort _Length = 0x1FFF;

            public static ushort Offset => _Offset;
            public static ushort Length => _Length;
        }

        private static int Length { get; set; }
        private static W65C02 Processor { get; set; }
        private static W65C22 GPIO { get; set; }
        private static W65C22 MM65SIB { get; set; }
        private static W65C51 ACIA { get; set; }
        private static HM62256 RAM { get; set; }
        private static AT28CXX SharedROM { get; set; }
        private static AT28CXX BankedROM { get; set; }
        public static byte[] MemorySpace { get; set; }

        public static void Init(int length, W65C02 processor, W65C22 gpio, W65C22 mm65sib, W65C51 acia, HM62256 ram, AT28CXX bankedRom, AT28CXX sharedRom)
		{
            MemorySpace = new byte[length];
            Length = length;
            BankedRom.CurrentBank = 0;
            BankedRam.CurrentBank = 0;
            Processor = processor;
			GPIO = gpio;
			MM65SIB = mm65sib;
			ACIA = acia;
			RAM = ram;
			SharedROM = sharedRom;
            BankedROM = bankedRom;
		}

        /// <summary>
        /// Returns the byte at the given address.
        /// </summary>
        /// <param name="address">The address to return</param>
        /// <returns>the byte being returned</returns>
        public static byte Read(int address)
        {
            var value = ReadWithoutCycle(address);
            Processor.IncrementCycleCount();
            return value;
        }

        public static byte ReadWithoutCycle(int address)
		{
            if (address == ACIA.address)
            {
                return ACIA.byteIn;
            }
            else if (address == GPIO.ACR)
            {
                byte data = (byte)0;
                if (GPIO.T1TimerControl)
                {
                    data = (byte)(data | GPIO.ACR_T1TC);
                }
                else if (GPIO.T2TimerControl)
                {
                    data = (byte)(data | GPIO.ACR_T2TC);
                }
                return data;

            }
            else if (0xD000 < address && address <= 0xD0FF)
            {
                throw new ArgumentOutOfRangeException("Device area accessed where there is no device!");
            }
            else if (BankedRom.Offset < address && address <= (BankedRom.Offset & BankedRom.Length))
            {
                return SharedROM.Memory[BankedRom.CurrentBank][address];
            }
            else if (BankedRam.Offset < address && address <= (BankedRam.Offset & BankedRam.Length))
            {
                return RAM.Memory[address];
            }
            else
            {
                return 0x00;
            }
        }

        /// <summary>
        /// Writes data to the given address.
        /// </summary>
        /// <param name="address">The address to write data to</param>
        /// <param name="data">The data to write</param>
        public static void Write(int address, byte data)
        {
            Processor.IncrementCycleCount();
            WriteWithoutCycle(address, data);
        }

        /// <summary>
        /// Writes data to the given address without incrementing the cycle.
        /// </summary>
        /// <param name="address">The address to write data to</param>
        /// <param name="data">The data to write</param>
        public static void WriteWithoutCycle(int address, byte data)
        {
            if (DeviceArea.Offset < address && address <= (DeviceArea.Offset & DeviceArea.Length))
            {
                if (address == ACIA.address)
                {
                    ACIA.WriteCOM(data);
                }
                else if ((address == GPIO.ACR) && ((data | GPIO.ACR_T1TC) == GPIO.ACR_T1TC))
                {
                    GPIO.T1TimerControl = true;
                }
                else if ((address == GPIO.ACR) && ((data | GPIO.ACR_T2TC) == GPIO.ACR_T2TC))
                {
                    GPIO.T2TimerControl = true;
                }
                else if ((address == GPIO.IER) && ((data | GPIO.IER_T1) == GPIO.IER_T1) && ((data | GPIO.IER_EN) == GPIO.IER_EN))
                {
                    GPIO.T1Init(GPIO.T1Interval);
                }
                else if ((address == GPIO.IER) && ((data | GPIO.IER_T2) == GPIO.IER_T2) && ((data | GPIO.IER_EN) == GPIO.IER_EN))
                {
                    GPIO.T2Init(GPIO.T2Interval);
                }
                else
                {
                    return;
                }
            }
            else if (BankedRom.Offset < address && address <= (BankedRom.Offset & BankedRom.Length))
            {
                SharedROM.Memory[BankedRom.CurrentBank][address] = data;
            }
            else if (BankedRam.Offset < address && address <= (BankedRam.Offset & BankedRam.Length))
            {
                RAM.Memory[address] = data;
            }
            else
            {
                return;
            }
        }
    }
}
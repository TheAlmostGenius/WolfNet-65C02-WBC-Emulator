﻿using System;
using System.IO;

namespace Hardware
{
    public class MemoryMap
    {
        public static class BankedRam
        {
            private static int _Offset = 0x0000;
            private static int _Length = 0x7FFF;

            public static int TotalLength = (BankSize * TotalBanks) - 1;
            public static int BankSize = (int)(Length + 1);
            public static byte TotalBanks = 16;

            public static int Offset { get { return _Offset; } }
            public static int Length { get { return _Length; } }
        }

        public static class DeviceArea
        {
            private static int _Offset = 0xD000;
            private static int _Length = 0x00FF;

            /// <summary>
            /// The end of memory
            /// </summary>
            public static int End { get { return Offset + Length; } }
            public static int Offset { get { return _Offset; } }
            public static int Length { get { return _Length; } }
        }

        public static class BankedRom
        {
            private static int _Offset = 0x8000;
            private static int _Length = 0x3FFF;

            public static byte TotalBanks = 16;

            public static int Offset { get { return _Offset; } }
            public static int Length { get { return _Length; } }
        }

        public static class SharedRom
        {
            private static int _Offset = 0xE000;
            private static int _Length = 0x1FFF;

            public static byte TotalBanks = 1;

            public static int Offset { get { return _Offset; } }
            public static int Length { get { return _Length; } }
        }

        public static class Devices
        {
            public static class ACIA
            {
                public static int Length = 0x03;
                public static byte Offset = 0x10;
            }

            public static class GPIO
            {
                public static int Length = 0x0F;
                public static byte Offset = 0x20;
            }

            public static class MM65SIB
            {
                public static int Length = 0x0F;
                public static byte Offset = 0x30;
            }
        }

        private static W65C02 Processor { get; set; }
        private static W65C22 GPIO { get; set; }
        private static W65C22 MM65SIB { get; set; }
        private static W65C51 ACIA { get; set; }
        private static AT28CXX SharedROM { get; set; }
        private static AT28CXX BankedROM { get; set; }
        private static HM62256 BankedRAM { get; set; }

        public static void Init(W65C02 processor, W65C22 gpio, W65C22 mm65sib, W65C51 acia, HM62256 bankedRam, AT28CXX bankedRom, AT28CXX sharedRom)
        {
            Processor = processor;
            GPIO = gpio;
            MM65SIB = mm65sib;
            ACIA = acia;
            SharedROM = sharedRom;
            BankedROM = bankedRom;
            BankedRAM = bankedRam;
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
            int _address = address;
            if ((ACIA.Offset <= _address) && (_address <= (ACIA.Offset + ACIA.Length)))
            {
                return ACIA.byteIn;
            }
            else if ((GPIO.Offset <= _address) && (_address <= (GPIO.Offset + GPIO.Length)))
            {
                return GPIO.Read(_address);
            }
            else if ((DeviceArea.Offset <= _address) && (_address < DeviceArea.End))
            {
                throw new ArgumentOutOfRangeException("Device area accessed where there is no device!");
            }
            else if ((SharedROM.Offset <= _address) && (_address < SharedROM.End))
            {
                return SharedROM.Read(_address);
            }
            else if ((BankedROM.Offset <= _address) && (_address < BankedROM.End))
            {
                return BankedROM.Read(_address);
            }
            else if ((BankedRAM.Offset <= _address) && (_address < BankedRAM.End))
            {
                return BankedRAM.Read(_address);
            }
            else
            {
#if DEBUG
                StreamWriter file = File.CreateText(FileLocations.ErrorFile);
                string stringToWrite;
                stringToWrite = String.Format("Banked RAM Offset: {0} Banked RAM End: {1]", BankedRAM.Offset.ToString(), (BankedRAM.Offset + BankedRAM.Length));
                file.WriteLine(stringToWrite);
                stringToWrite = String.Format("Banked ROM Offset: {0} Banked ROM End: {1]", BankedROM.Offset.ToString(), (BankedROM.Offset + BankedROM.Length));
                file.WriteLine(stringToWrite);
                stringToWrite = String.Format("Shared ROM Offset: {0} Shared ROM End: {1]", SharedROM.Offset.ToString(), (SharedROM.Offset + SharedROM.Length));
                file.WriteLine(stringToWrite);
                file.Flush();
                file.Close();
                throw new ArgumentOutOfRangeException(String.Format("Cannot access memory at address: {0}", address));
#else
                return 0x00;
#endif
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
            if ((ACIA.Offset <= address) && (address <= (ACIA.Offset + ACIA.Length)))
            {
                if (address == ACIA.Offset)
                {
                    ACIA.WriteCOM(data);
                }
            }
            else if ((GPIO.Offset <= address) && (address <= (GPIO.Offset + GPIO.Length)))
            {
                GPIO.Write(address, data);
            }
            else if ((SharedROM.Offset <= address) && (address <= SharedROM.End))
            {
                SharedROM.Write(address, data);
            }
            else if ((BankedROM.Offset <= address) && (address <= BankedROM.End))
            {
                BankedROM.Write(address, data);
            }
            else if ((BankedRAM.Offset <= address) && (address <= BankedRAM.End))
            {
                BankedRAM.Write(address, data);
            }
            else
            {
#if DEBUG
                throw new ApplicationException(String.Format("Cannot write to address: {0}", address));
#endif
            }
        }
    }
}
using System;
using System.Xml.Linq;

namespace Hardware
{
	/// <summary>
    /// An implementation of a W65C02 Processor.
    /// </summary>
    [Serializable]
	public class AT28CXX
	{
        //All of the properties here are public and read only to facilitate ease of debugging and testing.
        #region Properties
        /// <summary>
        /// The ROM.
        /// </summary>
        public static byte[][] Memory { get; private set; }

        /// <summary>
        /// The total number of banks on the ROM.
        /// </summary>
        protected static byte Banks { get; private set; }

        /// <summary>
        /// The memory
        /// </summary>
        protected static ushort Offset { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Default Constructor, Instantiates a new instance of the processor.
        /// </summary>
        public AT28CXX(ushort offset, byte banks)
        {
            Memory = new byte[banks][];
            for (int i = 0; i < banks; i++)
            {
                Memory[i] = new byte[MemoryMap.BankedRom.BankSize];
            }
            Offset = offset;
            Banks = banks;
            //Reset();
        }

        /// <summary>
        /// Initializes the ROM to its default state.
        /// </summary>
        public static void Reset()
		{
            throw new NotImplementedException("Reset signal not valid for simulated ROM!");
        }

        /// <summary>
        /// 28CXX family initialization routine.
        /// </summary>
        public static void Init(byte[][] data)
        {
            throw new NotImplementedException("28CXX should not be initialised through this routine!");
        }

        /// <summary>
        /// Loads a program into ROM.
        /// </summary>
        /// <param name="data">The program to be loaded</param>
        public static void LoadRom(byte[][] data)
        {
            for (byte i = 0; i < MemoryMap.BankedRom.TotalBanks; i++)
            {
                LoadRom(i, data);
            }
        }

        /// <summary>
        /// Loads a program into ROM.
        /// </summary>
        /// <param name="bank">The bank to load data to.</param>
        /// <param name="data">The data to be loaded to ROM.</param>
        public static void LoadRom(byte bank, byte[][] data)
        {
            Memory[bank] = data[bank];
        }

        /// <summary>
        /// Loads a program into ROM.
        /// </summary>
        /// <param name="bank">The bank to load data to.</param>
        /// <param name="offset">The offset within the bank to load data to.</param>
        /// <param name="data">The data to be loaded to ROM.</param>
        public static void LoadRom(ushort offset, byte bank, byte[][] data)
        {
            for (ushort i = 0; i < MemoryMap.BankedRom.BankSize - 1; i++)
            {
                Memory[bank][offset + i] = data[bank][offset];
            }
            
        }

        /// <summary>
        /// Returns the byte at a given address without incrementing the cycle. Useful for test harness. 
        /// </summary>
        /// <param name="bank">The bank to read data from.</param>
        /// <param name="address"></param>
        /// <returns>the byte being returned</returns>
        public static byte ReadBankedRomValue(byte bank, ushort address)
        {
            return Memory[bank][address];
        }

        /// <summary>
        /// Dumps the entire memory object. Used when saving the memory state
        /// </summary>
        /// <returns>2 dimensional array of data analogous to the ROM of the computer.</returns>
        public static byte[][] DumpMemory()
        {
            return Memory;
        }

        public byte[][] ConvertByteArrayToJagged(ushort elements, ushort bytesPerElement, byte[] array)
        {
            byte[][] jagged = new byte[elements][];
            int k = 0;

            for (int i = 0; i < jagged.Length; i++)
            {
                jagged[i] = new byte[bytesPerElement];
                for (int j = 0; j < jagged[i].Length; j++)
                {
                    if (k == array.Length) { break; }
                    jagged[i][j] = array[k];
                    k++;
                }
            }

            return jagged;
        }

        /// <summary>
        /// Dumps the selected ROM bank.
        /// </summary>
        /// <param name="bank">The bank to dump data from.</param>
        /// <returns>2 dimensional array of data analogous to the ROM of the computer.</returns>
        public static byte[] DumpMemory(byte bank)
        {
            byte[] _tempMemory = new byte[MemoryMap.BankedRom.Length + 1];
            for (var i = 0; i < MemoryMap.BankedRom.Length; i++) {
                _tempMemory[i] = Memory[bank][i];
            }
            return _tempMemory;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Clears the memory.
        /// </summary>
        //private static void ClearMemory()
        //{
        //    for (byte i = 0; i < MemoryMap.BankedRom.TotalBanks; i++)
        //    {
        //        for (ushort j = 0; j < MemoryMap.BankedRom.Length; i++)
        //        {
        //            Memory[i][j] = 0x00;
        //        }
        //    }
        //}

        /// <summary>
        /// Writes data to the given address without incrementing the cycle.
        /// </summary>
        /// <param name="bank">The bank to load data to.</param>
        /// <param name="address">The address to write data to</param>
        /// <param name="data">The data to write</param>
        //private static void WriteBankedRomValue(byte bank, ushort address, byte data)
        //{
        //    Memory[bank][address] = data;
        //}
        #endregion
    }
}
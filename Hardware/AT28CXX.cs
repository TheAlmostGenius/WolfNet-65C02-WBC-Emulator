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
        protected static byte[][] Memory { get; private set; }

        /// <summary>
        /// The total number of banks on the ROM.
        /// </summary>
        protected static byte TotalBanks { get; private set; }

        /// <summary>
        /// The memory
        /// </summary>
        protected static ushort Offset { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Default Constructor, Instantiates a new instance of the processor.
        /// </summary>
        public AT28CXX(ushort offset, byte banks, byte[][] data)
        {
            Offset = offset;
            TotalBanks = banks;
            Init(TotalBanks, data);
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
        public static void Init(byte banks, byte[][] data)
        {
            Memory = new byte[banks][];
            LoadRom(data);
        }

        /// <summary>
        /// Loads a program into ROM.
        /// </summary>
        /// <param name="data">The program to be loaded</param>
        public static void LoadRom(byte[][] data)
        {
            for (byte i = 0; i < MemoryMap.BankedRom.TotalBanks; i++)
            {
                for (byte j = 0; j <= MemoryMap.BankedRom.Length; j++)
                {
                   LoadRom(i, j, data[i][j]);
                }
            }
        }

        /// <summary>
        /// Loads a program into ROM.
        /// </summary>
        /// <param name="data">The data to be loaded to ROM.</param>
        public static void LoadRom(byte bank, byte[][] data)
        {
            for (byte i = 0; i <= MemoryMap.BankedRom.Length; i++)
            {
                LoadRom(bank, i, data[bank][i]);
            }
        }

		/// <summary>
		/// Returns the byte at a given address without incrementing the cycle. Useful for test harness. 
		/// </summary>
		/// <param name="address"></param>
		/// <returns>the byte being returned</returns>
		public static byte ReadBankedRomValue(byte bank, ushort address)
		{
			var value = Memory[bank][address];
			return value;
        }

        /// <summary>
        /// Dumps the entire memory object. Used when saving the memory state
        /// </summary>
        /// <returns>2 dimensional array of data analogous to the ROM of the computer.</returns>
        public static byte[][] DumpMemory()
        {
            return Memory;
        }

        /// <summary>
        /// Dumps the selected ROM bank.
        /// </summary>
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
        private static void ClearMemory()
        {
            for (byte i = 0; i < MemoryMap.BankedRom.TotalBanks; i++)
            {
                for (ushort j = 0; j < MemoryMap.BankedRom.Length; i++)
                {
                    Memory[i][j] = 0x00;
                }
            }
        }

        /// <summary>
        /// Writes data to the given address without incrementing the cycle.
        /// </summary>
        /// <param name="address">The address to write data to</param>
        /// <param name="data">The data to write</param>
        private static void WriteBankedRomValue(byte bank, ushort address, byte data)
        {
            Memory[bank][address] = data;
        }

        /// <summary>
        /// Loads a program into ROM.
        /// </summary>
        /// <param name="data">The data to be loaded to ROM.</param>
        private static void LoadRom(byte bank, ushort offset, byte data)
        {
            Memory[bank][offset] = data;
        }
        #endregion
    }
}
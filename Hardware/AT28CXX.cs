using System;
using System.IO;

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
        public byte[][] Memory { get; private set; }

        /// <summary>
        /// The total number of banks on the ROM.
        /// </summary>
        public byte Banks { get; private set; }

        /// <summary>
        /// The bank the ROM is currently using.
        /// </summary>
        public byte CurrentBank { get; private set; }

        /// <summary>
        /// The memory offset
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// The end of memory
        /// </summary>
        public int End { get { return Offset + Length; } }

        /// <summary>
        /// The memory length
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The processor reference
        /// </summary>
        public W65C02 Processor{ get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Default Constructor, Instantiates a new instance of the processor.
        /// </summary>
        public AT28CXX(int offset, int length, byte banks)
        {
            Memory = new byte[banks][];
            for (int i = 0; i < banks; i++)
            {
                Memory[i] = new byte[length + 1];
            }
            Offset = offset;
            Length = length;
            Banks = banks;
            CurrentBank = 0;
        }

        /// <summary>
        /// Initializes the ROM to its default state.
        /// </summary>
        public void Reset()
		{
            throw new NotImplementedException("Reset signal not valid for simulated ROM!");
        }

        /// <summary>
        /// 28CXX family initialization routine.
        /// </summary>
        public void Init()
        {
            throw new NotImplementedException("28CXX should not be initialised through this routine!");
        }

        /// <summary>
        /// Loads a program into ROM.
        /// </summary>
        /// <param name="data">The program to be loaded</param>
        public void Load(byte[][] data)
        {
            for (byte i = 0; i < Banks; i++)
            {
                Load(i, data[i]);
            }
        }

        /// <summary>
        /// Loads a program into ROM.
        /// </summary>
        /// <param name="bank">The bank to load data to.</param>
        /// <param name="data">The data to be loaded to ROM.</param>
        public void Load(byte bank, byte[] data)
        {
            for (int i = 0; i <= Length; i++)
            {
                Memory[bank][i] = data[i];
            }
        }

        public byte[][] TryRead(string filename)
        {
            byte[][] bios = new byte[Banks][];
            try
            {
                FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);
                for (int i = 0; i < Banks; i++)
                {
                    bios[i] = new byte[Length + 1];
                    for (int j = 0; j <= Length; j++)
                    {
                        bios[i][j] = new byte();
                        bios[i][j] = (byte)file.ReadByte();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return bios;
        }

        /// <summary>
        /// Returns the byte at a given address without incrementing the cycle. Useful for test harness. 
        /// </summary>
        /// <param name="bank">The bank to read data from.</param>
        /// <param name="address"></param>
        /// <returns>the byte being returned</returns>
        public byte Read(int address)
        {
            return Memory[CurrentBank][address - Offset];
        }

        /// <summary>
        /// Writes data to the given address without incrementing the cycle.
        /// </summary>
        /// <param name="bank">The bank to load data to.</param>
        /// <param name="address">The address to write data to</param>
        /// <param name="data">The data to write</param>
        public void Write(int address, byte data)
        {
            throw new NotSupportedException("Writing to ROM is not supported by the software as it isn't supported in the real world!");
        }

        /// <summary>
        /// Dumps the entire memory object. Used when saving the memory state
        /// </summary>
        /// <returns>2 dimensional array of data analogous to the ROM of the computer.</returns>
        public byte[][] DumpMemory()
        {
            return Memory;
        }

        /// <summary>
        /// Dumps the selected ROM bank.
        /// </summary>
        /// <param name="bank">The bank to dump data from.</param>
        /// <returns>Array that represents the selected ROM bank.</returns>
        public byte[] DumpMemory(byte bank)
        {
            byte[] _tempMemory = new byte[MemoryMap.BankedRom.Length + 1];
            for (var i = 0; i < MemoryMap.BankedRom.Length; i++) {
                _tempMemory[i] = Memory[bank][i];
            }
            return _tempMemory;
        }

        /// <summary>
        /// Clears the ROM.
        /// </summary>
        public void Clear()
        {
            for (byte i = 0; i < Banks; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    Memory[i][j] = 0x00;
                }
            }
        }
#endregion
    }
}
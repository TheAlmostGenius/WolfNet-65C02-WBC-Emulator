using System;

namespace Hardware
{
    public class HM62256
    {
        /// <summary>
        /// The memory area.
        /// </summary>
        public byte[][] Memory { get; set; }

        /// <summary>
        /// The memory offset.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The memory length.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The location of the end of memory.
        /// </summary>
        public int End { get { return Offset + Length; } }

        /// <summary>
        /// The number of banks the memory has.
        /// </summary>
        public byte Banks { get; set; }

        /// <summary>
        /// The currently selected bank.
        /// </summary>
        public byte CurrentBank { get; set; }

        /// <summary>
        /// Called whenever a new 62256 object is required.
        /// </summary>
        /// <param name="banks">Number of banks the new memory will have.</param>
        /// <param name="offset">Offset of the new memory in the address space.</param>
        /// <param name="length">Length of each bank of memory.</param>
        public HM62256(byte banks, int offset, int length)
        {
            Memory = new byte[banks][];
            for (int i = 0; i < banks; i++)
            {
                Memory[i] = new byte[length + 1];
            }
            Length = length;
            Banks = banks;
            Offset = offset;
            CurrentBank = 0;
        }

        /// <summary>
        /// Called whenever the emulated computer is reset.
        /// </summary>
        public void Reset()
        {
            Clear();
        }

        /// <summary>
        /// Clears the memory.
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < Banks; i++)
            {
                for (var j = 0; j < Memory.Length; j++)
                {
                    Memory[i][j] = 0x00;
                }
            }
        }
        
        /// <summary>
        /// Returns the byte at a given address without incrementing the cycle. Useful for test harness. 
        /// </summary>
        /// <param name="bank">The bank to read data from.</param>
        /// <param name="address"></param>
        /// <returns>The byte being read.</returns>
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
            Memory[CurrentBank][address - Offset] = data;
        }

        /// <summary>
        /// Dumps the entire memory object. Used when saving the memory state
        /// </summary>
        /// <returns>Jagged array representing the banked memory.</returns>
        public byte[][] DumpMemory()
        {
            return Memory;
        }
    }
}

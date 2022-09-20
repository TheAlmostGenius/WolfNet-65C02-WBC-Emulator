using System;

namespace Hardware
{
    [Serializable]
    public class HM62256
    {
        public byte[][] Memory { get; set; }

        public int Offset { get; set; }

        public int Length { get; set; }

        public int End { get { return Offset + Length; } }

        public byte Banks { get; set; }

        public byte CurrentBank { get; set; }

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

        public void Reset()
        {
            Clear();
        }

        /// <summary>
        /// Clears the memory
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
            Memory[CurrentBank][address - Offset] = data;
        }

        /// <summary>
        /// Dumps the entire memory object. Used when saving the memory state
        /// </summary>
        /// <returns></returns>
        public byte[][] DumpMemory()
        {
            return Memory;
        }
    }
}

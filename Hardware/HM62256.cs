using System;

namespace Hardware
{
    public class HM62256
    {
        public byte[] Memory { get; set; }

        private W65C02 Processor { get; set; }

        public uint Length { get; set; }

        public HM62256(uint size, W65C02 processor)
        {
            Memory = new byte[size];
            Length = size;
            Processor = processor;
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
            for (var i = 0; i < Memory.Length; i++)
                Memory[i] = 0x00;
        }
        
        /// <summary>
        /// Loads a program into the processors memory
        /// </summary>
        /// <param name="offset">The offset in memory when loading the program.</param>
        /// <param name="program">The program to be loaded</param>
        public void Load(int offset, byte[] program)
        {
            if (offset > Memory.Length)
                throw new InvalidOperationException("Offset '{0}' is larger than memory size '{1}'");

            if (program.Length > Memory.Length + offset)
                throw new InvalidOperationException(string.Format("Program Size '{0}' Cannot be Larger than Memory Size '{1}' plus offset '{2}'", program.Length, Memory.Length, offset));

            for (var i = 0; i < program.Length; i++)
            {
                Memory[i + offset] = program[i];
            }

            Processor.Reset();
        }

        /// <summary>
        /// Loads a program into the processors memory
        /// </summary>
        /// <param name="offset">The offset in memory when loading the program.</param>
        /// <param name="program">The program to be loaded</param>
        /// <param name="initialProgramCounter">The initial PC value, this is the entry point of the program</param>
        public void Load(int offset, byte[] program, int initialProgramCounter)
        {
            Load(offset, program);

            var bytes = BitConverter.GetBytes(initialProgramCounter);

            //Write the initialProgram Counter to the reset vector
            MemoryMap.WriteWithoutCycle(0xFFFC, bytes[0]);
            MemoryMap.WriteWithoutCycle(0xFFFD, bytes[1]);

            //Reset the CPU
            Processor.Reset();
        }

        /// <summary>
        /// Dumps the entire memory object. Used when saving the memory state
        /// </summary>
        /// <returns></returns>
        public byte[] DumpMemory()
        {
            return Memory;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Emulator.Model
{
	/// <summary>
	/// Model that contains the required information needed to save the current state of the processor to disk
	/// </summary>
	[Serializable]
	public class StateFileModel
	{
		/// <summary>
		/// The Number of Cycles the Program has Ran so Far
		/// </summary>
		public int NumberOfCycles { get; set; }

		/// <summary>
		/// The output of the program
		/// </summary>
		public IList<OutputLog> OutputLog { get; set; }

        /// <summary>
        /// The Processor Object that is being saved
        /// </summary>
        public Hardware.W65C02 W65C02 { get; set; }

        /// <summary>
        /// The first VIA Object that is being saved
        /// </summary>
        public Hardware.W65C22 W65C22 { get; set; }

        /// <summary>
        /// The second VIA Object that is being saved
        /// </summary>
        public Hardware.W65C22 MM65SIB { get; set; }

        /// <summary>
        /// The ACIA Object that is being saved
        /// </summary>
        public Hardware.W65C51 W65C51 { get; set; }

        /// <summary>
        /// The Shared ROM Object that is being saved
        /// </summary>
        public Hardware.AT28CXX AT28C010 { get; set; }

        /// <summary>
        /// The Banked ROM Object that is being saved
        /// </summary>
        public Hardware.AT28CXX AT28C64 { get; set; }
    }
}

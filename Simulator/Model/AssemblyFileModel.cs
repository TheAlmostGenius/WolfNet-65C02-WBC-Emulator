namespace Simulator.Model
{
	/// <summary>
	/// The Model used when Loading a Program
	/// </summary>
	public class AssemblyFileModel
    {
        /// <summary>
        /// The Program Converted into Hex
        /// </summary>
        public byte[] Bios { get; set; }

        /// <summary>
        /// The Program Converted into Hex
        /// </summary>
        public byte[] Rom { get; set; }

        /// <summary>
        /// The path of the Program that was loaded
        /// </summary>
        public string BiosFilePath { get; set; }


        /// <summary>
        /// The path of the Program that was loaded
        /// </summary>
        public string RomFilePath { get; set; }


        /// <summary>
        /// The COM port that is used for output
        /// </summary>
        public string ComPort { get; set; }
    }
}

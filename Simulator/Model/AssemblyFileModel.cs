namespace Simulator.Model
{
	/// <summary>
	/// The Model used when Loading a Program
	/// </summary>
	public class AssemblyFileModel
	{
		/// <summary>
		/// The BIOS Converted into Hex
		/// </summary>
		public byte[] Bios { get; set; }

		/// <summary>
		/// The Banked ROM Converted into Hex
		/// </summary>
		public byte[] ProgramRom { get; set; }

		/// <summary>
		/// The Programs Listing
		/// </summary>
		public string Listing { get; set; }

		/// <summary>
		/// The offset in memory to use when loading the program
		/// </summary>
		public int MemoryOffset { get; set; }

		/// <summary>
		/// The initial Vaulue of the Program Counter
		/// </summary>
		public int InitialProgramCounter { get; set; }

		/// <summary>
		/// The path of the BIOS that was loaded
		/// </summary>
		public string BiosPath { get; set; }

		/// <summary>
		/// The path of the Banked ROM that was loaded
		/// </summary>
		public string RomPath { get; set; }
	}
}

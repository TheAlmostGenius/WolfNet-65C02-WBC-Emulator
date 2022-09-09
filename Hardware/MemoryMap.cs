namespace Hardware
{
	public class MemoryMap
	{
		public class DeviceArea
		{
			private static ushort _Offset = 0xD000;
			private static ushort _Length = 0x00FF;
			public static ushort Offset => _Offset;
			public static ushort Length => _Length;
		}

		public static readonly ushort BankedRom = 0x8000;
        public static readonly ushort SharedRom = 0xE000;
	}
}
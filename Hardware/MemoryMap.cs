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

		public class BankedRom
		{
			private static ushort _Offset = 0x8000;
            private static ushort _Length = 0x3FFF;

            public static int TotalLength = (BankSize * TotalBanks);
            public static byte TotalBanks = 16;
            public static int BankSize = Length + 1;
            public static ushort Offset => _Offset;
            public static ushort Length => _Length;
        }
		public class SharedRom
		{
			private static ushort _Offset = 0xE000;
            private static ushort _Length = 0x1FFF;

            public static ushort Offset => _Offset;
            public static ushort Length => _Length;
        }
	}
}
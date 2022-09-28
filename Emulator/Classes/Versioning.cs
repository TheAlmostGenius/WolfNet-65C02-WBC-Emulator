namespace Emulator
{
    public static class Versioning
    {
        public class Product
        {
            public const int Major = 0;
            public const int Minor = 1;
            public const int Build = 3;
            public const int Revision = 1;
            public const string Title = Name;
            public const string Name = "WolfNet 65C02 WorkBench Computer Emulator";
            public const string Company = "WolfNet Computing";
            public const string Copyright = "Copyright © WolfNet Computing 2022";
            public const string VersionString = "0.2.4.1";
            public const string Description = "Emulator for the WolfNet 65C02 WorkBench Computer coded in C# using the .NET Framework";
        }
        public class SettingsFile
        {
            public const byte Major = 1;
            public const byte Minor = 0;
            public const byte Build = 0;
            public const byte Revision = 0;

        }
    }
}

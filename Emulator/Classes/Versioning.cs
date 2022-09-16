using System.Deployment;
using System.Reflection;
using System;

namespace Emulator
{
    public class Versioning
    {
        public class Product
        {
            public const string Title = Name;
            public const string Name = "WolfNet 65C02 WorkBench Computer Emulator";
            public const string Company = "WolfNet Computing";
            public const string Copyright = "Copyright © WolfNet Computing 2022";
            public const string Version = "0.0.2.0";
            public const string Description = "Emulator for the WolfNet 65C02 WorkBench Computer coded in C# using the .NET Framework";
        }
        public const string SettingsFile = "1.0.0.0";
    }
}

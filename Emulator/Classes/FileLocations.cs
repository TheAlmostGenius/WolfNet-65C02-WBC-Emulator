namespace Emulator
{
    internal class FileLocations
    {
        #region Fields
        public static string SettingsFile = "./Settings.xml";
        public static string ErrorFile = "./Errors.log";
#if DEBUG
            public static string BiosFile = "../../../bios.bin";
#else
        public static string BiosFile = "./bios.bin";
#endif
        #endregion
    }
}

using Emulator.Model;

namespace Emulator
{
    public static class SettingsFile
    {
        public static SettingsModel CreateNew()
        {
            // Create new settings file.
            SettingsModel _settings = new SettingsModel
            {
                SettingsVersionMajor = Versioning.SettingsFile.Major,
                SettingsVersionMinor = Versioning.SettingsFile.Minor,
                SettingsVersionBuild = Versioning.SettingsFile.Build,
                SettingsVersionRevision = Versioning.SettingsFile.Revision,
#if DEBUG
                ComPortName = "COM9",
#else
                ComPortName = "COM1",
#endif
            };
            return _settings;
        }
    }
}

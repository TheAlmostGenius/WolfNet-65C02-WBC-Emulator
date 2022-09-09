using System;
using System.IO.Ports;
using System.Xml.Serialization;

namespace Simulator.Model
{
	/// <summary>
	/// Model that contains the required information needed to save the current settings to disk
	/// </summary>
    [Serializable]
    [XmlRootAttribute("SettingsFileModel", Namespace="Simulator.Model", IsNullable = false)]
	public class SettingsModel
    {
        /// <summary>
        /// The version of the file that is being saved
        /// </summary>
        public string SettingsVersion { get; set; }

        /// <summary>
        /// The PC port that is being saved
        /// </summary>
        public string ComPort { get; set; }
    }
}

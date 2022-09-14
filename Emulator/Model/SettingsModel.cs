﻿using System;
using System.Xml.Serialization;

namespace Emulator.Model
{
	/// <summary>
	/// Model that contains the required information needed to save the current settings to disk
	/// </summary>
    [Serializable]
    [XmlRootAttribute("SettingsFileModel", Namespace="Emulator.Model", IsNullable = false)]
	public class SettingsModel
    {
        /// <summary>
        /// The version of the file that is being saved
        /// </summary>
        public string SettingsVersion { get; set; }

        /// <summary>
        /// The PC port that is being saved
        /// </summary>
        public string ComPortName { get; set; }
    }
}

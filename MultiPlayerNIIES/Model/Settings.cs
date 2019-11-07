using MultiPlayerNIIES.Tools.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.Model
{

    public static class Settings
    {
        public static string stateFilesRestorePathType = "Absolute";

        static public void SaveAllSettings()
        {
            ConfigurationTools.AddUpdateAppSettings("StateFilesRestorePathType", stateFilesRestorePathType);
        }

        static public void RestoreAllSettings()
        {
            stateFilesRestorePathType = ConfigurationTools.ReadSetting("StateFilesRestorePathType");
        }
    }
}
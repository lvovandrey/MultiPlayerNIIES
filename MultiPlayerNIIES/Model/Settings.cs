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
        public static string showFullNameInPlayerHeader = "True";
        public static bool ShowFullNameInPlayerHeader
        {
            get
            {
                if (showFullNameInPlayerHeader == "True")
                    return true;
                else return false;
            }
        }

        static public void SaveAllSettings()
        {
            ConfigurationTools.AddUpdateAppSettings("StateFilesRestorePathType", stateFilesRestorePathType);
            ConfigurationTools.AddUpdateAppSettings("ShowFullNameInPlayerHeader", showFullNameInPlayerHeader);
        }

        static public void RestoreAllSettings()
        {
            stateFilesRestorePathType = ConfigurationTools.ReadSetting("StateFilesRestorePathType");
            showFullNameInPlayerHeader = ConfigurationTools.ReadSetting("ShowFullNameInPlayerHeader");
        }
    }
}
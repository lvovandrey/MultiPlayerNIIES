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
        /// <summary>
        /// Событие возникает когда изменяются настройки. На него рекомендуется вешать все изменения которые завязаны с настройками.
        /// </summary>
        public static event Action SettingsChanged;

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
            if (SettingsChanged != null) SettingsChanged();
        }

        static public void RestoreAllSettings()
        {
            stateFilesRestorePathType = ConfigurationTools.ReadSetting("StateFilesRestorePathType");
            showFullNameInPlayerHeader = ConfigurationTools.ReadSetting("ShowFullNameInPlayerHeader");

            if(SettingsChanged!=null) SettingsChanged();
        }

       
    }
}
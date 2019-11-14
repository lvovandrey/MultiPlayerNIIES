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


        public static string slowRate = "0,5";
        public static double SlowRate
        {
            get
            {
                double s = 0.5;
                if (double.TryParse(step, out s)) return s;
                else return 0.5;
            }
        }

        public static string fastRate = "2";
        public static double FastRate
        {
            get
            {
                double s = 2;
                if (double.TryParse(step, out s)) return s;
                else return 2;
            }
        }

        public static string rateShift = "0,1";
        public static double RateShift
        {
            get
            {
                double s = 0.1;
                if (double.TryParse(step, out s)) return s;
                else return 0.1;
            }
        }

        public static string step = "100";
        public static double Step
        {
            get
            {
                double s = 0;
                if (double.TryParse(step, out s)) return s;
                else return 100;
            }
        }

        static public void SaveAllSettings()
        {
            ConfigurationTools.AddUpdateAppSettings("StateFilesRestorePathType", stateFilesRestorePathType);
            ConfigurationTools.AddUpdateAppSettings("ShowFullNameInPlayerHeader", showFullNameInPlayerHeader);

            ConfigurationTools.AddUpdateAppSettings("SlowRate", slowRate);
            ConfigurationTools.AddUpdateAppSettings("FastRate", fastRate);
            ConfigurationTools.AddUpdateAppSettings("RateShift", rateShift);
            ConfigurationTools.AddUpdateAppSettings("Step", step);

            if (SettingsChanged != null) SettingsChanged();
        }

        static public void RestoreAllSettings()
        {
            stateFilesRestorePathType = ConfigurationTools.ReadSetting("StateFilesRestorePathType");
            showFullNameInPlayerHeader = ConfigurationTools.ReadSetting("ShowFullNameInPlayerHeader");

            slowRate = ConfigurationTools.ReadSetting("SlowRate");
            fastRate = ConfigurationTools.ReadSetting("FastRate");
            rateShift = ConfigurationTools.ReadSetting("RateShift");
            step = ConfigurationTools.ReadSetting("Step");

            if(SettingsChanged!=null) SettingsChanged();
        }

       
    }
}
using MultiPlayerNIIES.Tools.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                if (double.TryParse(slowRate, out s)) return s;
                else return 0.5;
            }
            set
            {
                if (value <= 0 || value > 100)
                {
                    MessageBox.Show("Скорость воспроизведения должна быть положительным числом от 0.001 до 100", "Ошибка ввода скорости воспроизведения", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                slowRate = value.ToString();
            }
        }

        public static string fastRate = "2";
        public static double FastRate
        {
            get
            {
                double s = 2;
                if (double.TryParse(fastRate, out s)) return s;
                else return 2;
            }
            set
            {
                if (value <= 0 || value > 100)
                {
                    MessageBox.Show("Скорость воспроизведения должна быть положительным числом от 0.001 до 100", "Ошибка ввода скорости воспроизведения", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                fastRate = value.ToString();
            }
        }

        public static string rateShift = "0,1";
        public static double RateShift
        {
            get
            {
                double s = 0.1;
                if (double.TryParse(rateShift, out s)) return s;
                else return 0.1;
            }
            set
            {
                if (value <= 0 || value > 100)
                {
                    MessageBox.Show("Шаг скорости воспроизведения должна быть положительным числом от 0.001 до 100", "Ошибка ввода шага скорости воспроизведения", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                rateShift = value.ToString();
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
            set
            {
                if (value <= 1 || value > 3600000)
                {
                    MessageBox.Show("Шаг перемещения задается в милисекундах от 1 (1 мсек) до 3 600 000 (1 час)", "Ошибка шага перемещения", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                step = value.ToString();
            }
        }

        public static string defaultVolume = "50";
        public static double DefaultVolume
        {
            get
            {
                double s = 50;
                if (double.TryParse(defaultVolume, out s)) return s;
                else return 50;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    MessageBox.Show("Громкость по умолчанию - параметр задается в пределах от 0 до 100", "Ошибка ввода громкости по умолчанию", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                defaultVolume = value.ToString();
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
            ConfigurationTools.AddUpdateAppSettings("DefaultVolume", defaultVolume);

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
            defaultVolume = ConfigurationTools.ReadSetting("DefaultVolume");

            if (SettingsChanged!=null) SettingsChanged();
        }

       
    }
}
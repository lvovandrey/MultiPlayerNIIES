using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.Model;
using MultiPlayerNIIES.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel
{
    public class SettingsWindowVM : INPCBase
    {
        SettingsWindowView settingsWindowView;
        #region CONSTRUCTOR
        public SettingsWindowVM(SettingsWindowView _settingsWindowView)
        {
            settingsWindowView = _settingsWindowView;
            RestoreSettingsCommand.Execute(null);
            settingsWindowView.DataContext = this;

            settingsWindowView.Loaded += SettingsWindowView_Loaded;
            settingsWindowView.Activated += SettingsWindowView_Activated;

        }
        #endregion

        #region PROPERTIES
        public bool IsStateFilesRestorePathTypeAbsolute
        {
            get
            {
                if (Settings.stateFilesRestorePathType == "Absolute")
                    return true;
                else return false;
            }
            set
            {
                if (value == true) Settings.stateFilesRestorePathType = "Absolute";
                else Settings.stateFilesRestorePathType = "Relative";
                OnPropertyChanged("IsStateFilesRestorePathTypeAbsolute");
                OnPropertyChanged("IsStateFilesRestorePathTypeRelative");
            }
        }
        public bool IsStateFilesRestorePathTypeRelative
        {
            get { return !IsStateFilesRestorePathTypeAbsolute; }
            set { IsStateFilesRestorePathTypeAbsolute = !value; }
        }

        public bool ShowFullNameInPlayerHeader
        {
            get
            {
                if (Settings.showFullNameInPlayerHeader == "True")
                    return true;
                else return false;
            }
            set
            {
                if (value == true) Settings.showFullNameInPlayerHeader = "True";
                else Settings.showFullNameInPlayerHeader = "False";
                OnPropertyChanged("ShowFullNameInPlayerHeader");
            }
        }

        public double SlowRate
        {
            get { return Settings.SlowRate; }
            set { Settings.SlowRate = value; OnPropertyChanged("SlowRate"); }
        }

        public  double FastRate
        {
            get { return Settings.FastRate; }
            set { Settings.FastRate = value; OnPropertyChanged("FastRate"); }
        }

        public  double RateShift
        {
            get { return Settings.RateShift; }
            set { Settings.RateShift = value; OnPropertyChanged("RateShift"); }
        }

        public  double Step
        {
            get { return Settings.Step; }
            set { Settings.Step = value; OnPropertyChanged("Step"); }
        }

        public double DefaultVolume
        {
            get { return Settings.DefaultVolume; }
            set { Settings.DefaultVolume = value; OnPropertyChanged("DefaultVolume"); }
        }
        #endregion

        #region METHODS
        private void SettingsWindowView_Activated(object sender, EventArgs e)
        {
 //           RestoreSettingsCommand.Execute(null);
        }

        private void SettingsWindowView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RestoreSettingsCommand.Execute(null);
        }
        #endregion

        #region COMMANDS
        private RelayCommand saveSettingsCommand;
        public RelayCommand SaveSettingsCommand
        {
            get
            {
                return saveSettingsCommand ??
                  (saveSettingsCommand = new RelayCommand(obj =>
                  {
                      Settings.SaveAllSettings();
                  }));
            }
        }

        private RelayCommand restoreSettingsCommand;
        public RelayCommand RestoreSettingsCommand
        {
            get
            {
                return restoreSettingsCommand ??
                  (restoreSettingsCommand = new RelayCommand(obj =>
                  {
                      Settings.RestoreAllSettings();
                      OnPropertyChanged("IsStateFilesRestorePathTypeAbsolute");
                      OnPropertyChanged("IsStateFilesRestorePathTypeRelative");
                      OnPropertyChanged("ShowFullNameInPlayerHeader");
                      OnPropertyChanged("SlowRate");
                      OnPropertyChanged("FastRate");
                      OnPropertyChanged("RateShift");
                      OnPropertyChanged("Step");
                      OnPropertyChanged("DefaultVolume");
                  }));
            }
        }
        #endregion

    }

}


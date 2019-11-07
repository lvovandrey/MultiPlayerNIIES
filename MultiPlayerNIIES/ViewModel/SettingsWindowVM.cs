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
            }
        }
        public bool IsStateFilesRestorePathTypeRelative
        {
            get { return !IsStateFilesRestorePathTypeAbsolute; }
            set { IsStateFilesRestorePathTypeAbsolute = !value; }
        }


        public SettingsWindowVM(SettingsWindowView _settingsWindowView)
        {
            settingsWindowView = _settingsWindowView;
            RestoreSettingsCommand.Execute(null);
            settingsWindowView.DataContext = this;

        }



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
                  }));
            }
        }

    }

}


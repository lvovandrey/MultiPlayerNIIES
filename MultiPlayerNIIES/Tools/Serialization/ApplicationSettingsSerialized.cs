﻿using MultiPlayerNIIES.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Serialization;

namespace MultiPlayerNIIES.Tools.Serialization
{
    [Serializable]
    public class ApplicationSettingsSerialized
    {
        public Rect Position;
        public List<PlayerStateSerialized> Players;
        public bool IsExcelConnected;
        public string ExcelBookFilename;

        public ApplicationSettingsSerialized()
        {}

        public ApplicationSettingsSerialized(VM vm)
        {
            Position = new Rect(vm.MainWindow.Left, vm.MainWindow.Top, vm.MainWindow.ActualWidth, vm.MainWindow.ActualHeight);
            IsExcelConnected = vm.IsExcelConnected;
            if (IsExcelConnected) ExcelBookFilename = vm.ExcelBookFilename;
            Players = new List<PlayerStateSerialized>();
            foreach (var v in vm.videoPlayerVMs)
            {
                Players.Add(new PlayerStateSerialized()
                {
                    filename=v.SourceFilename,
                    CurTime = v.CurTime, 
                    TimeShift=v.SyncronizationShiftVM.ShiftTime,
                    IsSyncLeader = v.IsSyncronizeLeader,
                    Position = new Rect(v.Body.Margin.Left,v.Body.Margin.Top,v.Body.ActualWidth,v.Body.ActualHeight),
                    Duration = v.Duration
                });
            }    
        }
    }

    [Serializable]
    public class PlayerStateSerialized
    {
        public string filename;
        [XmlIgnore]
        public TimeSpan CurTime;
        [XmlIgnore]
        public TimeSpan TimeShift;
        [XmlIgnore]
        public TimeSpan Duration;

        public long sCurTime
        {
            get { return CurTime.Ticks; }
            set { CurTime = new TimeSpan(value);}
        }
        public long sTimeShift
        {
            get { return TimeShift.Ticks; }
            set { TimeShift = new TimeSpan(value); }
        }
        public long sDuration
        {
            get { return Duration.Ticks; }
            set { Duration = new TimeSpan(value); }
        }

        public bool IsSyncLeader;
        public Rect Position;

        public PlayerStateSerialized(){}

    }
    

}
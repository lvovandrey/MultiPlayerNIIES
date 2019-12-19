using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.Model;
using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.View.TimeDiffElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel.TimeDiffVM
{
    class TimeDiffWindowVM : INPCBase
    {

        #region Поля 
        List<TimeDiffColumnVM> ColumnVMs;
        List<TimeDiffVideoInfoRectVM> VideoInfoRectVMs;
        TimeDIffWindowWindow Body;
        public readonly VM VM;
        #endregion

        #region Конструкторы
        public TimeDiffWindowVM(TimeDIffWindowWindow body, VM vm)
        {
            ColumnVMs = new List<TimeDiffColumnVM>();
            VideoInfoRectVMs = new List<TimeDiffVideoInfoRectVM>();
            Body = body;
            VM = vm; 
        }
        #endregion

        #region Методы
        public void AddColumns()
        {
            Body.ClearColumns();
            foreach (var p in TimeDiffMeasuringManager.TimeDiffPositions)
            {
                TimeDiffColumnVM columnVM = new TimeDiffColumnVM(p.Number);
                ColumnVMs.Add(columnVM);
                Body.AddColumn(columnVM);
            }
        }

        public void AddVideos()
        {
            Body.ClearVideoInfoRects();
            int i = 1;
            foreach (var p in TimeDiffMeasuringManager.TimeDiffVideos)
            {
                TimeDiffVideoInfoRectVM videoVM = new TimeDiffVideoInfoRectVM(TimeDiffMeasuringManager.TimeDiffVideos.IndexOf(p), p.VideoPlayerVM);
                VideoInfoRectVMs.Add(videoVM);
                Body.AddVideo(videoVM);
                i++;
            }
            ToolsTimer.Delay(() => { Body.FormatWindowContent(); }, TimeSpan.FromSeconds(0.1));

        }


        #endregion

        #region Свойства


        #endregion

        #region Комманды

        private RelayCommand showCommand;
        

        public RelayCommand ShowCommand
        {
            get
            {
                return showCommand ?? (showCommand = new RelayCommand(obj =>
                {
                    AddColumns();
                    AddVideos();
                    
                }));
            }
        }





        #endregion

    }
}

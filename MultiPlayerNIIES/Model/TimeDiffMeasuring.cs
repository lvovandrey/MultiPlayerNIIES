using MultiPlayerNIIES.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.Model
{
    /// <summary>
    /// Класс содержит в себе всю информацию обо всех измерениях разниц во времени и состояние измерения
    /// </summary>
    public static class TimeDiffMeasuringManager
    {
        public static int CurrentMeasurement = 0;
        public static int MeasurementsCount = 2;
        public static TimeSpan TimeSyncLead = TimeSpan.Zero;
        public static List<TimeDiffPosition> TimeDiffPositions;
        public static List<TimeDiffVideo> TimeDiffVideos;

        public static void StartNewMeasuring(List<VideoPlayerVM> videoPlayersVMs)
        {
            if(TimeDiffPositions!=null) TimeDiffPositions.Clear();
            if(TimeDiffVideos!=null) TimeDiffVideos.Clear();

            TimeDiffMeasuringManager.MeasurementsCount = videoPlayersVMs.Count;
            TimeDiffMeasuringManager.CurrentMeasurement = 0;
            TimeDiffPositions = new List<TimeDiffPosition>();
            TimeDiffVideos = new List<TimeDiffVideo>();

            foreach (var v in videoPlayersVMs)
            {
                TimeDiffVideos.Add(new TimeDiffVideo(v.IsSyncronizeLeader, v.FilenameForTitle, null, v));
            }
        }
        public static void AddMeasurement(TimeSpan CurTime, List<System.Drawing.Bitmap> SnapShoots)
        {
            CurrentMeasurement++;
            TimeDiffPosition diffPosition = new TimeDiffPosition(CurrentMeasurement, CurTime);
            TimeDiffPositions.Add(diffPosition);
            TimeDiffVideos[CurrentMeasurement - 1].CurrentPosition = diffPosition;

            foreach (var v in TimeDiffVideos)
            {
                v.SnapShotsOnPositions.Add(CurrentMeasurement-1, SnapShoots[TimeDiffVideos.IndexOf(v)]);
            }
        }

    }

    /// <summary>
    /// Позиция при измерении времени - временная отметка
    /// </summary>
    public class TimeDiffPosition
    {
        public int Number;
        public TimeSpan Time;
        public TimeDiffPosition(int number, TimeSpan time)
        {
            Number = number;
            Time = time;
        }
    }

    /// <summary>
    /// Разная информация о видео для измерения разницы во времени
    /// </summary>
    public class TimeDiffVideo
    {

        public string FileName;
        public TimeDiffPosition CurrentPosition;
        public bool IsSyncLead;
        public Dictionary<int, System.Drawing.Bitmap> SnapShotsOnPositions;
        internal VideoPlayerVM VideoPlayerVM;//TODO: Может как-то это отсюда можно убрать?

        public System.Drawing.Bitmap CurrentSnapShot
        {
            get { return SnapShotsOnPositions[CurrentPosition.Number]; }
        }

        public TimeDiffVideo(bool isSyncLead, string fileName, TimeDiffPosition position, VideoPlayerVM videoPlayerVM)
        {
            FileName = fileName;
            IsSyncLead = isSyncLead;
            CurrentPosition = position;
            SnapShotsOnPositions = new Dictionary<int, System.Drawing.Bitmap>();
            VideoPlayerVM = videoPlayerVM;
        }

    }
}

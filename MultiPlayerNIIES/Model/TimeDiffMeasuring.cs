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
        public static List<TimeDiffPosition> TimeDiffPositions;
        public static List<TimeDiffVideo> TimeDiffVideos;

        public static void StartNewMeasuring(int MeasurementsCount, List<VideoPlayerVM> videoPlayersVMs)
        {
            if(TimeDiffPositions!=null) TimeDiffPositions.Clear();
            if(TimeDiffVideos!=null) TimeDiffVideos.Clear();

            TimeDiffMeasuringManager.MeasurementsCount = MeasurementsCount;
            TimeDiffMeasuringManager.CurrentMeasurement = 0;
            TimeDiffPositions = new List<TimeDiffPosition>();

            for (int i = 0; i <= MeasurementsCount; i++)
            {
                TimeDiffPositions.Add(new TimeDiffPosition(i));
            } 

            foreach (var v in videoPlayersVMs)
            {
                TimeDiffVideos.Add(new TimeDiffVideo(v.IsSyncronizeLeader, v.FilenameForTitle, TimeDiffPositions[0]));
            }


        }
        public static void AddMeasurement()
        {

        }

    }

    /// <summary>
    /// Позиция при измерении времени - временная отметка
    /// </summary>
    public class TimeDiffPosition
    {
        public int Number;
        public TimeSpan TimeDifference;
        public TimeDiffPosition(int number)
        {
            Number = number;
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
        public System.Drawing.Bitmap CurrentSnapShot
        {
            get { return SnapShotsOnPositions[CurrentPosition.Number]; }
        }

        public TimeDiffVideo(bool isSyncLead, string fileName, TimeDiffPosition position)
        {
            FileName = fileName;
            IsSyncLead = isSyncLead;
            CurrentPosition = position;
            SnapShotsOnPositions = new Dictionary<int, System.Drawing.Bitmap>();
        }

    }
}

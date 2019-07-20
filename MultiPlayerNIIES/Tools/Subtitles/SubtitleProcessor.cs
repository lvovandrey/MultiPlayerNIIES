using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiPlayerNIIES.Tools.Subtitles
{
    /// <summary>
    /// Основной внешний класс для работы с титрами.
    /// </summary>
    public class SubtitleProcessor
    {
        public bool Ready = false;
        List<Subtitle> subtitles;
        List<string> RawSubtitles;
        List<Record> records;

        public List<Record> Records
        {
            get { return records; }
        }
        public List<Subtitle> Subtitles
        {
            get { return subtitles; }
        }

        public SubtitleProcessor()
        {
            subtitles = new List<Subtitle>();
            RawSubtitles = new List<string>();
            records = new List<Record>(); 
        }
        public void LoadSubtitles(string filename)
        {
            try
            {
                RawSubtitles.Clear();
                using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        RawSubtitles.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка считывания файлов титров: " + e.Message);
            }
            Parser.ParseRawTitles(RawSubtitles, out subtitles);
            RawSubtitles = null;
            Parser.SegregateSubtitlesToRecords(subtitles, out records);

            Ready = true;
        }


        /////////// <summary>
        /////////// Ищет номер записи субтитра по времени видео
        /////////// </summary>
        /////////// <param name="timeVideo">Время по видео</param>
        /////////// <returns>Номер соответствующего титра в коллекции Subtitles</returns>




        /// <summary>
        /// Функция возвращает титр соответствующий данному моменту на видео
        /// </summary>
        /// <param name="TimeVideo">Время на видео</param>
        /// <returns></returns>
        public Subtitle GetSubtitle(TimeSpan TimeVideo)
        {
            return subtitles[SearchAndTools.BinarySearch(TimeVideo, subtitles)];
        }

        /// <summary>
        /// Функция возвращает титр через поиск его по времени, указанному в титрах
        /// </summary>
        /// <param name="TimeTitles">Время, которое должно быть указано в тексте титров</param>
        /// <returns></returns>
        public Subtitle GetSubtitleOfTitlesTimeText(TimeSpan TimeTitles)
        {
            return subtitles[SearchAndTools.BinarySearchInTimesFromTitles(TimeTitles, subtitles,0, subtitles.FindLastIndex((x)=> {return x.Begin > TimeSpan.FromSeconds(-1); } ))];
        }

        /// <summary>
        /// Возвращает время синхронизации видео по времени субтитров и номеру интервала субтитров
        /// </summary>
        /// <param name="syncTitlesTime">Время субтитров</param>
        /// <param name="RecordNumber">номер интервала субтитров</param>
        /// <returns></returns>
        internal TimeSpan GetSyncTime(TimeSpan syncTitlesTime, int recordNumber)
        {
            return Subtitles[SearchAndTools.BinarySearchInTimesFromTitles(syncTitlesTime, Subtitles, Records[recordNumber])].Begin;
        }
    }

    




}

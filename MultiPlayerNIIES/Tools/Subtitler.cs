using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiPlayerNIIES.Tools
{

    public class Subtitler
    {
        public bool Ready = false;
        public List<Subtitle> Subtitles;
        List<string> RawSubtitles;
        List<SubtitlesRecord> SubtitlesRecords;

        public Subtitler()
        {
            Subtitles = new List<Subtitle>();
            RawSubtitles = new List<string>();
            SubtitlesRecords = new List<SubtitlesRecord>(); 
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
            SubtitlesParser.ParseRawTitles(RawSubtitles, out Subtitles);
            RawSubtitles = null;
            SubtitlesParser.SegregateSubtitlesToRecords(Subtitles, out SubtitlesRecords);

            Ready = true;
        }

        public int BinarySearch(TimeSpan time)
        {
            int left = 0;
            int right = Subtitles.Count - 1;
            if (left == right)
                return left;
            while (true)
            {
                if (right - left == 1)
                {
                    if (Subtitles[left].Compare(time) == 0)
                        return left;
                    if (Subtitles[right].Compare(time) == 0)
                        return right;
                    return -1;
                }
                else
                {
                    int middle = left + (right - left) / 2;
                    int comparisonResult = Subtitles[middle].Compare(time);
                    if (comparisonResult == 0)
                        return middle;
                    if (comparisonResult < 0)
                        left = middle;
                    if (comparisonResult > 0)
                        right = middle;
                }
            }
        }


        //TODO: повторяющийся код в функциях BinarySearchInTimesFromTitles и BinarySearch - может и хрен с ним?
        public int BinarySearchInTimesFromTitles(TimeSpan time)
        {
            int left = 0;
            int right = Subtitles.Count - 1;
            if (left == right)
                return left;
            while (true)
            {
                if (right - left == 1)
                {
                    if (Subtitles[left].CompareWithTitleTime(time) == 0)
                        return left;
                    if (Subtitles[right].CompareWithTitleTime(time) == 0)
                        return right;
                    return -1;
                }
                else
                {
                    int middle = left + (right - left) / 2;
                    int comparisonResult = Subtitles[middle].CompareWithTitleTime(time);
                    if (comparisonResult == 0)
                        return middle;
                    if (comparisonResult < 0)
                        left = middle;
                    if (comparisonResult > 0)
                        right = middle;
                }
            }
        }



        int SearchSubRecord(TimeSpan time) { return 0; }
    }

    /// <summary>
    /// Отдельный субтитр
    /// </summary>
    public class Subtitle
    {
        public TimeSpan Begin;
        public TimeSpan End;
        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                TimeFromTextBegin = SetTimeFromText(text);
                TimeFromTextEnd = TimeFromTextBegin + (End - Begin);
            }
        }
        public TimeSpan TimeFromTextBegin;
        public TimeSpan TimeFromTextEnd;


        public bool IsIncluded(TimeSpan time)
        {
            if (time >= Begin && time < End)
            {
                return true;
            }
            return false;
        }

        public int Compare(TimeSpan time)
        {
            if (time >= Begin && time < End)
            {
                return 0;
            }
            else if (time < Begin)
            {
                return 1;
            }
            else if (time > End)
            {
                return -1;
            }
            return -1;
        }

        public int CompareWithTitleTime(TimeSpan time)
        {
            if (time >= TimeFromTextBegin && time < TimeFromTextEnd)
            {
                return 0;
            }
            else if (time < TimeFromTextBegin)
            {
                return 1;
            }
            else if (time > TimeFromTextEnd)
            {
                return -1;
            }
            return -1;
        }

        //TODO: позже сделать в SetTimeFromText закидывание делегата для парсинга 
        public static TimeSpan SetTimeFromText(string Text)
        {
            TimeSpan time;
            string Format = @"h\;mm\;ss\;fff";
            CultureInfo culture = CultureInfo.CurrentCulture;
            TimeSpan.TryParseExact(Text, Format, culture, TimeSpanStyles.None, out time);
            return time;
        }
    }

    /// <summary>
    /// Класс описывыет часть видео с непрерывными по времени субтитрами - в общем часть видео которая действительно непрерывна
    /// Сами субтитры тут не представлены чтобы не дублировать их
    /// </summary>
    public class SubtitlesRecord
    {
        public TimeSpan Begin;
        public TimeSpan End;

        public SubtitlesRecord(TimeSpan begin, TimeSpan end)
        {
            Begin = begin;
            End = end;
        }

        public bool IsIncluded(TimeSpan time)
        {
            if (time >= Begin && time < End)
            {
                return true;
            }
            return false;
        }
        public int Compare(TimeSpan time)
        {
            if (time >= Begin && time < End)
            {
                return 0;
            }
            else if (time < Begin)
            {
                return 1;
            }
            else if (time > End)
            {
                return -1;
            }
            return -1;
        }
    }


    public static class SubtitlesParser
    {
        private static bool GetTimeInterval(string s, out Subtitle titleInfo)
        {
            titleInfo = new Subtitle();
            int endOfFirstTime, beginOfSecondTime, delimiterIndex;
            delimiterIndex = s.IndexOf(" --> ");
            if (delimiterIndex < 5) { return false;}
            endOfFirstTime = delimiterIndex - 1;
            beginOfSecondTime = delimiterIndex+5;

            string FirstTimeStr, SecondTimeStr;
            FirstTimeStr = s.Substring(1,endOfFirstTime);
            SecondTimeStr = s.Substring(beginOfSecondTime);

            if (!TimeSpan.TryParse(FirstTimeStr,out titleInfo.Begin)) return false;
            if (!TimeSpan.TryParse(SecondTimeStr,out titleInfo.End)) return false;
            
            return true;
        }

        private static bool GetSubtitleText(string s, ref Subtitle titleInfo)
        {
            string tmpstr = s;
            TitleClearTags(ref tmpstr);
            titleInfo.Text = tmpstr.Trim();
            return true;
        }

        private static void TitleClearTags(ref string Text)
        {
            string tmpstr = Text;

            int antiInfinity = 0;
            while (tmpstr.Contains("{"))
            {
                if (++antiInfinity > 10000) break;

                int beg = tmpstr.IndexOf("{");
                int end = tmpstr.IndexOf("}");
                if (beg >= 0 && end >= 0 && beg < end)
                {
                    string taggedSubStr = tmpstr.Substring(beg, end - beg+1);
                    tmpstr = tmpstr.Replace(taggedSubStr, "");
                }
            }

            antiInfinity = 0;
            while (tmpstr.Contains("<"))
            {
                if (++antiInfinity > 10000) break;

                int beg = tmpstr.IndexOf("<");
                int end = tmpstr.IndexOf(">");
                if (beg >= 0 && end >= 0 && beg < end)
                {
                    string taggedSubStr = tmpstr.Substring(beg, end - beg + 1);
                    tmpstr = tmpstr.Replace(taggedSubStr, "");
                }
            }
            Text = tmpstr;
        }

        private static int FindNextTitleRecord(List<string> RawSubtitles, int curStringNumber)
        {
            int i = curStringNumber;
            while (RawSubtitles[i] != "")
            {
                i++;
                if (i >= RawSubtitles.Count)
                {
                    i = -1;
                    break;
                }
            }
            return i;
        }

        public static bool ParseRawTitles(List<string> RawSubtitles, out List<Subtitle> OuterSubtitles)
        {
            int curStrNumber = 1;
            int nextRecStrNumber = 0;
            string TimeStr, TextStr; 

            OuterSubtitles = new List<Subtitle>();

            while (curStrNumber < RawSubtitles.Count)
            {
                nextRecStrNumber = FindNextTitleRecord(RawSubtitles, curStrNumber);
                if (nextRecStrNumber < curStrNumber) break;
                if (nextRecStrNumber > RawSubtitles.Count - 3) break;

                TimeStr = RawSubtitles[nextRecStrNumber + 2];
                TextStr = RawSubtitles[nextRecStrNumber + 3];
                Subtitle titleInfo = new Subtitle();
                string text = "";
                GetTimeInterval(TimeStr, out titleInfo);
                GetSubtitleText(TextStr, ref titleInfo);
                OuterSubtitles.Add(titleInfo);
                curStrNumber = nextRecStrNumber+1;
            }
            return false;
        }

        /// <summary>
        /// Разбиваем весть интервал с титрами на куски соответствующие непрерывным видеозаписям 
        /// Приходится делать, когда в одном видео насчитывается несколько слепленных вместе видеозаписей и соответственно несколько кусочков титров.
        /// </summary>
        /// <param name="Subtitles">Весь интервал субтитров</param>
        /// <param name="OuterSubtitlesRecords">Коллекция отдельных непрерывных кусочков субтитров</param>
        /// <returns></returns>
        public static bool SegregateSubtitlesToRecords(List<Subtitle> Subtitles, out List<SubtitlesRecord> OuterSubtitlesRecords)
        {
            OuterSubtitlesRecords = new List<SubtitlesRecord>();
            if (Subtitles.Count < 2) return false;

            OuterSubtitlesRecords.Add(new SubtitlesRecord(Subtitles.First().Begin, Subtitles.Last().End)); //добавляем весь интервал с титрами, который представлен на видео (короче все видео)

            for (int i = 0; i < Subtitles.Count - 2; i++)
            {
                if (Subtitles[i].TimeFromTextBegin > Subtitles[i + 1].TimeFromTextBegin) //если найдена граница (разрыв) интеравала внутри текста субтитров типа 0:00-0:01-0:02-0:03-0:04-0:05-!ТУТ!-0:00-0:01-0:02....
                {
                    OuterSubtitlesRecords.Last().End = Subtitles[i].End;
                    OuterSubtitlesRecords.Add(new SubtitlesRecord(Subtitles[i+1].Begin, Subtitles.Last().End));
                }
            }

            return true;
        }

    }

}

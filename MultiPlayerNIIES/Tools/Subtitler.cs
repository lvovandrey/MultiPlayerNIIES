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
        public List<TitleInfo> Subtitles;
        List<string> RawSubtitles;
        public Subtitler()
        {
            Subtitles = new List<TitleInfo>();
            RawSubtitles = new List<string>();
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

            Ready = true;
        }

        public int BinarySearch(TimeSpan time)
        {
            int left = 0;
            int right = Subtitles.Count-1;
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
    }

    public class TitleInfo
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
                TimeFromText = SetTimeFromText(text);
                TimeFromTextEnd = TimeFromText + (End - Begin);
            }
        }
        public TimeSpan TimeFromText;
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
            else if (time < Begin )
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
            if (time >= TimeFromText && time < TimeFromTextEnd)
            {
                return 0;
            }
            else if (time < TimeFromText)
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
            //string tmpstr = Text;
            //tmpstr = tmpstr.Substring(0, 11);
            //tmpstr.Replace(";", ":");
            //string s1 =  tmpstr.Substring(0,tmpstr.LastIndexOf(":"));
            //string s2 = tmpstr.Substring(tmpstr.LastIndexOf(":")+1);
            //tmpstr = s1 + "." + s2;

            TimeSpan time;
            string Format = @"h\;mm\;ss\;fff";
            CultureInfo culture = CultureInfo.CurrentCulture;
            TimeSpan.TryParseExact(Text, Format, culture, TimeSpanStyles.None, out time);
            return time;
        }
    }

    public static class SubtitlesParser
    {
        public static bool GetTimeInterval(string s, out TitleInfo titleInfo)
        {
            titleInfo = new TitleInfo();
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

        public static bool GetSubtitleText(string s, ref TitleInfo titleInfo)
        {
            string tmpstr = s;
            TitleClearTags(ref tmpstr);
            titleInfo.Text = tmpstr.Trim();
            return true;
        }

        static void TitleClearTags(ref string Text)
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
       


        public static int FindNextTitleRecord(List<string> RawSubtitles, int curStringNumber)
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



        public static bool ParseRawTitles(List<string> RawSubtitles, out List<TitleInfo> OuterSubtitles)
        {
            int curStrNumber = 1;
            int nextRecStrNumber = 0;
            string TimeStr, TextStr; 

            OuterSubtitles = new List<TitleInfo>();

            while (curStrNumber < RawSubtitles.Count)
            {
                nextRecStrNumber = FindNextTitleRecord(RawSubtitles, curStrNumber);
                if (nextRecStrNumber < curStrNumber) break;
                if (nextRecStrNumber > RawSubtitles.Count - 3) break;

                TimeStr = RawSubtitles[nextRecStrNumber + 2];
                TextStr = RawSubtitles[nextRecStrNumber + 3];
                TitleInfo titleInfo = new TitleInfo();
                string text = "";
                GetTimeInterval(TimeStr, out titleInfo);
                GetSubtitleText(TextStr, ref titleInfo);
                OuterSubtitles.Add(titleInfo);
                curStrNumber = nextRecStrNumber+1;
            }
            return false;
        }

    }

}

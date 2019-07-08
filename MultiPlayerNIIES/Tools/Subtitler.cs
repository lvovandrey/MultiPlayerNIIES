using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiPlayerNIIES.Tools
{

    public class Subtitler
    {
        Dictionary<TimeInterval, string> Subtitles;
        List<string> RawSubtitles;
        public Subtitler()
        {
            Subtitles = new Dictionary<TimeInterval, string>();
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
        }
    }

    public class TimeInterval
    {
        public TimeSpan Begin;
        public TimeSpan End;

        public bool IsIncluded(TimeSpan time)
        {
            if (time >= Begin && time < End)
            {
                return true;
            }
            return false;
        }
    }

    public static class SubtitlesParser
    {
        public static bool GetTimeInterval(string s, out TimeInterval timeInterval)
        {
            timeInterval = new TimeInterval();
            int endOfFirstTime, beginOfSecondTime, delimiterIndex;
            delimiterIndex = s.IndexOf(" --> ");
            if (delimiterIndex < 5) { return false;}
            endOfFirstTime = delimiterIndex - 1;
            beginOfSecondTime = delimiterIndex+5;

            string FirstTimeStr, SecondTimeStr;
            FirstTimeStr = s.Substring(1,endOfFirstTime);
            SecondTimeStr = s.Substring(beginOfSecondTime);

            if (!TimeSpan.TryParse(FirstTimeStr,out timeInterval.Begin)) return false;
            if (!TimeSpan.TryParse(SecondTimeStr,out timeInterval.End)) return false;
            
            return true;
        }

        public static bool GetSubtitleText(string s, out string Text)
        {

            Text = s;
            TitleClearTags(ref Text);
            Text = Text.Trim();
            return true;
        }

        static void TitleClearTags(ref string Text)
        {
            string tmpstr= Text;

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



        public static bool ParseRawTitles(List<string> RawSubtitles, out Dictionary<TimeInterval, string> OuterSubtitles)
        {
            int curStrNumber = 1;
            int nextRecStrNumber = 0;
            string TimeStr, TextStr; 

            OuterSubtitles = new Dictionary<TimeInterval, string>();

            while (curStrNumber < RawSubtitles.Count)
            {
                nextRecStrNumber = FindNextTitleRecord(RawSubtitles, curStrNumber);
                if (nextRecStrNumber < curStrNumber) break;
                if (nextRecStrNumber > RawSubtitles.Count - 3) break;

                TimeStr = RawSubtitles[nextRecStrNumber + 2];
                TextStr = RawSubtitles[nextRecStrNumber + 3];
                TimeInterval timeInterval = new TimeInterval();
                string text = "";
                GetTimeInterval(TimeStr, out timeInterval);
                GetSubtitleText(TextStr, out text);
                OuterSubtitles.Add(timeInterval, text);
                curStrNumber = nextRecStrNumber+1;
            }
            return false;
        }

    }

}

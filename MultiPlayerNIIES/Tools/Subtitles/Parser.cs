using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.Tools.Subtitles
{

    public static class Parser
    {
        private static bool GetTimeInterval(string s, out Subtitle titleInfo)
        {
            titleInfo = new Subtitle();
            int endOfFirstTime, beginOfSecondTime, delimiterIndex;
            delimiterIndex = s.IndexOf(" --> ");
            if (delimiterIndex < 5) { return false; }
            endOfFirstTime = delimiterIndex - 1;
            beginOfSecondTime = delimiterIndex + 5;

            string FirstTimeStr, SecondTimeStr;
            FirstTimeStr = s.Substring(1, endOfFirstTime);
            SecondTimeStr = s.Substring(beginOfSecondTime);

            if (!TimeSpan.TryParse(FirstTimeStr, out titleInfo.Begin)) return false;
            if (!TimeSpan.TryParse(SecondTimeStr, out titleInfo.End)) return false;

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
                    string taggedSubStr = tmpstr.Substring(beg, end - beg + 1);
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
                curStrNumber = nextRecStrNumber + 1;
            }
            return false;
        }

        /// <summary>
        /// Разбиваем весть интервал с титрами на куски соответствующие непрерывным видеозаписям 
        /// Приходится делать, когда в одном видео насчитывается несколько слепленных вместе видеозаписей и соответственно несколько кусочков титров.
        /// </summary>
        /// <param name="Subtitles">Весь интервал субтитров</param>
        /// <param name="OuterRecords">Коллекция отдельных непрерывных кусочков субтитров</param>
        /// <returns></returns>
        public static bool SegregateSubtitlesToRecords(List<Subtitle> Subtitles, out List<Record> OuterRecords)
        {
            OuterRecords = new List<Record>();
            if (Subtitles.Count < 2) return false;

            OuterRecords.Add(new Record(Subtitles.First().Begin, Subtitles.Last().Begin)); //добавляем весь интервал с титрами, который представлен на видео (короче все видео)

            for (int i = 0; i < Subtitles.Count - 2; i++)
            {
                if (Subtitles[i].TimeFromTextBegin > Subtitles[i + 1].TimeFromTextBegin) //если найдена граница (разрыв) интеравала внутри текста субтитров типа 0:00-0:01-0:02-0:03-0:04-0:05-!ТУТ!-0:00-0:01-0:02....
                {
                    OuterRecords.Last().End = Subtitles[i].Begin;
                    OuterRecords.Add(new Record(Subtitles[i + 1].Begin, Subtitles.Last().Begin));
                }
            }

            return true;
        }

    }

}

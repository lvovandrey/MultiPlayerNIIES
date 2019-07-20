using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.Tools.Subtitles
{
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
}

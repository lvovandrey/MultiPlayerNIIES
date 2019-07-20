using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.Tools.Subtitles
{
    /// <summary>
    /// Класс описывыет часть видео с непрерывными по времени субтитрами - в общем часть видео которая действительно непрерывна
    /// Сами субтитры тут не представлены чтобы не дублировать их
    /// </summary>
    public class Record
    {
        public TimeSpan Begin;
        public TimeSpan End;

        public Record(TimeSpan begin, TimeSpan end)
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

}

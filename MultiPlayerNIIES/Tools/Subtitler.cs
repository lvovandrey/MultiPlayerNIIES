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
        public Subtitler()
        {
            Subtitles = new Dictionary<TimeInterval, string>();
        }
        public void LoadSubtitles(string filename)
        {
            try
            {
                Subtitles.Clear();
                using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Subtitles.Add(new TimeInterval(), line);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка считывания файлов титров: " + e.Message);
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
    }
}

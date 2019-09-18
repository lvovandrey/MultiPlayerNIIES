using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.Tools.Subtitles
{

    /// <summary>
    /// Класс обеспечивает поиск по субтитрам и другие полезные функции
    /// </summary>
    public static class SearchAndTools
    {
        /// <summary>
        /// Бинарный поиск по времени видео в субтитрах
        /// </summary>
        /// <param name="time">Время по видео</param>
        /// <param name="Subtitles">Коллекция субтитров в которых ищем</param>
        /// <returns>Возвращает номер субтитров или -1 если ничего не найдено</returns>
        public static int BinarySearch(TimeSpan time, List<Subtitle> Subtitles)
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

        /// <summary>
        /// Бинарный поиск по времени видео в субтитрах
        /// </summary>
        /// <param name="timeSubs">Время в титре</param>
        /// <param name="Subtitles">Коллекция субтитров в которых ищем</param>
        /// <param name="Record">Интервал, по которому будем искать</param>
        /// <returns>Возвращает номер субтитров или -1 если ничего не найдено</returns>
        public static int BinarySearchInTimesFromTitles(TimeSpan timeSubs, List<Subtitle> Subtitles, Record Record)
        {
            return BinarySearchInTimesFromTitles(timeSubs, Subtitles, Record.Begin, Record.End);
        }

        /// <summary>
        /// Бинарный поиск по времени видео в субтитрах
        /// </summary>
        /// <param name="timeSubs">Время в титре</param>
        /// <param name="Subtitles">Коллекция субтитров в которых ищем</param>
        /// <param name="SearchedIntervalTimeBegin">Время (по видео) начала интервала в коллекции субтиров, по которому нужно произвести поиск </param>
        /// <param name="SearchedIntervalTimeEnd">Время (по видео) конца интервала в коллекции субтиров, по которому нужно произвести поиск </param>
        /// <returns>Возвращает номер субтитров или -1 если ничего не найдено</returns>
        public static int BinarySearchInTimesFromTitles(TimeSpan timeSubs, List<Subtitle> Subtitles, TimeSpan SearchedIntervalTimeBegin, TimeSpan SearchedIntervalTimeEnd)
        {
            int left = BinarySearch(SearchedIntervalTimeBegin,Subtitles);
            if (left == -1) return -1;
            int right = BinarySearch(SearchedIntervalTimeEnd-TimeSpan.FromMilliseconds(200), Subtitles);
            if (right== -1) return -1;
            if (left > right) return -1;

            return BinarySearchInTimesFromTitles(timeSubs, Subtitles, left, right);
        }


        /// <summary>
        /// Бинарный поиск по времени видео в субтитрах
        /// </summary>
        /// <param name="timeSubs">Время в титре</param>
        /// <param name="Subtitles">Коллекция субтитров в которых ищем</param>
        /// <param name="left">Левая граница интервала внутри коллекции по которой ищем</param>
        /// <param name="right">Правая граница интервала внутри коллекции по которой ищем</param>
        /// <returns></returns>
        public static int BinarySearchInTimesFromTitles(TimeSpan timeSubs, List<Subtitle> Subtitles, int left, int right)
        {
            if (left == right)
                return left;
            while (true)
            {
                if (right - left == 1)
                {
                    if (Subtitles[left].CompareWithTitleTime(timeSubs) == 0)
                        return left;
                    if (Subtitles[right].CompareWithTitleTime(timeSubs) == 0)
                        return right;
                    return -1;
                }
                else
                {
                    int middle = left + (right - left) / 2;
                    int comparisonResult = Subtitles[middle].CompareWithTitleTime(timeSubs);
                    if (comparisonResult == 0)
                        return middle;
                    if (comparisonResult < 0)
                        left = middle;
                    if (comparisonResult > 0)
                        right = middle;
                }
            }
        }




        /// <summary>
        /// Метод адаптивно вычисляет наиболее близкий интервал для второго видео, внутри которого содержится искомое время.
        /// Интервалов может быть несколько, возвращается наиболее вероятный (критерий - различие по времени пар титры-видео для первого и второго видео)
        /// </summary>
        /// <param name="timeVideo1">Время по первому видео</param>
        /// <param name="subtitler1">SubtitleProcessor первого видео (по которому указано искомое время)</param>
        /// <param name="subtitler2">SubtitleProcessor второго видео по которому ищем интервал</param>
        /// <returns>Номер наиболее вероятного интервала Record на втором видео где нужно искать данный субтитр</returns>
        public static int SmartSearchRecord(TimeSpan timeVideo1, SubtitleProcessor subtitler1, SubtitleProcessor subtitler2)
        {
            TimeSpan timeSubs1 = subtitler1.GetSubtitle(timeVideo1).TimeFromTextBegin;
            if (subtitler2.Records.Count < 1) return -1;

            /* вычисляем смещение времени на титре ко времени по видео 
            (если субтитры и видео начали записываться в один момент - оно будет малым, 
            а если у нас например вторая часть видео(в том же файле лежит)
            и субтитры пошли с нуля оно будет большим */
            TimeSpan timeDiff = timeVideo1 - timeSubs1;

            //создаем словарь, в котором ключем будет номер Record"а (интеравла), а значением - разница по времени между титрами и видео 
            Dictionary<int, TimeSpan> TSDiffs = new Dictionary<int, TimeSpan>();

            foreach (Record R in subtitler2.Records)
            {
                int subsN = BinarySearchInTimesFromTitles(timeSubs1, subtitler2.Subtitles, R);//ищем номер субтитра по текущему интервалу Record во втором видео
                if (subsN != -1) //если нашли...
                {
                    TimeSpan diffInVid2 = subtitler2.Subtitles[subsN].Begin - timeSubs1;
                    TSDiffs.Add(subtitler2.Records.IndexOf(R), diffInVid2);
                }
            }
            if (TSDiffs.Count < 1) return -1;

            //А теперь надо найти наименьшее отклонение в коллекции TSDiffs от timeDiff - тут просто для ясности мы повторяем перебор - можно было бы в предыдущем цикле все сделать
            Dictionary<int, double> DiffsNormalized = new Dictionary<int, double>();
            foreach (KeyValuePair<int,TimeSpan> diff in TSDiffs)
            {
                double d = Math.Abs(diff.Value.TotalSeconds - timeDiff.TotalSeconds);
                DiffsNormalized.Add(diff.Key, d);
            }
            double min = DiffsNormalized.Min(s => s.Value);
            List<int> ts = DiffsNormalized.Where(s => s.Value.Equals(min)).Select(s => s.Key).ToList();
            if (ts.Count < 1) return -1;
            int result = ts.First();


            return result;
        }
    }
}

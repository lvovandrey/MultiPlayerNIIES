using MultiPlayerNIIES.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace MultiPlayerNIIES.Tools.Serialization
{
    public static class StateSaver
    {
        public static void Save(string filename, VM vm)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ApplicationStateSerialized));
            ApplicationStateSerialized sets = new ApplicationStateSerialized(vm, filename);

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                formatter.Serialize(fs, sets);
            }
        }

        public static void Restore(string filename, VM vm)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ApplicationStateSerialized));
            ApplicationStateSerialized sets;

            if (!File.Exists(filename)) { MessageBox.Show("Файл настроек " + filename + " не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                sets = (ApplicationStateSerialized)formatter.Deserialize(fs);
            }
            if (sets == null) { MessageBox.Show("Ошибка открытия файла настроек. Десериализатор вернул null."); return; }

            vm.СloseAllCommand.Execute(null);

            ValidateSets(sets);
            RestoreViewModel(sets, vm);

        }

        private static void ValidateSets(ApplicationStateSerialized sets)
        {
            string ErrorsReport = "";
            //удалим видео которые не найдены
            int c = sets.Players.Count;
            for (int i = 0; i < c; i++)
                if (!File.Exists(sets.Players[i].filename))
                {
                    sets.Players.Remove(sets.Players[i]);
                    ErrorsReport += @"Видео не найдено: " + sets.Players[i].filename + "\n";
                    c--;
                }

            if (sets.IsExcelConnected && !File.Exists(sets.ExcelBookFilename))
            {
                ErrorsReport += @"Excel-файл не найден: " + sets.ExcelBookFilename + "\n";
                sets.IsExcelConnected = false;
            }
            double w = SystemParameters.PrimaryScreenWidth;
            double h = SystemParameters.PrimaryScreenHeight;

            if (sets.Position.Width < 100 ||
                sets.Position.Height < 100 ||
                (sets.Position.Left + sets.Position.Width < 0) ||
                (sets.Position.Top + sets.Position.Height < 0) ||
                (sets.Position.Top + sets.Position.Height < 0) ||
                (sets.Position.Top + sets.Position.Height < 0) ||
                (sets.Position.Left > w) ||
                (sets.Position.Top > h) ||
                (sets.Position.Top < 0))
            {
                ErrorsReport += "Главное окно расположено некорректно (судя по сохраненному файлу - где-то за пределами экрана)\n";
                sets.Position = new Rect(0, 0, w, h);
            }

            int PlayerId = 0;
            foreach (var v in sets.Players)
            {
                PlayerId++;
                if (v.Position.Width < 50 ||
                    v.Position.Height < 50 ||
                   (v.Position.Left + sets.Position.Width < 0) ||
                   (v.Position.Top + sets.Position.Height < 0) ||
                   (v.Position.Top + sets.Position.Height < 0) ||
                   (v.Position.Top + sets.Position.Height < 0) ||
                   (v.Position.Top < 0))
                {
                    ErrorsReport += "Окно проигрывателя файла " + Path.GetFileName(v.filename) + " расположено некорректно\n";
                    v.Position = new Rect(0 + PlayerId * 50, 0 + PlayerId * 50, 300, 400);
                }
            }

            foreach (var v in sets.Players)
            {
                if (v.CurTime < TimeSpan.Zero) v.CurTime = TimeSpan.Zero;
                if (v.CurTime + v.TimeShift > v.Duration) v.CurTime = TimeSpan.Zero;
                if (v.CurTime + v.TimeShift < TimeSpan.Zero) v.TimeShift = TimeSpan.Zero;
            }
        }

        private static void RestoreViewModel(ApplicationStateSerialized sets, VM vm)
        {
            vm.MainWindow.Left = sets.Position.Left;
            vm.MainWindow.Top = sets.Position.Top;
            vm.MainWindow.Width = sets.Position.Width;
            vm.MainWindow.Height = sets.Position.Height;

            //готовим перечень файлов...
            string[] filenames = new string[sets.Players.Count];
            for (int i = 0; i < sets.Players.Count; i++)
                filenames[i] = sets.Players[i].filename;
            //... и зон размещения проигрывателей для ...
            Rect[] areas = new Rect[sets.Players.Count];
            for (int i = 0; i < sets.Players.Count; i++)
                areas[i] = sets.Players[i].Position;
            //... открытия
            vm.OpenVideos(filenames, areas);

            //Назначаем лидера синхронизации
            int SyncLeadId = 0;
            foreach (var v in sets.Players)
                if (v.IsSyncLeader) SyncLeadId = sets.Players.IndexOf(v);
            vm.SyncLeadPlayer = vm.videoPlayerVMs[SyncLeadId];

            //переводим все видео на нужный момент
            foreach (VideoPlayerVM v in vm.videoPlayerVMs)
            {
                v.CurTime = sets.Players[vm.videoPlayerVMs.IndexOf(v)].CurTime;
                v.SyncronizationShiftVM.ShiftTime = sets.Players[vm.videoPlayerVMs.IndexOf(v)].TimeShift;
            }
        }


    }
}

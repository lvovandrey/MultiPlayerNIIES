using MultiPlayerNIIES.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MultiPlayerNIIES.Tools.Syncronization
{
    public class Syncronizator
    {
        VM VM;
        public Syncronizator(VM vm)
        {
            VM = vm;
        }

        public async void SyncronizeOnShiftAsync()
        {
            VM.SyncDeltasBuffer.Clear();
            VM.SyncDelta = TimeSpan.Zero;

            await Task.Run(SyncronizeOnShift);

        }

        bool isSyncInProcess = false;
        public bool IsSyncInProcess { get { return isSyncInProcess; } }

        /// <summary>
        /// Синхронизировать все плееры
        /// </summary>
        public void SyncronizeOnShift()
        {
            if (IsSyncInProcess) return;
            isSyncInProcess = true;
            bool IsSyncLeadPaused = VM.SyncLeadPlayer.IsPaused;
            VM.SyncDeltasBuffer.Clear();
            VM.SyncDelta = TimeSpan.Zero;

            VM.MainTimer.Stop();
            int AttemptCounter = 0;
            do
            {
                foreach (VideoPlayerVM v in VM.videoPlayerVMs)
                    v.Body.Pause();
                Thread.Sleep(50);
                if (AttemptCounter++ > 20) { Console.WriteLine("Syncronizator.SyncronizeOnShift() - не все плеры могут быть остановлены. Синхронизация возможно сорвется."); break; }
            } while (!IsAllPlayersPaused());

            foreach (VideoPlayerVM v in VM.videoPlayerVMs)
            {
                do
                {
                    SyncronizeOnePlayer(v);
                    Thread.Sleep(50);
                    if (AttemptCounter++ > 50) { Console.WriteLine("Syncronizator.SyncronizeOnShift() - плеер " + v.FilenameForTitle + " не перешел в нужную позицию."); break; }
                } while (!IsPlayerAlreadySyncronized(v));
            }

            

            
            Thread.Sleep(3000);

            foreach (VideoPlayerVM v in VM.videoPlayerVMs)
                v.IsPausedReal = false;
            Console.WriteLine("КОНЕЦ СИНХРОНИЗАЦИИ");
            VM.MainTimer.Start();
            isSyncInProcess = false;
        }

        /// <summary>
        /// Все ли плееры остановлены
        /// </summary>
        /// <returns></returns>
        bool IsAllPlayersPaused()
        {
            bool flag = true;
            foreach (VideoPlayerVM v in VM.videoPlayerVMs)
            {
                if (!v.IsPausedReal)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        bool IsAnyPlayerPaused()
        {
            bool flag = false;
            foreach (VideoPlayerVM v in VM.videoPlayerVMs)
            {
                if (v.IsPausedReal)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }


        /// <summary>
        /// Осуществляет синхронизацию указанного плеера 
        /// </summary>
        /// <param name="v">VM-ка плеера, который будет синхронизироваться</param>
        void SyncronizeOnePlayer(VideoPlayerVM v)
        {
            if (!v.IsSyncronizeLeader)
            {
                if (v.SyncronizationShiftVM.ShiftTime + VM.TimeSyncLead > v.Duration)
                {
                    v.CurTime = v.Duration - TimeSpan.FromSeconds(0.01);
                    return;
                }
                else if (v.SyncronizationShiftVM.ShiftTime + VM.TimeSyncLead <= TimeSpan.Zero)
                {
                    v.CurTime = TimeSpan.FromSeconds(0.01);
                    return;
                }
                else
                    v.CurTime = v.SyncronizationShiftVM.ShiftTime + VM.TimeSyncLead;
            }
            else v.CurTime = VM.TimeSyncLead;
        }

        /// <summary>
        /// Синхронизирован ли уже указанный плеер 
        /// </summary>
        /// <param name="v">ВМ-ка плеера</param>
        /// <returns></returns>
        bool IsPlayerAlreadySyncronized(VideoPlayerVM v)
        {
            if (!v.IsSyncronizeLeader)
            {
                if (v.SyncronizationShiftVM.ShiftTime + VM.TimeSyncLead > v.Duration)
                {
                    if (!(v.CurTime <= v.Duration && v.CurTime > v.Duration - TimeSpan.FromSeconds(0.02)))
                    {
                        return false;
                    }
                }
                else if (v.SyncronizationShiftVM.ShiftTime + VM.TimeSyncLead <= TimeSpan.Zero)
                {
                    if (!(v.CurTime >= TimeSpan.Zero && v.CurTime < TimeSpan.FromSeconds(0.02)))
                    {
                        return false;
                    }
                }
                else
                {
                    TimeSpan t = v.SyncronizationShiftVM.ShiftTime + VM.TimeSyncLead;
                    if (!(v.CurTime >= t - TimeSpan.FromSeconds(0.01) && v.CurTime <= t + TimeSpan.FromSeconds(0.01)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

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
        PlayerControlTools PlayerControlTools;
        public Syncronizator(VM vm)
        {
            VM = vm;
            PlayerControlTools = VM.PlayerControlTools;
        }

        public async void SyncronizeOnShiftAsync()
        {
            await Task.Run(SyncronizeOnShift);

        }



        /// <summary>
        /// Синхронизировать все плееры
        /// </summary>
        public void SyncronizeOnShift()
        {
            if (PlayerControlTools.IsOperationInProcess) return;
            PlayerControlTools.isOperationInProcess = true;
            bool IsSyncLeadPaused = VM.SyncLeadPlayer.IsPaused;
            VM.MainTimer.Stop();
            int AttemptCounter = 0;

            for (int i = 0; i < 20; i++)
            {
                foreach (VideoPlayerVM v in VM.videoPlayerVMs)
                    v.Body.Pause();
                Thread.Sleep(20);
            }

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

            VM.SyncDeltasBuffer.Clear();
            VM.SyncDelta = TimeSpan.Zero;
            Thread.Sleep(2000);

            VM.MainTimer.Start();
           
            PlayerControlTools.isOperationInProcess = false;
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
                if (!v.IsPaused)
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
                if (v.IsPaused)
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

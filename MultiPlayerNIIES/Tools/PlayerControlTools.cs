using MultiPlayerNIIES.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.Tools
{
    public class PlayerControlTools
    {
        VM VM;
        public PlayerControlTools(VM vm)
        {
            VM = vm;
        }

        bool isOperationInProcess= false;
        public bool IsOperationInProcess { get { return isOperationInProcess; } }

        public async void PlayAllAsync()
        {
            await Task.Run(PlayAll);
        }

        public async void PauseAllAsync()
        {
            await Task.Run(PauseAll);
        }

        /// <summary>
        /// Старт всех плееров
        /// </summary>
        public void PlayAll()
        {

            if (IsOperationInProcess) return;
            isOperationInProcess = true;
            int AttemptCounter = 0;

            do
            {
                foreach (VideoPlayerVM v in VM.videoPlayerVMs)
                    v.Body.Play();
                Thread.Sleep(20);
                if (AttemptCounter++ > 500)
                {
                    Console.WriteLine("Syncronizator.SyncronizeOnShift() - не все плеры могут быть запущены. Хьюстон у нас проблемы.");
                    break;
                }
            } while (!IsAllPlayersPlayed());
            Console.WriteLine("СТАРТ");
            Thread.Sleep(1000);
            isOperationInProcess = false;

        }

        /// <summary>
        /// Пауза всех плееров
        /// </summary>
        public void PauseAll()
        {
            if (IsOperationInProcess) return;
            isOperationInProcess = true;
            int AttemptCounter = 0;

            do
            {
                foreach (VideoPlayerVM v in VM.videoPlayerVMs)
                    v.Body.Pause();
                Thread.Sleep(20);
                if (AttemptCounter++ > 500)
                {
                    Console.WriteLine("Syncronizator.SyncronizeOnShift() - не все плеры могут быть остановлены. Хьюстон у нас проблемы.");
                    break;
                }
            } while (!IsAllPlayersPaused());

            Console.WriteLine("ПАУЗА");


            Thread.Sleep(1000);
            foreach (VideoPlayerVM v in VM.videoPlayerVMs)
                v.IsPausedReal = false;
            isOperationInProcess = false;

        }


        /// <summary>
        /// Все ли плееры запущены
        /// </summary>
        /// <returns></returns>
        bool IsAllPlayersPlayed()
        {
            bool flag = true;
            foreach (VideoPlayerVM v in VM.videoPlayerVMs)
            {
                if (!v.IsPlayingEx)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        /// <summary>
        /// Все ли плееры на паузе
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



    }
}

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
            ToolsTimer.Timer(TimerTick, TimeSpan.FromMilliseconds(50));
        }


        Stack<string> CommandStack =  new Stack<string>();

        void TimerTick()
        {
            ExecuteCommand();
        }

        private void ExecuteCommand()
        {
            if (isOperationInProcess) return;
            if (CommandStack.Count == 0) return;
            switch (CommandStack.Pop())
            {
                case "PlayAll": PlayAllAsync(); break;
                case "PauseAll": PauseAllAsync(); break;
                default:
                    break;
            }
            CommandStack.Clear();
        }

        public void CallCommand(string commandName)
        {
            CommandStack.Push(commandName);
        }

        public bool isOperationInProcess= false;
        public bool IsOperationInProcess { get { return isOperationInProcess; } }


        async void PlayAllAsync()
        {
            await Task.Run(PlayAll);
        }

        async void PauseAllAsync()
        {
            await Task.Run(PauseAll);
        }

        /// <summary>
        /// Старт всех плееров
        /// </summary>
        void PlayAll()
        {

            if (IsOperationInProcess) return;
            isOperationInProcess = true;
            int AttemptCounter = 0;

            
            for (int i = 0; i < 20; i++)
            {
                foreach (VideoPlayerVM v in VM.videoPlayerVMs)
                    v.Body.Play();
                Thread.Sleep(20);
            }
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



            // Console.WriteLine(AttemptCounter);
            Thread.Sleep(500);
            isOperationInProcess = false;

        }

        /// <summary>
        /// Пауза всех плееров
        /// </summary>
        void PauseAll()
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

            Console.WriteLine(AttemptCounter);
            

            Thread.Sleep(500);
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
                if (!v.IsPausedEx)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }



    }
}

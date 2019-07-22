using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MultiPlayerNIIES.ViewModel
{
    class VideoPlayerVM : INPCBase
    {
        VideoPlayerView Body; //Ну это не настоящая VM
        Grid Container;
        VM VM;

        public VideoPlayerVM(Grid container, VM vm, Rect AreaForPlacementInContainer)
        {
            Container = container;
            Body = new VideoPlayerView();
            VM = vm;
            container.Children.Add(Body);
            Body.DragDropSwitchOn(Container, Body.Dragger);
            Body.ResizeSwitchOn(Container);
            Body.HorizontalAlignment = HorizontalAlignment.Left;
            Body.VerticalAlignment = VerticalAlignment.Top;
            Replace(AreaForPlacementInContainer);
        }


        #region Methods
        public void LoadFile(string filename)
        {
            if (File.Exists(filename))
            {
                Body.Load(filename);
            }
            else
            {
                MessageBox.Show("Файл " + filename + " не найден.");
            }
        }

        public void Replace(Rect AreaForPlacementInContainer)
        {
            Body.Margin = new Thickness(AreaForPlacementInContainer.Left, AreaForPlacementInContainer.Top, 0, 0);
            Body.Width = AreaForPlacementInContainer.Width;
            Body.Height = AreaForPlacementInContainer.Height;
        }
        #endregion

        #region КОМАНДЫ
        private RelayCommand playCommand;
        public RelayCommand PlayCommand
        {
            get
            {
                return playCommand ??
                  (playCommand = new RelayCommand(obj =>
                  {

                  }));
            }
        }
        #endregion
    }
}

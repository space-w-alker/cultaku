using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace culTAKU
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private int keyDownTimeShift;
        private int keyDownTimeCtrl;

        public App()
        {
            //var si = GetContentStream(new Uri("./Styles/blue_skin.xaml",UriKind.Relative));
            //var rd = (ResourceDictionary)XamlReader.Load(si.Stream);
            //Current.Resources.MergedDictionaries.Add(rd);


            EventManager.RegisterClassHandler(typeof(Control), Window.KeyUpEvent, new KeyEventHandler(keyUp), false);
            EventManager.RegisterClassHandler(typeof(Control), Window.KeyDownEvent, new KeyEventHandler(KeyDown), false);
            keyDownTimeCtrl = 0;
            keyDownTimeShift = 0;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if(Misc.Miscelleneous.MainWindow.Player.PlayerState == ViewsAndControllers.AnimePlayer.PLAYER_STATE.STOPPED)
            {
                return;
            }
            
            if(e.Key == Key.Right)
            {
                if(Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if (e.Timestamp - keyDownTimeShift < 300)
                    {
                        
                        return;
                    }
                    keyDownTimeShift = e.Timestamp;
                    Misc.Miscelleneous.MainWindow.Player.AnimeMediaPlayer.Position = Misc.Miscelleneous.MainWindow.Player.AnimeMediaPlayer.Position.Add(TimeSpan.FromSeconds(3));
                }
            }

            if (e.Key == Key.Left)
            {
                if (Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if (e.Timestamp - keyDownTimeShift < 300)
                    {
                        
                        return;
                    }
                    keyDownTimeShift = e.Timestamp;
                    Misc.Miscelleneous.MainWindow.Player.AnimeMediaPlayer.Position = Misc.Miscelleneous.MainWindow.Player.AnimeMediaPlayer.Position.Subtract(TimeSpan.FromSeconds(3));
                }
            }
            
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            if(Misc.Miscelleneous.MainWindow.AppState == culTAKU.MainWindow.APP_STATE.PLAYING)
            {
                if (e.Key == Key.Space)
                {
                    Misc.Miscelleneous.MainWindow.Player.Toggle_Play_Pause();
                    e.Handled = true;
                }
            }
        }
    }
}

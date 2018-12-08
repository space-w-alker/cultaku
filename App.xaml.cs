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
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            int a = 0;
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

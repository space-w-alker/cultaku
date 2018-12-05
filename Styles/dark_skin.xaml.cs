using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace culTAKU.Styles
{
    partial class dark_skin : System.Windows.ResourceDictionary
    {
        public dark_skin()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FrameworkElement source = (FrameworkElement)e.Source;
            Misc.Miscelleneous.MainWindow.PlayAnime((long)source.Tag);
        }
    }
}

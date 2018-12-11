using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace culTAKU.ViewsAndControllers
{
    /// <summary>
    /// Interaction logic for DetailsView.xaml
    /// </summary>
    public partial class DetailsView : UserControl
    {
       
        

        public DetailsView()
        {
            InitializeComponent();
            EpisodesList.MouseDoubleClick += EpisodesList_MouseDoubleClick;
        }

        private void EpisodesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EpisodesList.SelectedItem == null) return;
            Models.Anime selected = ((Models.AnimeEpisode)EpisodesList.SelectedItem).ParentAnime;

            Misc.Miscelleneous.MainWindow.PlayAnime(selected.Id, EpisodesList.SelectedIndex);
        }
    }
}

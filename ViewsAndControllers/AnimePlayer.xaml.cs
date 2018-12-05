using System;
using System.Collections.Generic;
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
    /// Interaction logic for AnimePlayer.xaml
    /// </summary>
    public partial class AnimePlayer : UserControl
    {
        private Models.Anime CurrentlyPlayingAnime;
        private Models.AnimeEpisode CurrentlyPlayingEpisode;

        public AnimePlayer()
        {
            InitializeComponent();
            AnimeMediaPlayer.LoadedBehavior = MediaState.Manual;
            AnimeMediaPlayer.UnloadedBehavior = MediaState.Manual;
        }

        public void PlayAnime(Models.Anime anime, int EpisodeNumber = -1)
        {
            AnimeMediaPlayer.Source = new Uri(anime.ListOfUnOrderedEpisodes[0].EpisodePath);
            
            AnimeMediaPlayer.Play();
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.IO;
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

namespace culTAKU.ViewsAndControllers.Extras
{
    /// <summary>
    /// Interaction logic for ContinueWatchingView.xaml
    /// </summary>
    public partial class ContinueWatchingView : UserControl
    {
        public ObservableCollection<Models.Anime> ContinueWatchingList;
        DispatcherTimer Timer;

        public ContinueWatchingView()
        {
            InitializeComponent();
            PreviewContinuePlaying.LoadedBehavior = MediaState.Manual;
            PreviewContinuePlaying.UnloadedBehavior = MediaState.Manual;
            ContinueWatchingList = new ObservableCollection<Models.Anime>();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(10);
            Timer.Tick += Timer_Tick;
            ListContinue.DataContext = ContinueWatchingList;
            ListContinue.ItemsSource = ContinueWatchingList;
            ListContinue.SelectionChanged += ListContinue_SelectionChanged;
            ContinueWatchingButton.Click += ContinueWatchingButton_Click;
        }

        private void ContinueWatchingButton_Click(object sender, RoutedEventArgs e)
        {
            if(ListContinue.SelectedItem != null)
            {
                Models.Anime anime = (Models.Anime)ListContinue.SelectedItem;
                Misc.Miscelleneous.MainWindow.PlayAnime(anime.Id, anime.ListOfEpisodes.IndexOf(anime.LastPlayed), true);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ListContinue.SelectedIndex = (ListContinue.SelectedIndex + 1) % ContinueWatchingList.Count;
        }

        private void ListContinue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Timer.Stop();
            
            PreviewContinuePlaying.Stop();
            Models.AnimeEpisode lastPlayed = null;
            try
            {
                lastPlayed = ((Models.Anime)ContinueWatchingList[ListContinue.SelectedIndex]).LastPlayed;
                ContinueWatchingText.Text = string.Format("Continue Watching {0} Episode {1}", lastPlayed.ParentAnime.Name, lastPlayed.EpisodeNumber);
            }
            catch (ArgumentOutOfRangeException error)
            {
                return;
            }
            PreviewContinuePlaying.Source = new Uri(lastPlayed.EpisodePath, UriKind.Absolute);
            
            PreviewContinuePlaying.Play();
            PreviewContinuePlaying.Position = lastPlayed.StoppedAt.Subtract(TimeSpan.FromSeconds(12));
            Timer.Start();
        }

        public void CloseAll()
        {
            Timer.Stop();
            PreviewContinuePlaying.Stop();
        }

        public void LoadContinueWatching()
        {
            ContinueWatchingList.Clear();
            Timer.Stop();
            foreach(Models.AnimeEpisode episode in Misc.Miscelleneous.MainWindow.MyAnimeCollection.ContinueWatching)
            {
                ContinueWatchingList.Add(episode.ParentAnime);
            }
            ListContinue.SelectedIndex = 0;
            Timer.Start();
        }
    }
}

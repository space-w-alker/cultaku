using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace culTAKU.ViewsAndControllers
{
    /// <summary>
    /// Interaction logic for AnimePlayer.xaml
    /// </summary>
    public partial class AnimePlayer : UserControl, INotifyPropertyChanged
    {
        public enum PLAYER_STATE {PLAYING, FILE_NOT_FOUND, STOPPED, PAUSED }

        

        //private Models.Anime CurrentlyPlayingAnime;
        private int currentIndex;
        private Storyboard Burst;
        private DispatcherTimer Timer;
        private DateTime SeekUpdateTimeStamp;
        private long IdleTime;
        public PLAYER_STATE PlayerState;
        

        public ObservableCollection<Models.AnimeEpisode> CombinedEpisodes;

        public event PropertyChangedEventHandler PropertyChanged;

        public int PlayIndex
        {
            get { return currentIndex; }
            set { currentIndex = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayIndex")); }
        }

        public AnimePlayer()
        {
            PlayerState = PLAYER_STATE.STOPPED;

            InitializeComponent();
            AnimeMediaPlayer.LoadedBehavior = MediaState.Manual;
            AnimeMediaPlayer.UnloadedBehavior = MediaState.Manual;
            Burst = Resources["Burst"] as Storyboard;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(911);
            Timer.Tick += Timer_Tick;
            
            

            
            AnimeMediaPlayer.MediaOpened += AnimeMediaPlayer_MediaOpened;
            AnimeMediaPlayer.MediaEnded += AnimeMediaPlayer_MediaEnded;
            AnimeMediaPlayer.PreviewMouseLeftButtonUp += AnimeMediaPlayer_PreviewMouseLeftButtonUp;
            AnimeMediaPlayer.PreviewMouseMove += AnimeMediaPlayer_PreviewMouseMove;
            AnimeMediaPlayer.SourceUpdated += AnimeMediaPlayer_SourceUpdated;
            SeekBar.ValueChanged += SeekBar_ValueChanged;
            EpisodesList.MouseDoubleClick += EpisodesList_MouseDoubleClick;
        }

        private void EpisodesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlayNextEpisode(EpisodesList.SelectedIndex);
        }

        private void AnimeMediaPlayer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            PlayerControls.Visibility = Visibility.Visible;
            IdleTime = 0;
        }

        private void AnimeMediaPlayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowHideControls();
        }

        private void AnimeMediaPlayer_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            PlayerState = PLAYER_STATE.STOPPED;
        }

        private void AnimeMediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            PlayNextEpisode();
        }

        private void SeekBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PlayerState == PLAYER_STATE.STOPPED)
            {
                return;
            }

            if (DateTime.Now.Subtract(SeekUpdateTimeStamp).TotalMilliseconds > 211)
            {
                AnimeMediaPlayer.Position = TimeSpan.FromSeconds(SeekBar.Value);
                SeekUpdateTimeStamp = DateTime.Now;
            }
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SeekBar.Value = AnimeMediaPlayer.Position.TotalSeconds;
            TimeSpan pos = AnimeMediaPlayer.Position;
            TimeSpan tot = AnimeMediaPlayer.NaturalDuration.TimeSpan;
            DateTime date = DateTime.Now;
            Regex formatTime = new Regex(@".*? (?<str>\d*:\d*):\d*? (?<tm>.*)");
            var matched = formatTime.Match(date.AddSeconds(tot.TotalSeconds - pos.TotalSeconds).ToString());
            EndsAtDisplay.Text = string.Format("Ends At {0} {1}", matched.Groups["str"], matched.Groups["tm"]);
            PlayTime.Text = string.Format("{0}:{1}:{2} / {3}:{4}:{5}", pos.Hours, pos.Minutes, pos.Seconds, tot.Hours, tot.Minutes, tot.Seconds);
            IdleTime += 300;
            if (IdleTime > 5000)
            {
                PlayerControls.Visibility = Visibility.Hidden;
            }
        }

        private void AnimeMediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            SeekBar.Maximum = AnimeMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            Timer.Start();
            PlayerState = PLAYER_STATE.PLAYING;
            SetEpisodeStatus(Models.AnimeEpisode.STATUS.WATCHING, CombinedEpisodes[PlayIndex]);
            CurrentPlayingDisplay.Text = string.Format("{0} Episode {1}", CombinedEpisodes[PlayIndex].ParentAnime.Name, CombinedEpisodes[PlayIndex].EpisodeNumber);
            IdleTime = 0;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Toggle_Play_Pause();
        }

        

        public void PlayAnime(Models.Anime anime, int EpisodeNumber = -1)
        {
            CombinedEpisodes = new ObservableCollection<Models.AnimeEpisode>(anime.ListOfEpisodes.Concat(anime.ListOfUnOrderedEpisodes));
            EpisodesList.DataContext = CombinedEpisodes;
            AnimeMediaPlayer.Source = new Uri(CombinedEpisodes[0].EpisodePath, UriKind.Absolute);
            PlayIndex = 0;
            AnimeMediaPlayer.Play();
            SeekUpdateTimeStamp = DateTime.Now;
        }


        public void PlayNextEpisode(int index = -1)
        {
            Timer.Stop();
            if(100.0 * AnimeMediaPlayer.Position.TotalSeconds / AnimeMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds > 85)
            {
                SetEpisodeStatus(Models.AnimeEpisode.STATUS.WATCHED, CombinedEpisodes[PlayIndex]);
            }
            CombinedEpisodes[PlayIndex].StoppedAt = AnimeMediaPlayer.Position;
            AnimeMediaPlayer.Stop();

            if (index == -1)
            {
                PlayIndex = (PlayIndex + 1) % CombinedEpisodes.Count;
            }
            else
            {
                PlayIndex = index;
            }
            AnimeMediaPlayer.Source = new Uri(CombinedEpisodes[PlayIndex].EpisodePath, UriKind.Absolute);
            AnimeMediaPlayer.Play();
        }

        public void PlayPreviousEpisode()
        {
            Timer.Stop();
            if (100.0 * AnimeMediaPlayer.Position.TotalSeconds / AnimeMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds > 85)
            {
                SetEpisodeStatus(Models.AnimeEpisode.STATUS.WATCHED, CombinedEpisodes[PlayIndex]);
            }
            CombinedEpisodes[PlayIndex].StoppedAt = AnimeMediaPlayer.Position;
            AnimeMediaPlayer.Stop();
            PlayIndex = (PlayIndex - 1) % CombinedEpisodes.Count;
            AnimeMediaPlayer.Source = new Uri(CombinedEpisodes[PlayIndex].EpisodePath, UriKind.Absolute);
            AnimeMediaPlayer.Play();
        }

        public void Play_Pressed()
        {
            AnimeMediaPlayer.Play();
            Burst.Begin(PlayButtonPathScreen, true);
            PlayerState = PLAYER_STATE.PLAYING;
        }

        public void Pause_Pressed()
        {
            Burst.Begin(PauseButtonPathScreen, true);
            AnimeMediaPlayer.Pause();
            PlayerState = PLAYER_STATE.PAUSED;
        }

        public void Toggle_Play_Pause()
        {
            if (PlayerState == PLAYER_STATE.PLAYING)
            {
                Pause_Pressed();
            }
            else if (PlayerState == PLAYER_STATE.PAUSED)
            {
                Play_Pressed();
            }
        }

        public void ShowHideControls()
        {
            if (PlayerControls.Visibility == Visibility.Visible)
            {
                PlayerControls.Visibility = Visibility.Hidden;
            }
            else if (PlayerControls.Visibility == Visibility.Hidden)
            {
                PlayerControls.Visibility = Visibility.Visible;
            }
        }

        private void SetEpisodeStatus(Models.AnimeEpisode.STATUS status, Models.AnimeEpisode episode)
        {
            if(episode.Status != Models.AnimeEpisode.STATUS.WATCHED)
            {
                episode.Status = status;          
            }
        }

        public void Closing()
        {
            Timer.Stop();
            if (100.0 * AnimeMediaPlayer.Position.TotalSeconds / AnimeMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds > 85)
            {
                SetEpisodeStatus(Models.AnimeEpisode.STATUS.WATCHED, CombinedEpisodes[PlayIndex]);
            }
            Misc.Miscelleneous.MainWindow.MyAnimeCollection.AddToContinueWatching(CombinedEpisodes[PlayIndex]);
            CombinedEpisodes[PlayIndex].StoppedAt = AnimeMediaPlayer.Position;
            CombinedEpisodes[PlayIndex].ParentAnime.LastPlayed = CombinedEpisodes[PlayIndex];
            Models.CollectionHandler.SaveMyCollectionFile(Misc.Miscelleneous.MainWindow.MyAnimeCollection);
            AnimeMediaPlayer.Close();
            CombinedEpisodes = null;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PlayNextEpisode();
        }

        public void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            PlayPreviousEpisode();
        }
    }
}

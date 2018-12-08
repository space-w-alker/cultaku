using culTAKU.Models;
using culTAKU.ViewsAndControllers;
using culTAKU.ViewsAndControllers.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace culTAKU
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public enum APP_STATE { HOME, PLAYING, DETAILS_VIEW, ANIME_HISTORY}

        internal AnimeCollection MyAnimeCollection;
        internal TopOptionsBar TopBar;
        internal HomeView HomeViewObject;
        internal StatusDisplay statusDisplay;
        internal AnimePlayer Player;
        internal bool isConnected;
        internal APP_STATE AppState { get; set; }
        ICollectionView animeDisplayView;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected")); }
        }

        public ICollectionView AnimeDisplayView { get => animeDisplayView; set => animeDisplayView = value; }

        public MainWindow()
        {

            
            MyAnimeCollection = CollectionHandler.LoadMyCollectionFile();
            statusDisplay = new StatusDisplay();
            InitializeComponent();
            

            if (MyAnimeCollection == null || MyAnimeCollection.AllAnimePaths.Count < 1)
            {
                MyAnimeCollection = new AnimeCollection();
            }
            
            Misc.Miscelleneous.MainWindow = this;

            HomeViewObject = new HomeView(MyAnimeCollection);
            mainGrid.Children.Insert(0, HomeViewObject);

            Player = new AnimePlayer();

            TopBar = new TopOptionsBar();
            OverLayer.Children.Add(TopBar);
            animeDisplayView = CollectionViewSource.GetDefaultView(HomeViewObject.DataContext);
            TopBar.SearchBar.TextChanged += SearchBar_TextChanged;
            HomeViewObject.anime_list.MouseDoubleClick += Anime_list_MouseDoubleClick;
            Player.ExitButton.Click += ExitButton_Click;
            AppState = APP_STATE.HOME;

        }

        private void Anime_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Anime selected = (Anime)HomeViewObject.anime_list.SelectedItem;
            DetailsView details = new DetailsView();
            details.DataContext = selected;
            details.EpisodesList.DataContext = selected.ListOfEpisodes;
            details.EpisodesList.ItemsSource = selected.ListOfEpisodes;

            mainGrid.Children.RemoveAt(0);
            mainGrid.Children.Insert(0, details);
            AppState = APP_STATE.DETAILS_VIEW;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Closing();
            mainGrid.Children[0].Visibility = Visibility.Visible;
            AppState = APP_STATE.HOME;
            mainGrid.Children.RemoveAt(1);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (TopBar.SearchBar.Text.Length == 0) AnimeDisplayView.Filter = null;
            

            if (String.IsNullOrWhiteSpace(TopBar.SearchBar.Text) || TopBar.SearchBar.Text.Length < 3)
            {
                return;
            }

            else
            {
                AnimeDisplayView.Filter = obj => ((Anime)obj).Name.IndexOf(TopBar.SearchBar.Text, StringComparison.InvariantCultureIgnoreCase) > -1;
            }
        }

        public void PlayAnime(long animeId, int EpisodeNumber = -1)
        {
            foreach(Anime anime in MyAnimeCollection.ListOfAnime)
            {
                if (anime.Id == animeId)
                {
                    Player.PlayAnime(anime);
                    //mainGrid.Children.RemoveAt(0);
                    mainGrid.Children[0].Visibility = Visibility.Collapsed;
                    mainGrid.Children.Insert(1, Player);
                    AppState = APP_STATE.PLAYING;
                }
            }
        }

        public void CheckInternet()
        {
            statusDisplay.DisplayImagePath = StatusDisplay.StatusType.LOADING;
            statusDisplay.DisplayText = "Checking Internet Connection...";
            OverLayer.Children.Add(statusDisplay);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool conn = Misc.Miscelleneous.IsConnectionActive();
                Dispatcher.BeginInvoke(new Action(()=> {
                    IsConnected = conn;
                }));
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(AppState == APP_STATE.PLAYING)
            {
                Player.Closing();
            }
            CollectionHandler.SaveMyCollectionFile(MyAnimeCollection);
        }

        
    }
}

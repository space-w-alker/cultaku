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
        internal ContinueWatchingView ContinueWatching;
        internal bool isConnected;
        private APP_STATE appState;
        internal APP_STATE AppState {get { return appState; } set { appState = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AppState")); } }
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
            TopBar = new TopOptionsBar();
            HomeViewObject = new HomeView(MyAnimeCollection);
            mainGrid.Children.Insert(0, HomeViewObject);

            Player = new AnimePlayer();

            
            if(MyAnimeCollection.ContinueWatching.Count > 2)
            {
                ContinueWatching = new ContinueWatchingView();
                //ContinueWatching.LoadContinueWatching();
                HomeViewObject.HomeViewGrid.Children.Insert(0, ContinueWatching);
                ContinueWatching.ExitButton.Click += ExitButton_Click1;
            }
            HomeViewObject.MainGrid.Children.Add(TopBar);

            animeDisplayView = CollectionViewSource.GetDefaultView(HomeViewObject.DataContext);
            animeDisplayView.SortDescriptions.Add(new SortDescription("Rating", ListSortDirection.Descending));
            animeDisplayView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            PropertyChanged += MainWindow_PropertyChanged;
            TopBar.SearchBar.TextChanged += SearchBar_TextChanged;
            TopBar.SortOption.SelectionChanged += SortOption_SelectionChanged;
            HomeViewObject.anime_list.MouseDoubleClick += Anime_list_MouseDoubleClick;
            HomeViewObject.anime_list.KeyUp += Anime_list_KeyUp;
            Player.ExitButton.Click += ExitButton_Click;
            
            AppState = APP_STATE.HOME;

        }

        

        private void ExitButton_Click1(object sender, RoutedEventArgs e)
        {
            HomeViewObject.HomeViewGrid.Children.RemoveAt(0);
            ContinueWatching.CloseAll();
            ContinueWatching = null;
        }

        private void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "AppState")
            {
                if (ContinueWatching == null) return;
                if(AppState == APP_STATE.HOME)
                {
                    ContinueWatching.LoadContinueWatching();
                }
                else { ContinueWatching.CloseAll(); }
            }
        }

        private void Anime_list_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                OpenDetails();
            }
        }

        private void Anime_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenDetails();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Button back_button = (Button)sender;
            back_button.Click -= BackButton_Click;
            mainGrid.Children.RemoveAt(0);
            mainGrid.Children.Insert(0, HomeViewObject);
            AppState = APP_STATE.HOME;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Closing();
            mainGrid.Children[0].Visibility = Visibility.Visible;
            if(mainGrid.Children[0].GetType() == typeof(HomeView)) { AppState = APP_STATE.HOME; }
            else if(mainGrid.Children.GetType() == typeof(DetailsView)) { AppState = APP_STATE.DETAILS_VIEW; }
            mainGrid.Children.RemoveAt(1);
        }


        private void SortOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = ((TextBlock)TopBar.SortOption.SelectedItem).Text;
            switch (selection)
            {
                case "Name":
                    animeDisplayView.SortDescriptions.Clear();
                    animeDisplayView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    break;
                case "Rating":
                    animeDisplayView.SortDescriptions.Clear();
                    animeDisplayView.SortDescriptions.Add(new SortDescription("Rating", ListSortDirection.Descending));
                    animeDisplayView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    break;              
            }
            
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

        public void PlayAnime(long animeId, int EpisodeIndex = -1, bool isContinue = false)
        {
            foreach(Anime anime in MyAnimeCollection.ListOfAnime)
            {
                if (anime.Id == animeId)
                {
                    Player.PlayAnime(anime, EpisodeIndex, isContinue);
                    //mainGrid.Children.RemoveAt(0);
                    mainGrid.Children[0].Visibility = Visibility.Collapsed;
                    mainGrid.Children.Insert(1, Player);
                    AppState = APP_STATE.PLAYING;
                }
            }
        }

        public void OpenDetails()
        {
            Anime selected = (Anime)HomeViewObject.anime_list.SelectedItem;
            DetailsView details = new DetailsView();
            details.BackButton.Click += BackButton_Click;
            details.DataContext = selected;
            details.EpisodesList.DataContext = selected.ListOfEpisodes;
            details.EpisodesList.ItemsSource = selected.ListOfEpisodes;

            mainGrid.Children.RemoveAt(0);
            mainGrid.Children.Insert(0, details);
            AppState = APP_STATE.DETAILS_VIEW;
        }

        public void CheckInternet()
        {
            statusDisplay.DisplayImagePath = StatusDisplay.StatusType.LOADING;
            statusDisplay.DisplayText = "Checking Internet Connection...";
            try
            {
                OverLayer.Children.Add(statusDisplay);
            }
            catch(ArgumentException e)
            {
                return;
            }

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

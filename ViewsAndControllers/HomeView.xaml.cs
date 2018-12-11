using culTAKU.Models;
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
using System.Windows.Forms;
using System.Threading;

using System.Windows.Shapes;
using System.ComponentModel;

namespace culTAKU.ViewsAndControllers
{
    /// <summary>
    /// Interaction logic for DefaultView.xaml
    /// </summary>
    public partial class HomeView : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        private AnimeCollection MyAnimeCollection;
        private int fetchedCount;
        public int FetchedCount { get { return fetchedCount; } set { fetchedCount = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FetchedCount")); } }
        private object fetchCountLock;
        private bool isFetching;
        public HomeView()
        {
            InitializeComponent();
            
        }

        public HomeView(AnimeCollection animeCollection)
        {
            MyAnimeCollection = animeCollection;
            InitializeComponent();
            if(MyAnimeCollection == null || MyAnimeCollection.AllAnimePaths.Count < 1)
            {
                
                selectFolderView.Visibility = Visibility.Visible;
            }

            DataContext = MyAnimeCollection.ListOfAnime;
            anime_list.ItemsSource = MyAnimeCollection.ListOfAnime;
            fetchedCount = 0;
            fetchCountLock = new object();
            isFetching = false;

            Misc.Miscelleneous.MainWindow.TopBar.FetchAllDetailsButton.Click += FetchAllDetailsButton_Click;
        }

        private void FetchAllDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            FetchAllAnime();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void FetchAllAnime()
        {
            if (isFetching) return;
            isFetching = true;
            Misc.Miscelleneous.MainWindow.PropertyChanged += InternetStateChanged;
            Misc.Miscelleneous.MainWindow.CheckInternet();
        }

        public void InternetStateChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsConnected")
            {
                Misc.Miscelleneous.MainWindow.PropertyChanged -= InternetStateChanged;
                if (Misc.Miscelleneous.MainWindow.IsConnected)
                {
                    Misc.Miscelleneous.MainWindow.statusDisplay.DisplayText = "Fetching Anime Details...";
                    Misc.Miscelleneous.MainWindow.statusDisplay.DisplayImagePath = Extras.StatusDisplay.StatusType.LOADING;
                    PropertyChanged += HomeView_PropertyChanged;
                    
                    foreach(Anime anime in MyAnimeCollection.ListOfAnime)
                    {
                        if (anime.DetailsFetched && anime.ImageFetched)
                        {
                            lock (fetchCountLock)
                            {
                                FetchedCount += 1;
                                continue;
                            }
                        }
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            anime.FetchDetails();
                            Dispatcher.Invoke(new Action(() =>
                            {
                                lock (fetchCountLock)
                                {
                                    FetchedCount += 1;
                                }
                            }));
                        });
                    }
                }

                else
                {
                    Misc.Miscelleneous.MainWindow.statusDisplay.DisplayText = "Connection Error. Unable to fetch Anime details";
                    Misc.Miscelleneous.MainWindow.statusDisplay.DisplayImagePath = Extras.StatusDisplay.StatusType.ERROR;
                    isFetching = false;
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Thread.Sleep(5000);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Misc.Miscelleneous.MainWindow.OverLayer.Children.Remove(Misc.Miscelleneous.MainWindow.statusDisplay);
                        }));
                    });
                }
            }
        }

        private void HomeView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "FetchedCount")
            {
                Misc.Miscelleneous.MainWindow.statusDisplay.DisplayText = string.Format("Fetching Anime Details... {0}/{1}", FetchedCount, Misc.Miscelleneous.MainWindow.MyAnimeCollection.ListOfAnime.Count);
                if(FetchedCount == Misc.Miscelleneous.MainWindow.MyAnimeCollection.ListOfAnime.Count)
                {
                    PropertyChanged -= HomeView_PropertyChanged;
                    FetchedCount = 0;
                    isFetching = false;
                    Misc.Miscelleneous.MainWindow.statusDisplay.DisplayText = "Done";
                    Misc.Miscelleneous.MainWindow.statusDisplay.DisplayImagePath = Extras.StatusDisplay.StatusType.CAUTION;
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Thread.Sleep(5000);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Misc.Miscelleneous.MainWindow.OverLayer.Children.Remove(Misc.Miscelleneous.MainWindow.statusDisplay);
                        }));
                    });
                }
            }
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MyAnimeCollection.AddPath(dialog.SelectedPath);
                    FetchAllAnime();
                    selectFolderView.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}

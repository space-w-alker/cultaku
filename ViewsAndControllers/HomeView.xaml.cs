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
    public partial class HomeView : System.Windows.Controls.UserControl
    {
        private AnimeCollection MyAnimeCollection;

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
        }

        public void FetchAllAnime()
        {
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
                    
                    foreach(Anime anime in MyAnimeCollection.ListOfAnime)
                    {
                        if (anime.ImageFetched) continue;
                        anime.FetchDetailsAsync();
                    }
                }

                else
                {
                    Misc.Miscelleneous.MainWindow.statusDisplay.DisplayText = "Connection Error. Unable to fetch Anime details";
                    Misc.Miscelleneous.MainWindow.statusDisplay.DisplayImagePath = Extras.StatusDisplay.StatusType.ERROR;
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

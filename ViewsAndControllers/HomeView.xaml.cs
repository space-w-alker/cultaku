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

using System.Windows.Shapes;

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

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MyAnimeCollection.AddPath(dialog.SelectedPath);
                    selectFolderView.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}

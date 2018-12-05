using culTAKU.Models;
using culTAKU.ViewsAndControllers;
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

namespace culTAKU
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal AnimeCollection MyAnimeCollection;
        
        public MainWindow()
        {
            MyAnimeCollection = CollectionHandler.LoadMyCollectionFile();
            if(MyAnimeCollection == null)
            {
                MyAnimeCollection = new AnimeCollection();
            }

            InitializeComponent();
            Misc.Miscelleneous.MainWindow = this;
            mainGrid.Children.Insert(0,new HomeView(MyAnimeCollection));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CollectionHandler.SaveMyCollectionFile(MyAnimeCollection);
        }
    }
}

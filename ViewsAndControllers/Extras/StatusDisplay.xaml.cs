using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace culTAKU.ViewsAndControllers.Extras
{
    /// <summary>
    /// Interaction logic for StatusDisplay.xaml
    /// </summary>
    public partial class StatusDisplay : UserControl, INotifyPropertyChanged
    {
        public enum StatusType { CAUTION, ERROR, LOADING}
        private string displayText;
        private Storyboard RotateLoadingImage;

        public event PropertyChangedEventHandler PropertyChanged;


        public string DisplayText
        {
            get { return displayText; }
            set
            {
                displayText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayText"));
            }
        }

        public StatusType DisplayImagePath
        {
            set
            {

                string full = System.IO.Path.GetFullPath(String.Format("Icons/{0}.png",value.ToString().ToLower()));

                if(full == System.IO.Path.GetFullPath("Icons/loading.png"))
                {
                    RotateLoadingImage.Begin(image, true);
                }
                else
                {
                    int m = 1;
                    RotateLoadingImage.Remove(image);
                    RotateLoadingImage.Stop(image);
                    
                }


                image.Source = new BitmapImage(new Uri(full, UriKind.Absolute));
            }
        }
       

        public StatusDisplay()
        {
            
            InitializeComponent();
            RotateLoadingImage = (Storyboard)Resources["RotateLoadingImage"];
            DisplayText = "Loading...";
            DisplayImagePath = StatusType.LOADING;
        }
    }
}

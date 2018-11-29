using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace culTAKU
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //var si = GetContentStream(new Uri("./Styles/blue_skin.xaml",UriKind.Relative));
            //var rd = (ResourceDictionary)XamlReader.Load(si.Stream);
            //Current.Resources.MergedDictionaries.Add(rd);
        }
    }
}

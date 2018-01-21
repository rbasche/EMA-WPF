using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

//using IO.Swagger.Api;
//using IO.Swagger.Client;
//using IO.Swagger.Model;
using ESISharp;
using ESISharp.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EMA_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private SearchApi searchApi;
        //private UniverseApi universeApi;
        //private ESIEve.Public publicEve;
        //private SearchApi searchApi;
        //private UniverseApi universeApi;
        //private ESIEve.Public publicEve;
        //static ESIEve.Public publicEve = new ESIEve.Public();
        static public EMAName purchaseStationName, sellStationName;
        static public EMARegion purchaseRegion, sellRegion;

        //public static ESIEve.Public PublicEve { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //searchApi = new SearchApi();
            //universeApi = new UniverseApi();
            //PublicEve = new ESIEve.Public();
        }

        private void ItemTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (purchaseStationName != null)
            {
                purchaseItemGrid.stationIDTextBlock.Text = purchaseStationName.Id.ToString();
                purchaseItemGrid.stationNameTextBlock.Text = purchaseStationName.Name;
            }

            if (purchaseRegion != null)
            {
                purchaseItemGrid.regionIDTextBlock.Text = purchaseRegion.Region_id.ToString();
                purchaseItemGrid.regionNameTextBlock.Text = purchaseRegion.Name;

            }

            if (sellStationName != null)
            {
                sellItemGrid.stationIDTextBlock.Text = sellStationName.Id.ToString();
                sellItemGrid.stationNameTextBlock.Text = sellStationName.Name;
            }
            if (sellRegion != null)
            {
                sellItemGrid.regionIDTextBlock.Text = sellRegion.Region_id.ToString();
                sellItemGrid.regionNameTextBlock.Text = sellRegion.Name;
            }
        }
    }
}
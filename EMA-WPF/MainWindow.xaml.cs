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
        private EMA ema;
        public MainWindow()
        {
            InitializeComponent();
            ema = EMA.Instance;
        }

        private void ItemTab_GotFocus(object sender, RoutedEventArgs e)
        {
            itemGrid.purchaseStationIDTextBlock.Text = ema.PurchaseStation.Station_id.ToString();
            itemGrid.purchaseStationNameTextBlock.Text = ema.PurchaseStation.Station_name;
            itemGrid.purchaseRegionIDTextBlock.Text = ema.PurchaseStation.Region_id.ToString();
            itemGrid.purchaseRegionNameTextBlock.Text = ema.PurchaseStation.Region_name;

            itemGrid.sellStationIDTextBlock.Text = ema.SellStation.Station_id.ToString();
            itemGrid.sellStationNameTextBlock.Text = ema.SellStation.Station_name;
            itemGrid.sellRegionIDTextBlock.Text = ema.SellStation.Region_id.ToString();
            itemGrid.sellRegionNameTextBlock.Text = ema.SellStation.Region_name;
        }


    }
}
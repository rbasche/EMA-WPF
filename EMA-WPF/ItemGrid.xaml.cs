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
using ESISharp;
using ESISharp.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace EMA_WPF
{
    /// <summary>
    /// Interaction logic for ItemGrid.xaml
    /// </summary>
    public partial class ItemGrid : UserControl
    {
        private ObservableCollection<EMAOrder> emaOrderList;
        private ESIEve.Public publicEve;

        public ItemGrid()
        {
            InitializeComponent();
            publicEve = new ESIEve.Public();

        }

        private void GetOrderButton_Click(object sender, RoutedEventArgs e)
        {
            emaOrderList = new ObservableCollection<EMAOrder>();
            itemListBox.ItemsSource = emaOrderList;

            switch (this.Name)
            {
                case "purchaseItemGrid":
                    emaOrderList = GetOrders(MainWindow.purchaseRegion,MainWindow.purchaseStationName.Id);
                    break;
                case "sellItemGrid":
                    emaOrderList = GetOrders(MainWindow.sellRegion, MainWindow.sellStationName.Id);
                    break;
                default:
                    break;
            }
            itemListBox.ItemsSource = emaOrderList;
            this.statusTextBlock.Text = String.Format("finished: {0} orders", emaOrderList.Count);
        }

        private ObservableCollection<EMAOrder> GetOrders(EMARegion region, int stationId)
        {
            ObservableCollection<EMAOrder> orderCollection;
            EsiResponse esiResponse;
 
            orderCollection = new ObservableCollection<EMAOrder>();
            if (region != null)
            {
                esiResponse = publicEve.Market.GetRegionOrders(region.Region_id,null,MarketOrderType.Sell,1).Execute();
                int pages = esiResponse.Headers.Pages;
                for (int page = 1; page < pages; page++)
                {
                    AddPageToCollection(orderCollection,esiResponse,stationId);
                    esiResponse = publicEve.Market.GetRegionOrders(region.Region_id, null, MarketOrderType.Sell, page+1).Execute();
                }
                AddPageToCollection(orderCollection, esiResponse, stationId);
            }
            return orderCollection;
        }

        private void AddPageToCollection(ObservableCollection<EMAOrder> collection, EsiResponse response, int id)
        {
            List<EMAOrder> page = JsonConvert.DeserializeObject<List<EMAOrder>>(response.Body);
            foreach (EMAOrder element in page)
            {
                if (element.Location_id == id)
                {
                    collection.Add(element);
                }
            }
            itemListBox.ItemsSource = collection;
            this.statusTextBlock.Text = String.Format("working: {0} orders", collection.Count);


        }
    }
}

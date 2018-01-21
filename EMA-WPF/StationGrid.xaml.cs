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
    /// Interaction logic for Stationsgrid.xaml
    /// </summary>
    public partial class StationGrid : UserControl
    {
        private ObservableCollection<EMAName> emaNameList;
        private ESIEve.Public publicEve;

        public StationGrid()
        {
            InitializeComponent();
            publicEve = new ESIEve.Public();

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(searchTextBox.Text))
            {
                statusTextBlock.Text = "Search Field is empty";
                return;
            }

            EsiResponse searchResponse = publicEve.Search.SearchPublic(searchTextBox.Text, SearchCategory.Station).Execute();

            statusTextBlock.Text = String.Format("Search request finished with status code {0}", searchResponse.Code);
            if (searchResponse.Code != System.Net.HttpStatusCode.OK)
            {
                return;
            }

            EMASearch emaSearch = new EMASearch();
            emaSearch = JsonConvert.DeserializeObject<EMASearch>(searchResponse.Body);

            if (emaSearch.Station == null)
            {
                statusTextBlock.Text += ", no stations found";
                return;
            }
            statusTextBlock.Text += String.Format(", {0} stations found", emaSearch.Station.Count);

            EsiResponse namesResponse = publicEve.Universe.GetTypeNamesAndCategories(emaSearch.Station).Execute();

            //statusTextBlock.Text = String.Format("Name lookup request finished with status code {0}", namesResponse.Code);
            if (namesResponse.Code != System.Net.HttpStatusCode.OK)
            {
                return;
            }

            emaNameList = new ObservableCollection<EMAName>();
            emaNameList = JsonConvert.DeserializeObject<ObservableCollection<EMAName>>(namesResponse.Body);
            stationListBox.ItemsSource = emaNameList;
            statusTextBlock.Text += String.Format(", for {0} stations", emaNameList.Count);
        }

        private void StationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (stationListBox.SelectedItem != null)
            {
                switch (this.Name)
                {
                    case "purchaseStationGrid":
                        MainWindow.purchaseStationName = ((EMAName)stationListBox.SelectedItem);
                        MainWindow.purchaseRegion = GetRegion(MainWindow.purchaseStationName);                        
                        break;
                    case "sellStationGrid":
                        MainWindow.sellStationName = ((EMAName)stationListBox.SelectedItem);
                        MainWindow.sellRegion = GetRegion(MainWindow.sellStationName);
                        break;
                    default:
                        break;
                }
                selectionTextBlock.Text = ((EMAName)stationListBox.SelectedItem).Name;
            }
        }

        private EMARegion GetRegion(EMAName stationName)
        {
            EsiResponse esiResponse;
            EMAStation emaStation;
            EMASystem emaSystem;
            EMAConstellation emaConstellation;
            EMARegion emaRegion;

            esiResponse = publicEve.Universe.GetStationInfo(stationName.Id).Execute();
            emaStation = JsonConvert.DeserializeObject<EMAStation>(esiResponse.Body);
            esiResponse = publicEve.Universe.GetSystemInfo(emaStation.System_id).Execute();
            emaSystem = JsonConvert.DeserializeObject<EMASystem>(esiResponse.Body);
            esiResponse = publicEve.Universe.GetConstellationInfo(emaSystem.Constellation_id).Execute();
            emaConstellation = JsonConvert.DeserializeObject<EMAConstellation>(esiResponse.Body);
            esiResponse = publicEve.Universe.GetRegionInfo(emaConstellation.Region_id).Execute();
            emaRegion = JsonConvert.DeserializeObject<EMARegion>(esiResponse.Body);

            return emaRegion;

        }

    }
}

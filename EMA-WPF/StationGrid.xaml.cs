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
        private EMASearch emaSearch;
        private ObservableCollection<EMAName> emaNameList;
        private ESIEve.Public publicEve;

        public StationGrid()
        {
           publicEve = new ESIEve.Public();
           InitializeComponent();
 
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(searchTextBox.Text))
            {
                statusTextBlock.Text = "Search Field is empty";
                return;
            }

            emaSearch = EmaSearch(searchTextBox.Text, SearchCategory.Station);
 
            if (emaSearch == null)
            {
                statusTextBlock.Text = "Error: No stations found";
                return;
            }
            statusTextBlock.Text = String.Format("Search: {0} stations found", emaSearch.Station.Count);

            emaNameList = GetEMANames(emaSearch.Station);
            if (emaNameList == null)
            {
                statusTextBlock.Text = "Error: Name lookup failed";
                return;
            }
            stationListBox.ItemsSource = emaNameList;
            statusTextBlock.Text = String.Format("Name lookup: {0} stations", emaNameList.Count);
        }

        private void StationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (stationListBox.SelectedItem != null)
            {
                switch (this.Name)
                {
                    case "purchaseStationGrid":
                        MainWindow.purchaseStationName = ((EMAName)stationListBox.SelectedItem);
                        MainWindow.purchaseRegion = GetEMARegion(MainWindow.purchaseStationName);                        
                        break;
                    case "sellStationGrid":
                        MainWindow.sellStationName = ((EMAName)stationListBox.SelectedItem);
                        MainWindow.sellRegion = GetEMARegion(MainWindow.sellStationName);
                        break;
                    default:
                        break;
                }
                selectionTextBlock.Text = ((EMAName)stationListBox.SelectedItem).Name;
            }
        }

        private EMASearch EmaSearch(string item, SearchCategory category)
        {
            EsiResponse response = publicEve.Search.SearchPublic(item, category).Execute();
            if (response.Code != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            EMASearch search = new EMASearch();
            search = JsonConvert.DeserializeObject<EMASearch>(response.Body);

            return search;
        }

        private ObservableCollection<EMAName> GetEMANames(List<int> ids)
        {
            EsiResponse response = publicEve.Universe.GetTypeNamesAndCategories(ids).Execute();
            if (response.Code != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            ObservableCollection<EMAName> collection = new ObservableCollection<EMAName>();
            collection = JsonConvert.DeserializeObject<ObservableCollection<EMAName>>(response.Body);
            return collection;
        }

        private EMARegion GetEMARegion(EMAName stationName)
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

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
//using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
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
        private ESIEve.Public publicEve;

        public MainWindow()
        {
            InitializeComponent();

            //searchApi = new SearchApi();
            //universeApi = new UniverseApi();
            publicEve = new ESIEve.Public();

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            
            testTextBlock.Text = "";

            if (String.IsNullOrEmpty(searchTextBox.Text))
            {
                statusTextBlock.Text = "Search Field is empty";
                return;
            }

            /*
           List<string> searchCategories = new List<string>();
           searchCategories.Add("station");

           GetSearchOk stationsList = searchApi.GetSearch(searchCategories, searchTextBox.Text);

           if (stationsList.Station == null)
           {
           }

           int? stationID = stationsList.Station[0];
           string stationName = universeApi.GetUniverseStationsStationId(stationID).Name;
           testTextBlock.Text += stationID + " " + stationName;

           for (int i = 1; i < stationsList.Station.Count; i++)
           {
               stationID = stationsList.Station[i];
               stationName = universeApi.GetUniverseStationsStationId(stationID).Name;
               testTextBlock.Text += "\n" + stationID + " " + stationName;
           }

           */
            EsiResponse searchResponse = publicEve.Search.SearchPublic(searchTextBox.Text, SearchCategory.Station).Execute();

           statusTextBlock.Text = String.Format("Search request finished with status code {0}", searchResponse.Code);
           if (searchResponse.Code != System.Net.HttpStatusCode.OK)
            {
                return;
            }

            EMASearch emaSearch = new EMASearch();
            emaSearch = JsonConvert.DeserializeObject<EMASearch>(searchResponse.Body);

            if (emaSearch.station == null)
            {
                statusTextBlock.Text += ", no stations found";
                return;
            }
            statusTextBlock.Text += String.Format(", {0} stations found", emaSearch.station.Count);

            EsiResponse stationResponse = publicEve.Universe.GetStationInfo(emaSearch.station[0]).Execute();

            EMAStation emaStation = new EMAStation();
            emaStation = JsonConvert.DeserializeObject<EMAStation>(stationResponse.Body);

            testTextBlock.Text += emaSearch.station[0] + " " + emaStation.name;

            for (int i = 1; i < emaSearch.station.Count; i++)
            {
                stationResponse = publicEve.Universe.GetStationInfo(emaSearch.station[i]).Execute();
                emaStation = JsonConvert.DeserializeObject<EMAStation>(stationResponse.Body);
                testTextBlock.Text += "\n" + emaSearch.station[i] + " " + emaStation.name;
            }
        }
    }

    class EMAPosition
    {
        public float x;
        public float y;
        public float z;
    }
    class EMAStation
    {
        /*"{\"station_id\":60002959,
         * \"name\":\"Jita IV - Moon 10 - Caldari Constructions Production Plant\",
         * \"type_id\":1529,
         * \"position\":{\"x\":-105999360000.0,\"y\":-18516664320.0,\"z\":435947888640.0},
         * \"system_id\":30000142,
         * \"reprocessing_efficiency\":0.5,
         * \"reprocessing_stations_take\":0.05,
         * \"max_dockable_ship_volume\":50000000.0,
         * \"office_rental_cost\":4595833.0,
         * \"services\":
         *  [\"bounty-missions\",
         *   \"courier-missions\",
         *   \"reprocessing-plant\",
         *   \"market\",
         *   \"repair-facilities\",
         *   \"factory\",\"fitting\",
         *   \"news\",
         *   \"storage\",
         *   \"insurance\",
         *   \"docking\",
         *   \"office-rental\",
         *   \"loyalty-point-store\",
         *   \"navy-offices\"],
         * \"owner\":1000026,
         * \"race_id\":1}" 
        */
        public int station_id;
        public string name;
        public int type_id;
        public EMAPosition position;
        public int system_id;
        public decimal reprocessing_efficiency;
        public decimal reprocessing_stations_take;
        public float max_dockable_ship_volume;
        public decimal office_rental_cost;
        public List<string> services;
        public int owner;
        public int race_id;
    }
    class EMASearch
    {
        public List<int> agent;
        public List<int> alliance;
        public List<int> character;
        public List<int> constellation;
        public List<int> corporation;
        public List<int> faction;
        public List<int> inventory_type;
        public List<int> region;
        public List<int> solar_system;
        public List<int> station;
    }
}

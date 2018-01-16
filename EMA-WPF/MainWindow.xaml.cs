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
                testTextBlock.Text = "Search Field is empty";
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

            if (searchResponse.Code != System.Net.HttpStatusCode.OK)
            {
                testTextBlock.Text = "Status is not OK";
                return;
            }
            testTextBlock.Text = searchResponse.Body + "\n";
            //"{\"station\":[60002959,60003757,60003760,60003460,60002953,60003463,60003469,61000676,60004423,60000361,60000364,61000133,61000088,60003055,60000451,60000463,60003466]}"
            JObject searchBody = JObject.Parse(searchResponse.Body);
            JArray idArray = (JArray)searchBody["station"];
            if (idArray == null)
            {
                testTextBlock.Text = "No stations returned";
                return;
            }
            int[] stationIds = idArray.Select(c => (int)c).ToArray();

            int stationID = stationIds[0];
            EsiResponse stationResponse = publicEve.Universe.GetStationInfo(stationID).Execute();
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
             * \"race_id\":1}" */
            string stationName = ExtractName(stationResponse.Body);
            testTextBlock.Text += stationID + " " + stationName;

            for (int i = 1; i < stationIds.Length; i++)
            {
                stationID = stationIds[i];
                stationResponse = publicEve.Universe.GetStationInfo(stationID).Execute();
                stationName = ExtractName(stationResponse.Body);
                testTextBlock.Text += "\n" + stationID + " " + stationName;
            }
        }

        private string ExtractName(string body)
        {
            JObject jBody = JObject.Parse(body);
            string name = (string)jBody.SelectToken("name");
            return name;
        }
    }
}

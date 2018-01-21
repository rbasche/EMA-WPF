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
        private ESIEve.Public publicEve;
        ObservableCollection<EMAName> emaNameList;

        public MainWindow()
        {
            InitializeComponent();

            //searchApi = new SearchApi();
            //universeApi = new UniverseApi();
            publicEve = new ESIEve.Public();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            
            //testTextBlock.Text = "";

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

            statusTextBlock.Text = String.Format("Name lookup request finished with status code {0}", namesResponse.Code);
            if (namesResponse.Code != System.Net.HttpStatusCode.OK)
            {
                return;
            }

            emaNameList = new ObservableCollection<EMAName>() ;
            emaNameList = JsonConvert.DeserializeObject<ObservableCollection<EMAName>>(namesResponse.Body);
            stationListBox.ItemsSource = emaNameList;
            statusTextBlock.Text += String.Format(", for {0} stations", emaNameList.Count);
        }

        private void stationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (stationListBox.SelectedItem != null)
            {
                selectionTextBlock.Text = ((EMAName) stationListBox.SelectedItem).Name;
            }
        }
    }

    class EMAPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
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
        public int Station_id { get; set; }
        public string Name { get; set; }
        public int Type_id { get; set; }
        public EMAPosition position { get; set; }
        public int System_id { get; set; }
        public decimal Reprocessing_efficiency { get; set; }
        public decimal Reprocessing_stations_take { get; set; }
        public float Max_dockable_ship_volume { get; set; }
        public decimal Office_rental_cost { get; set; }
        public List<string> Services { get; set; }
        public int Owner { get; set; }
        public int Race_id { get; set; }
    }
    class EMASearch
    {
        public List<int> Agent { get; set; }
        public List<int> Alliance { get; set; }
        public List<int> Character { get; set; }
        public List<int> Constellation { get; set; }
        public List<int> Corporation { get; set; }
        public List<int> Faction { get; set; }
        public List<int> Inventory_type { get; set; }
        public List<int> Region { get; set; }
        public List<int> Solar_system { get; set; }
        public List<int> Station { get; set; }
    }
    class EMAName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }
}

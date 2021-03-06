﻿using System;
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
        private EveSearch stationSearch;
        private List<EveName> stationNameList;
        private EMA ema;

        public StationGrid()
        {
            InitializeComponent();
            ema = EMA.Instance;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(searchTextBox.Text))
            {
                statusTextBlock.Text = "Search Field is empty";
                return;
            }

            stationSearch = EMA.Search(searchTextBox.Text, SearchCategory.Station);
 
            if (stationSearch == null)
            {
                statusTextBlock.Text = "Error: No stations found";
                return;
            }
            statusTextBlock.Text = String.Format("Search: {0} stations found", stationSearch.Station.Count);

            stationNameList = EMA.GetEveNames(stationSearch.Station);

            if (stationNameList == null)
            {
                statusTextBlock.Text = "Error: Name lookup failed";
                return;
            }
            stationListBox.ItemsSource = stationNameList;
            statusTextBlock.Text = String.Format("Name lookup: {0} stations", stationNameList.Count);
        }

        private void StationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EMAStation station;
            
            if (stationListBox.SelectedItem != null)
            {
                switch (this.Name)
                {
                    case "purchaseStationGrid":
                        station = ema.PurchaseStation;
                        break;
                    case "sellStationGrid":
                        station = ema.SellStation;
                        break;
                    default:
                        station = null;
                        break;
                }
                station.Station_id = ((EveName)stationListBox.SelectedItem).Id;
                station.Station_name = ((EveName)stationListBox.SelectedItem).Name;
                station.Region_id = ema.GetEveRegion((EveName)stationListBox.SelectedItem).Region_id;
                station.Region_name = ema.GetEveRegion((EveName)stationListBox.SelectedItem).Name;
                selectionTextBlock.Text = ((EveName)stationListBox.SelectedItem).Name;
            }
        }

    }
}

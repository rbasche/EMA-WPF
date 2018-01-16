﻿using System;
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
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace EMA_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SearchApi searchApi;
        private UniverseApi universeApi;

        public MainWindow()
        {
            InitializeComponent();

            searchApi = new SearchApi();
            universeApi = new UniverseApi();

        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {

            testTextBlock.Text = "";

            List<string> searchCategories = new List<string>();
            searchCategories.Add("station");

            if (String.IsNullOrEmpty(searchTextBox.Text))
            {
                testTextBlock.Text = "Search Field is empty";
                return;
            }

            GetSearchOk stationsList = searchApi.GetSearch(searchCategories, searchTextBox.Text);

            if (stationsList.Station == null)
            {
                testTextBlock.Text = "No stations returned";
                return;
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

            
        }
    }
}
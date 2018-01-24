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
        private EMA ema;

        public ItemGrid()
        {
            InitializeComponent();
            ema = EMA.Instance;

        }

        private async void GetItemButton_Click(object sender, RoutedEventArgs e)
        {
            itemListView.ItemsSource = ema.SellItems;
            var progressHandler = new Progress<string>(value =>
            {
                this.statusTextBlock.Text = String.Format("Get Item: {0}",value);
            });
            var progress = progressHandler as IProgress<string>;

            this.statusTextBlock.Text = "starting: Get Items";
            TimeSpan elapsed = await Task<TimeSpan>.Run(() =>
            {               
                return ema.GetItemNamesForRegions(progress);
            });
            this.statusTextBlock.Text = String.Format("finished: Get Items, {0}",elapsed);
        }

        private async void GetOrderButton_Click(object sender, RoutedEventArgs e)
        {
            itemListView.ItemsSource = ema.SellItems;
            var progressHandler = new Progress<string>(value =>
            {
                this.statusTextBlock.Text = String.Format("Get Item: {0}", value);
                this.itemListView.ItemsSource = ema.SellItems;
            });
            var progress = progressHandler as IProgress<string>;

            this.statusTextBlock.Text = "starting: Get Orders";
            TimeSpan elapsed = await Task<TimeSpan>.Run(() =>
            {
                return ema.GetSellItems(progress);
            });
            this.statusTextBlock.Text = String.Format("finished: Get Orders, {0}", elapsed);
            itemListView.ItemsSource = ema.SellItems;
        }

    }
}

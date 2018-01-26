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
        private ObservableCollection<EMASellItem> mySellItems;

        public ItemGrid()
        {
            InitializeComponent();
            ema = EMA.Instance;
            mySellItems = new ObservableCollection<EMASellItem>();

        }

        private async void GetItemButton_Click(object sender, RoutedEventArgs e)
        {
            itemButton.IsEnabled = false;
            orderButton.IsEnabled = false;
            itemListView.ItemsSource = mySellItems;
            var progressHandler = new Progress<string>(value =>
            {
                this.statusTextBlock.Text = String.Format("Get Item: {0}",value);
            });
            var progress = progressHandler as IProgress<string>;

            this.statusTextBlock.Text = "starting: Get Items";
            string message = await Task<TimeSpan>.Run(() =>
            {
                try
                {
                    return ema.GetItemNamesForRegions(progress);
                }
                catch (Exception ex)
                {
                    String exceptionMessage = String.Format(" -->Exception {0}<--", ex.Message);
                    return exceptionMessage;
                }
            });
            this.statusTextBlock.Text += message;
            itemButton.IsEnabled = true;
            orderButton.IsEnabled = true;
        }

        private async void GetOrderButton_Click(object sender, RoutedEventArgs e)
        {
            orderButton.IsEnabled = false;
            itemButton.IsEnabled = false;
            itemListView.ItemsSource = mySellItems;
            var progressHandler = new Progress<EMAProgress>(value =>
            {
                this.statusTextBlock.Text = String.Format("Get Orders: sell item {0}", value.Message);
                //mySellItems.Add(ema.SellItems.Last());
                //mySellItems.Clear();
                if (value.IsNewItem)
                {
                    mySellItems.Add(value.Item);
                }
            });
            var progress = progressHandler as IProgress<EMAProgress>;

            this.statusTextBlock.Text = "starting: Get Orders";
            string message = await Task<TimeSpan>.Run(() =>
            {
                try
                {
                    return ema.GetSellItems(progress);
                    //return ema.GetSellItemsByName(progress);
                }
                catch (Exception ex)
                {
                    string exceptionMessage = String.Format(" --> Exception {0}< --", ex.Message);
                    return exceptionMessage;
                }
            });
            
            foreach (EMASellItem item in ema.SellItems)
            {
                if (!mySellItems.Contains(item)) mySellItems.Add(item);
            }
            this.statusTextBlock.Text += message;
            orderButton.IsEnabled = true;
            itemButton.IsEnabled = true;
        }

    }
}

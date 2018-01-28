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
        private ObservableCollection<EMAHistory> myHistory;

        public ItemGrid()
        {
            InitializeComponent();
            ema = EMA.Instance;
            mySellItems = new ObservableCollection<EMASellItem>();
            historyButton.IsEnabled = false;

        }

        private async void GetOrderButton_Click(object sender, RoutedEventArgs e)
        {
            orderButton.IsEnabled = false;
            historyButton.IsEnabled = false;
            itemListView.ItemsSource = mySellItems;
            var progressHandler1 = new Progress<string>(value =>
            {
                this.statusTextBlock.Text = String.Format("Get Item: {0}", value);
            });
            var progress1 = progressHandler1 as IProgress<string>;

            this.statusTextBlock.Text = "starting: Get Items";
            string message = await Task<TimeSpan>.Run(() =>
            {
                try
                {
                    return ema.GetItemNamesForRegions(progress1);
                }
                catch (Exception ex)
                {
                    String exceptionMessage = String.Format(" -->Exception {0}<--", ex.Message);
                    return exceptionMessage;
                }
            });
            this.statusTextBlock.Text += message;
            var progressHandler2 = new Progress<EMAProgress>(value =>
            {
                this.statusTextBlock.Text = String.Format("Get Orders: sell item {0}", value.Message);
                //mySellItems.Add(ema.SellItems.Last());
                //mySellItems.Clear();
                if (value.IsNewItem)
                {
                    mySellItems.Add(value.Item);
                }
            });
            var progress2 = progressHandler2 as IProgress<EMAProgress>;

            this.statusTextBlock.Text = "starting: Get Orders";
            message = await Task<TimeSpan>.Run(() =>
            {
                try
                {
                    return ema.GetSellItems(progress2);
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
            historyButton.IsEnabled = true;
        }

        private void GetHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            orderButton.IsEnabled = false;
            historyButton.IsEnabled = false;

            ema.EMAGetHistory(mySellItems);

            historyButton.IsEnabled = true;
            orderButton.IsEnabled = true;
        }
    }
}

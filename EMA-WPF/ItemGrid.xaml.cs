using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Windows;


namespace EMA_WPF
{
    /// <summary>
    /// Interaction logic for ItemGrid.xaml
    /// </summary>
    public partial class ItemGrid : UserControl
    {
        private EMA ema;
        private ObservableCollection<EMASellItem> mySellItems;
        private ObservableCollection<EMAHistoryItem> myHistory;

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
            mySellItems.Clear();
            itemListView.ItemsSource = mySellItems;

            Progress<EMAProgressInfo> emaProgressHandler = new Progress<EMAProgressInfo>(info =>
            {
                this.statusTextBlock.Text = info.Message;
                if (info.IsNewItem)
                {
                    mySellItems.Add(info.Item);
                }
            });
            ema.Progress = emaProgressHandler as IProgress<EMAProgressInfo>;

            this.statusTextBlock.Text = "starting: Get Items";
            string message = await Task<TimeSpan>.Run( () =>
            {
                try
                {
                    return ema.GetItemNamesForRegions();
                }
                catch (Exception ex)
                {
                    exceptionLog(ex);
                    string exceptionMessage = String.Format(" --> Exception {0}< --", ex.Message);
                    return exceptionMessage;
                }
            } );
            this.statusTextBlock.Text += message;


            this.statusTextBlock.Text = "starting: Get Orders";
            message = await Task<TimeSpan>.Run(() =>
            {
                try
                {
                    return ema.GetSellItems();
                }
                catch (Exception ex)
                {
                    exceptionLog(ex);

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

            void exceptionLog(Exception exception)
            {
                //string logFile = System.AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + ".exception.log";
                //string logFile = System.AppDomain.CurrentDomain.BaseDirectory + "Exception.log";

                using (FileStream logStream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\EMA\Exception.log", FileMode.Append))
                using (StreamWriter logWriter = new StreamWriter(logStream))
                { 
                    logWriter.WriteLine(String.Format("{0}: {1}", DateTime.Now, exception.Message));
                    logWriter.WriteLine(exception.StackTrace);
                    foreach (var item in exception.Data)
                    {
                        logWriter.WriteLine(item.ToString());
                    }
                    logWriter.Flush();
                }

            }
        }

        private void GetHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            orderButton.IsEnabled = false;
            historyButton.IsEnabled = false;

            ema.GetHistory();
            //mySellItems.Clear();

            List<EMASellItem> selectedItems = ema.SellItems.FindAll(ema.FindSelectedItemsHelper);
            foreach (EMASellItem item in selectedItems)
            {
                int index = mySellItems.IndexOf(item);

                mySellItems[index].PurchaseHistory = item.PurchaseHistory;
                mySellItems[index].Purchase_price = item.Purchase_price;
                mySellItems[index].SellHistory = item.SellHistory;
                mySellItems[index].Sell_price = item.Sell_price;

            }

            historyButton.IsEnabled = true;
            orderButton.IsEnabled = true;


        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            EMASellItem item = (EMASellItem)((CheckBox)sender).DataContext;
            item.IsSelected = true;
            EMASellItem originItem = ema.SellItems.Find(FindSellItemHelper);
            originItem.IsSelected = true;

            bool FindSellItemHelper(EMASellItem sellItem)
            {
                return sellItem.Type_id == item.Type_id;
            }
        }
    }
}

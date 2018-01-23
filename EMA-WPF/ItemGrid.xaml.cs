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

        private void GetItemButton_Click(object sender, RoutedEventArgs e)
        {
            List<EveName> itemNames = new List<EveName>();
            DateTime start = DateTime.Now;
            ema.GetItemNamesForRegions();
            TimeSpan elapsed = DateTime.Now - start;

            this.statusTextBlock.Text = String.Format("finished: {0} items, time elapsed {1}", itemNames.Count, elapsed);
        }

        private void GetOrderButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = DateTime.Now;
            ema.GetSellItems();
            TimeSpan elapsed = DateTime.Now - start;
            itemListBox.ItemsSource = ema.SellItems;

            this.statusTextBlock.Text = String.Format("finished: {0} items, time elapsed {1}", ema.SellItems.Count, elapsed);


        }
    }
}

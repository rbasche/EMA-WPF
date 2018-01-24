using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESISharp;
using ESISharp.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace EMA_WPF
{

    public sealed class EMA
    {

        private EMA()
        {
            PublicEve = new ESIEve.Public();
            SellItems = new List<EMASellItem>();
            SellItems.Capacity = 10000;
            sellItems.Capacity = 10000;
            PurchaseStation = new EMAStation();
            SellStation = new EMAStation();
            SellItemNames = new List<EveName>();
            SellItemNames.Capacity = 200000;
            sellItemNames.Capacity = 200000;
        }
        private static readonly Lazy<EMA> lazyEMA = new Lazy<EMA>(() => new EMA());
        public static EMA Instance
        {
            get
            {
                return lazyEMA.Value;
            }
        }

        public EMAStation PurchaseStation { get => purchaseStation; set => purchaseStation = value; }
        private EMAStation purchaseStation;

        public EMAStation SellStation { get => sellStation; set => sellStation = value; }
        private EMAStation sellStation;

        public ESIEve.Public PublicEve { get => publicEve; set => publicEve = value; }
        private ESIEve.Public publicEve;

        public List<EMASellItem> SellItems { get => sellItems; set => sellItems = value; }
        private List<EMASellItem> sellItems;

        public List<EveName> SellItemNames { get => sellItemNames; set => sellItemNames = value; }
        private List<EveName> sellItemNames;

        public static List<EveName> GetEveNames(List<int> ids)
        {
            EMA ema = EMA.Instance;

            EsiResponse response = ema.PublicEve.Universe.GetTypeNamesAndCategories(ids).Execute();
            if (response.Code != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            List<EveName> list = new List<EveName>();
            list = JsonConvert.DeserializeObject<List<EveName>>(response.Body);
            return list;
        }

        public static EveSearch Search(string item, SearchCategory category)
        {
            EMA ema = EMA.Instance;

            EsiResponse response = ema.PublicEve.Search.SearchPublic(item, category).Execute();
            if (response.Code != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            EveSearch search = new EveSearch();
            search = JsonConvert.DeserializeObject<EveSearch>(response.Body);

            return search;
        }

        public EveRegion GetEveRegion(EveName stationName)
        {
            EsiResponse esiResponse;
            EveStation emaStation;
            EveSystem emaSystem;
            EveConstellation emaConstellation;
            EveRegion emaRegion;

            esiResponse = PublicEve.Universe.GetStationInfo(stationName.Id).Execute();
            emaStation = JsonConvert.DeserializeObject<EveStation>(esiResponse.Body);
            esiResponse = PublicEve.Universe.GetSystemInfo(emaStation.System_id).Execute();
            emaSystem = JsonConvert.DeserializeObject<EveSystem>(esiResponse.Body);
            esiResponse = PublicEve.Universe.GetConstellationInfo(emaSystem.Constellation_id).Execute();
            emaConstellation = JsonConvert.DeserializeObject<EveConstellation>(esiResponse.Body);
            esiResponse = PublicEve.Universe.GetRegionInfo(emaConstellation.Region_id).Execute();
            emaRegion = JsonConvert.DeserializeObject<EveRegion>(esiResponse.Body);

            return emaRegion;
        }

        private List<EveOrder> GetEveOrders(EMAStation station,EveName name)
        {
            List<EveOrder> orderList;
            EsiResponse response;

            orderList = new List<EveOrder>();
            if (station != null)
            {
                List<EveOrder> orderPage;
                response = PublicEve.Market.GetRegionOrders(station.Region_id, name.Id, MarketOrderType.Sell).Execute();
                int pages = response.Headers.Pages;
                for (int page = 1; page < pages; page++)
                {
                    orderPage = JsonConvert.DeserializeObject<List<EveOrder>>(response.Body);
                    orderList.AddRange(orderPage);
                    response = PublicEve.Market.GetRegionOrders(station.Region_id, name.Id, MarketOrderType.Sell, page + 1).Execute();
                }
                orderPage = JsonConvert.DeserializeObject<List<EveOrder>>(response.Body);
                orderList.AddRange(orderPage);
            }
            return orderList;
        }

        public TimeSpan GetItemNamesForRegions()
        {
            return GetItemNamesForRegions(null);
        }
        public TimeSpan GetItemNamesForRegions(IProgress<string> progress)
        {
            DateTime start = DateTime.Now;
            List<int> purchaseItemIDs, sellItemIDs;
            SellItemNames.Clear();
            EsiResponse response;

            
            purchaseItemIDs = GetItemIDs(PurchaseStation.Region_id, progress);
            sellItemIDs = GetItemIDs(SellStation.Region_id, progress);

            List<int> itemIDs = new List<int>
            {
                Capacity = 50000
            };

            itemIDs.AddRange(purchaseItemIDs);
            itemIDs = itemIDs.Intersect(sellItemIDs).ToList<int>();
            if (progress != null)
            {
                progress.Report(String.Format("region {0}: {1} ids, region {2}: {3} ids, consolidated {4} ids",
                PurchaseStation.Region_id, purchaseItemIDs.Count.ToString(),
                SellStation.Region_id, sellItemIDs.Count.ToString(),
                itemIDs.Count.ToString()));
            }

            List<int> slice = new List<int>();
            int increment = 1000;
            for (int i = 0; i < itemIDs.Count; i += increment)
            {
                slice = itemIDs.GetRange(i, Math.Min(increment, itemIDs.Count - i));
                response = PublicEve.Universe.GetTypeNamesAndCategories(slice).Execute();
                SellItemNames.AddRange(JsonConvert.DeserializeObject<List<EveName>>(response.Body));
                if (progress != null)
                {
                    progress.Report(String.Format("names: {0}/{1}", SellItemNames.Count.ToString(), itemIDs.Count.ToString()));
                }

            }
            return DateTime.Now - start;
        }

        private List<int> GetItemIDs(int region)
        {
            return GetItemIDs(region, null);
        }
        private List<int> GetItemIDs(int region,IProgress<string> progress)
        {
            EsiResponse response;
            response = PublicEve.Market.GetMarketTypes(region).Execute();
            if (response.Code != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            //List<int> itemIDPage = new List<int>();
            List<int> itemIDs = new List<int>
            {
                Capacity = 100000
            };

            int pages = response.Headers.Pages;
            for (int page = 1; page < pages; page++)
            {
                itemIDs.AddRange(JsonConvert.DeserializeObject<List<int>>(response.Body).Distinct().ToList());
                if (progress != null)
                {
                    progress.Report(String.Format("region {0}: ids {1}", region.ToString(), itemIDs.Count.ToString()));
                }
                response = PublicEve.Market.GetMarketTypes(region, page + 1).Execute();
            }
            itemIDs.AddRange(JsonConvert.DeserializeObject<List<int>>(response.Body).Distinct().ToList());
            itemIDs = itemIDs.Distinct().ToList();

            if (progress != null)
            {
                progress.Report(String.Format("region {0}: ids {1}", region.ToString(), itemIDs.Count.ToString()));
            }
            return itemIDs;
        }

        public TimeSpan GetSellItems()
        {
            return GetSellItems(null);
        }
        public TimeSpan GetSellItems(IProgress<string> progress)
        {
            DateTime start = DateTime.Now;
            List<EveOrder> purchaseOrders, sellOrders;
 
            EMASellItem item;
            int i = 0;
            foreach (EveName name in SellItemNames)
            {
                purchaseOrders = GetEveOrders(PurchaseStation, name);
                if (purchaseOrders.Count == 0)
                {
                    //if the item is not sold in the purchase station skip it
                    continue;
                }
                sellOrders = GetEveOrders(SellStation, name);
                item = FillItem(name, purchaseOrders, sellOrders);
                if (item.Margin < 20)
                {
                    continue;
                }
                SellItems.Add(item);
                progress.Report(String.Format("Item {0}: {1}", item.Type_id.ToString(), item.Name));
            }
            return DateTime.Now - start;
        }

        private EMASellItem FillItem(EveName name, List<EveOrder> pOrders, List<EveOrder> sOrders)
        {
            EMASellItem item = new EMASellItem();
            DateTime now = DateTime.Now;
            TimeSpan[] timeSpan = new TimeSpan[] { TimeSpan.FromHours(1), TimeSpan.FromHours(3), TimeSpan.FromHours(24), TimeSpan.FromDays(3) };

            TimeSpan activeFor = new TimeSpan();
            item.Name = name.Name;
            item.Type_id = name.Id;
            foreach (EveOrder oItem in pOrders)
            {
                item.Purchase_price = Math.Max(item.Purchase_price, oItem.Price);
            }

            foreach (EveOrder sItem in sOrders)
            {
                activeFor = now - sItem.Issued;
                item.Sell_price = Math.Max(item.Sell_price, sItem.Price);
                activeFor = now - sItem.Issued;
                for (int i = 0; i < 4; i++)
                {
                    if (activeFor <= timeSpan[i]) item.Competition[i]++;
                }
            }
            if (item.Sell_price == 0)
            {
                item.Margin = 1000;
            }
            else
            {
                item.Margin = 100*Decimal.Round((item.Sell_price-item.Purchase_price)/item.Sell_price,2);
            }
            
            return item;
        }

    } /* class EMA */

    public class EMAStation
    {
        public int Station_id { get; set; }
        public string Station_name { get; set; }
        public int Region_id { get; set; }
        public string Region_name { get; set; }
    }

    public class EMASellItem
    {
        private int _type_id;
        private string _name;
        private decimal _purchase_price;
        private int _purchase_volume;
        private decimal _sell_price;
        private int _sell_volume;
        private decimal _margin;
        private int[] _competition;

        public EMASellItem()
        {
            _type_id = 0;
            _name = null;
            _purchase_price = 0;
            _purchase_volume = 0;
            _sell_price = 0;
            _sell_volume = 0;
            _margin = 0;
            _competition = new int[] { 0, 0, 0, 0 };
        }

        public int Type_id { get => _type_id; set => _type_id = value; }
        public string Name { get => _name; set => _name = value; }
        public decimal Purchase_price { get => _purchase_price; set => _purchase_price = value; }
        public int Purchase_volume { get => _purchase_volume; set => _purchase_volume = value; }
        public decimal Sell_price { get => _sell_price; set => _sell_price = value; }
        public int Sell_volume { get => _sell_volume; set => _sell_volume = value; }
        public decimal Margin { get => _margin; set => _margin = value; }
        public int[] Competition { get => _competition; set => _competition = value; }
    }

}

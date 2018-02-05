using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using IO.Swagger.Api;
using IO.Swagger.Model;
using IO.Swagger.Client;
using System.Threading;

namespace EMA_WPF
{

    public sealed class Singleton
    {
        private static readonly Singleton instance = new Singleton();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Singleton()
        {
        }

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get
            {
                return instance;
            }
        }
    }
    public sealed class EMA
    {

        private static readonly EMA instance = new EMA();
        private Eve eve = Eve.Instance;


        private static SearchApi searchApi = new SearchApi();
        private static UniverseApi universeApi = new UniverseApi();
        private static MarketApi marketApi = new MarketApi();

        public SearchApi SearchApi { get => searchApi; }
        public UniverseApi UniverseApi { get => universeApi; }
        public MarketApi MarketApi { get => marketApi; }

        public EMAStation PurchaseStation { get => purchaseStation; set => purchaseStation = value; }
        private EMAStation purchaseStation;

        public EMAStation SellStation { get => sellStation; set => sellStation = value; }
        private EMAStation sellStation;

        public List<EMASellItem> SellItems { get => sellItems; set => sellItems = value; }
        private List<EMASellItem> sellItems;

        public List<PostUniverseNames200Ok> SellItemNames { get => sellItemNames; set => sellItemNames = value; }
        private List<PostUniverseNames200Ok> sellItemNames;

        private int retryDelay;
        public int RetryDelay{ get => retryDelay; set => retryDelay = value; }

        private int retryMax;
        public int RetryMax { get => retryMax; set => retryMax = value; }

        private int historyRange;
        public int HistoryRange { get => historyRange; set => historyRange = value; }

        private double minMargin;
        public double MinMargin { get => minMargin; set => minMargin = value; }

        private double brokerRate;
        public double BrokerRate { get => brokerRate; set => brokerRate = value; }

        private double taxRate;
        public double TaxRate { get => taxRate; set => taxRate = value; }

        private IProgress<EMAProgressInfo> progress;
        public IProgress<EMAProgressInfo> Progress { get => progress; set => progress = value; }

        static EMA()
        {
        }

        private EMA()
        {
            //ESIEve.Public PublicEve = new ESIEve.Public();

            SellItems = new List<EMASellItem>
            {
                Capacity = 20000
            };
            PurchaseStation = new EMAStation();
            SellStation = new EMAStation();
            SellItemNames = new List<PostUniverseNames200Ok>
            {
                Capacity = 20000
            };
            int timeout = IO.Swagger.Client.Configuration.Default.Timeout;
            RetryDelay = 400;
            RetryMax = 10;
            HistoryRange = 30;
            MinMargin = 0.10; //filter out all items with a margin less than 10%
            BrokerRate = 0.03; // 3%
            TaxRate = 0.03; // 3%


        }
        public static EMA Instance
        {
            get
            {
                return instance;
            }
        }


        public static Eve.Search Search(string item, List<string> categories)
        {
            EMA ema = EMA.Instance;
            Eve eve = Eve.Instance;

            //ApiResponse<GetSearchOk> response = eve.SearchApi.GetSearchWithHttpInfo(categories, item);

            Eve.Search search = new Eve.Search(item, categories);
 
            return search;
        }

        public static List<PostUniverseNames200Ok> GetEveNames(List<int?> ids)
        {
            EMA ema = EMA.Instance;

            ApiResponse<List<PostUniverseNames200Ok>> response = ema.UniverseApi.PostUniverseNamesWithHttpInfo(ids);
            if (!response.StatusCode.Equals((int)HttpStatusCode.OK))
            {
                return null;
            }

            return response.Data;
        }


        public GetUniverseRegionsRegionIdOk GetEveRegion(PostUniverseNames200Ok stationName)
        {
            ApiResponse<GetUniverseStationsStationIdOk> stationResponse = UniverseApi.GetUniverseStationsStationIdWithHttpInfo(stationName.Id);
            ApiResponse<GetUniverseSystemsSystemIdOk> systemResponse = UniverseApi.GetUniverseSystemsSystemIdWithHttpInfo(stationResponse.Data.SystemId);
            ApiResponse<GetUniverseConstellationsConstellationIdOk> constellationResponse = UniverseApi.GetUniverseConstellationsConstellationIdWithHttpInfo(systemResponse.Data.ConstellationId);
            ApiResponse<GetUniverseRegionsRegionIdOk> regionResponse = UniverseApi.GetUniverseRegionsRegionIdWithHttpInfo(constellationResponse.Data.RegionId);
            return regionResponse.Data;
        }

        private List<GetMarketsRegionIdOrders200Ok> GetEveOrders(EMAStation station)
        {
            List<GetMarketsRegionIdOrders200Ok> orderList;
            ApiResponse<List<GetMarketsRegionIdOrders200Ok>> response;

            orderList = new List<GetMarketsRegionIdOrders200Ok>
            {
                Capacity = 100000
            };
            if (station != null)
            {
                List<GetMarketsRegionIdOrders200Ok> orderPage;
                //response = GetMarketOrdersHelper(station.Region_id, 1, null, progress);
                response = GetMarketOrdersHelper(station.Region_id, 1, null);
                int pages = int.Parse(response.Headers["X-Pages"]);
                for (int page = 1; page < pages; page++)
                {
                    orderPage = response.Data;
                    orderList.AddRange(orderPage);
                    //response = GetMarketOrdersHelper(station.Region_id, page, null, progress);
                    response = GetMarketOrdersHelper(station.Region_id, page, null);
                }
                orderPage = response.Data;
                orderList.AddRange(orderPage);
            }
            return orderList;
        }

        private List<GetMarketsRegionIdOrders200Ok> GetEveOrdersByName(EMAStation station, PostUniverseNames200Ok name)
        {
            List<GetMarketsRegionIdOrders200Ok> orderList;
            ApiResponse<List<GetMarketsRegionIdOrders200Ok>> response;

            orderList = new List<GetMarketsRegionIdOrders200Ok>
            {
                Capacity = 100000
            };
            if (station != null)
            {
                List<GetMarketsRegionIdOrders200Ok> orderPage;
                //response = GetMarketOrdersHelper(station.Region_id, 1, name.Id, progress);
                response = GetMarketOrdersHelper(station.Region_id, 1, name.Id);
                int pages = int.Parse(response.Headers["X-Pages"]);
                for (int page = 1; page < pages; page++)
                {
                    orderPage = response.Data;
                    orderList.AddRange(orderPage);
                    //response = GetMarketOrdersHelper(station.Region_id, page, name.Id, progress);
                    response = GetMarketOrdersHelper(station.Region_id, page, name.Id);
                }
                orderPage = response.Data;
                orderList.AddRange(orderPage);
            }
            return orderList;
        }

        private ApiResponse<List<GetMarketsRegionIdOrders200Ok>> GetMarketOrdersHelper(int regionid, int page, int? nameid)
        {
            int retry = 0;
            ApiResponse<List<GetMarketsRegionIdOrders200Ok>> response;
            try
            {
                response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", regionid, null, page, nameid);

            }
            catch (IO.Swagger.Client.ApiException ex)
            {
                retry++;
                if (retry < RetryMax)
                {
                    if (nameid == null)
                    {
                        Progress.Report(new EMAProgressInfo(String.Format(" -->GetMarketOrders: {0} retry {1}, region {2}, page{3}", ex.Message, retry, regionid, page)));
                    }
                    else
                    {
                        progress.Report(new EMAProgressInfo(String.Format(" -->GetMarketOrders: {0} retry {1}, region {2}, item {3}, page{4}", ex.Message, retry, regionid, nameid, page)));
                    }
                    Thread.Sleep(RetryDelay); //wait some time before retrying
                    response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", regionid, null, page, nameid);
                    return response;
                }
                throw;
            }
            return response;
        }

        public string GetItemNamesForRegions()
        {
            DateTime start = DateTime.Now;
            List<int?> purchaseItemIDs, sellItemIDs;
            SellItemNames.Clear();

            purchaseItemIDs = GetItemIDs(PurchaseStation.Region_id);
            sellItemIDs = GetItemIDs(SellStation.Region_id);

            List<int?> itemIDs = new List<int?>
            {
                Capacity = 50000
            };

            itemIDs.AddRange(purchaseItemIDs);
            itemIDs = itemIDs.Intersect(sellItemIDs).ToList();
            if (Progress != null)
            {
                Progress.Report(new EMAProgressInfo(String.Format("Get Items: region {0}: {1} ids, region {2}: {3} ids, consolidated {4} ids",
                            PurchaseStation.Region_id, purchaseItemIDs.Count.ToString(),
                            SellStation.Region_id, sellItemIDs.Count.ToString(),
                            itemIDs.Count.ToString())));
            }

            List<int?> slice = new List<int?>();
            ApiResponse<List<PostUniverseNames200Ok>> response;
            int increment = 1000;
            for (int i = 0; i < itemIDs.Count; i += increment)
            {
                slice = itemIDs.GetRange(i, Math.Min(increment, itemIDs.Count - i));
                response = UniverseApi.PostUniverseNamesWithHttpInfo(slice);
                SellItemNames.AddRange(response.Data);
                if (Progress != null)
                {
                    Progress.Report(new EMAProgressInfo(String.Format("Get Items: names: {0}/{1}", SellItemNames.Count.ToString(), itemIDs.Count.ToString())));
                }

            }
            return String.Format(" finished, time elapsed: {0}", DateTime.Now - start);
        }

        private List<int?> GetItemIDs(int region)
        {
            List<int?> itemIDs = new List<int?>
            {
                Capacity = 100000
            };

            ApiResponse<List<int?>> response = GetItemIdsHelper(1);
            if (!response.StatusCode.Equals((int)HttpStatusCode.OK))
            {
                return null;
            }

            int pages = int.Parse(response.Headers["X-Pages"]);
            for (int page = 1; page < pages; page++)
            {
                itemIDs.AddRange(response.Data);
                response = GetItemIdsHelper(page + 1);
            }
            itemIDs.AddRange(response.Data);
            itemIDs = itemIDs.Distinct().ToList();

            if (Progress != null)
            {
                Progress.Report(new EMAProgressInfo(String.Format("region {0}: ids {1}", region.ToString(), itemIDs.Count.ToString())));
            }
            return itemIDs;

            ApiResponse<List<int?>> GetItemIdsHelper(int _page)
            {
                ApiResponse<List<int?>> _response;
                int retryCount = 0;
                try
                {
                    _response = MarketApi.GetMarketsRegionIdTypesWithHttpInfo(region, null, _page);
                    if (Progress != null)
                    {
                        Progress.Report(new EMAProgressInfo(String.Format("region {0}: ids {1}", region.ToString(), itemIDs.Count.ToString())));
                    }
                }
                catch (IO.Swagger.Client.ApiException ex)
                {
                    retryCount++;
                    if (retryCount < retryMax)
                    {
                        if (Progress != null)
                        {
                            Progress.Report(new EMAProgressInfo(String.Format("region {0}: ids {1}, -->Exception {2}, retrying", region.ToString(), itemIDs.Count.ToString(), ex.Message)));

                        }
                        Thread.Sleep(RetryDelay); //wait some time before retrying
                        _response = MarketApi.GetMarketsRegionIdTypesWithHttpInfo(region, null, _page);
                        return _response;
                    }
                    throw;
                }
                return _response;
            }
        }

        public string GetSellItems()
        {
            DateTime start = DateTime.Now;
            List<GetMarketsRegionIdOrders200Ok> purchaseOrders, sellOrders, pOrders, sOrders;

            EMASellItem item;
            purchaseOrders = GetEveOrders(PurchaseStation);
            int removedPurchaseRegionBuyOrders = purchaseOrders.RemoveAll(RemoveBuyOrdersHelper);
            int removedWrongStationPurchaseOrders = purchaseOrders.RemoveAll(RemoveWrongPurchaseStationsHelper);

            sellOrders = GetEveOrders(SellStation);
            int removedSellRegionBuyOrders = sellOrders.RemoveAll(RemoveBuyOrdersHelper);
            int removedWrongStationSellOrders = sellOrders.RemoveAll(RemoveWrongSellStationsHelper);

            foreach (PostUniverseNames200Ok name in SellItemNames)
            {
                bool GetOrdersByItemNameHelper(GetMarketsRegionIdOrders200Ok order)
                {
                    return (bool)(order.TypeId == name.Id);
                }
                sOrders = sellOrders.FindAll(GetOrdersByItemNameHelper);
                pOrders = purchaseOrders.FindAll(GetOrdersByItemNameHelper);
                item = FillItem(PurchaseStation.Station_id, SellStation.Station_id, name, pOrders, sOrders);
                if (item != null)
                {
                    SellItems.Add(item);
                    Progress.Report(new EMAProgressInfo(item,true,String.Format("{0}: Item {1}: {2}", SellItems.Count, item.Type_id.ToString(), item.Name)));
                }
            }
            return String.Format(" finished, time elapsed: {0}", DateTime.Now - start);
        }

        private bool RemoveBuyOrdersHelper(GetMarketsRegionIdOrders200Ok order)
        {
            return (bool)order.IsBuyOrder;
        }
        private bool RemoveWrongPurchaseStationsHelper(GetMarketsRegionIdOrders200Ok order)
        {
            return (bool)(order.LocationId != PurchaseStation.Station_id);
        }
        private bool RemoveWrongSellStationsHelper(GetMarketsRegionIdOrders200Ok order)
        {
            return (bool)(order.LocationId != SellStation.Station_id);
        }

        public string GetSellItemsByName()
        {
            DateTime start = DateTime.Now;
            List<GetMarketsRegionIdOrders200Ok> purchaseOrders, sellOrders;
 
            EMASellItem item;
            foreach (PostUniverseNames200Ok name in SellItemNames)
            {
                purchaseOrders = GetEveOrdersByName(PurchaseStation, name);
                if (purchaseOrders.Count == 0)
                {
                    //if the item is not sold in the region of purchase station skip it
                    continue;
                }
                sellOrders = GetEveOrdersByName(SellStation, name);
                item = FillItem(PurchaseStation.Station_id, SellStation.Station_id, name, purchaseOrders, sellOrders);
                if (item != null)
                {
                    if (item.Margin < 20)
                    {
                        continue;
                    }
                    SellItems.Add(item);
                    Progress.Report(new EMAProgressInfo(item, true, String.Format("{0}: Item {1}: {2}", SellItems.Count, item.Type_id.ToString(), item.Name)));
                }
            }
            return String.Format(" finished, time elapsed: {0}",DateTime.Now - start);
        }

        private EMASellItem FillItem(int pStation, int sStation, PostUniverseNames200Ok name, List<GetMarketsRegionIdOrders200Ok> pOrders, List<GetMarketsRegionIdOrders200Ok> sOrders)
        {

            EMASellItem item = new EMASellItem();
            DateTime now = DateTime.Now.ToUniversalTime();
            TimeSpan[] timeSpan = new TimeSpan[] { TimeSpan.FromHours(1), TimeSpan.FromHours(3), TimeSpan.FromHours(24), TimeSpan.FromDays(3) };

            TimeSpan activeFor = new TimeSpan();
            item.Name = name.Name;
            item.Type_id = (int)name.Id;
            foreach (GetMarketsRegionIdOrders200Ok order in pOrders)
            {
                if (order.LocationId == pStation)
                {
                    item.Purchase_price = Math.Min(item.Purchase_price, (double)order.Price);
                }
            }
            if (item.Purchase_price == Double.MaxValue) //not sold in purchase station
            {
                return null;
            }

            item.Competition[4] = 0; // overall number of competitors
            foreach (GetMarketsRegionIdOrders200Ok order in sOrders)
            {
                if (order.LocationId == sStation)
                {
                    item.Competition[4]++;
                    activeFor = now - (DateTime)order.Issued;
                    item.Sell_price = Math.Min(item.Sell_price, (double)order.Price);
                    for (int i = 0; i < 4; i++)
                    {
                        if (activeFor <= timeSpan[i]) item.Competition[i]++;
                    }
                }
            }
            if (item.Sell_price == Double.MaxValue) item.Sell_price = 0;
            //double correctedPurchasePrice = item.Purchase_price * (1 + BrokerRate);
            double correctedPurchasePrice = item.Purchase_price;
            double correctedSellPrice = item.Sell_price * (1 + TaxRate + BrokerRate);
            item.AbsoluteMargin = correctedSellPrice - correctedPurchasePrice;
            item.Margin = item.AbsoluteMargin / correctedPurchasePrice;

            if (item.Margin < MinMargin && item.Margin > -1) return null;

            return item;
        }

        public void GetHistory()
        {
            List<EMASellItem> selecteditems = SellItems.FindAll(FindSelectedItemsHelper);

            foreach (EMASellItem item in selecteditems)
            {
                ApiResponse<List<GetMarketsRegionIdHistory200Ok>> response;
                List<GetMarketsRegionIdHistory200Ok> eveHistoryData;
                List<EMAHistoryItem> emaHistoryData;

                int retryCount = 0;
                try
                {
                    response = MarketApi.GetMarketsRegionIdHistoryWithHttpInfo(SellStation.Region_id, item.Type_id);
                }
                catch (IO.Swagger.Client.ApiException ex)
                {
                    if (ex.Message.Contains("\"error\":\"Type not found!\"")) return;
                    retryCount++;
                    if (retryCount > retryMax)
                    {
                        throw;
                    }
                    Thread.Sleep(RetryDelay);
                    response = MarketApi.GetMarketsRegionIdHistoryWithHttpInfo(SellStation.Region_id, item.Type_id);
                }
                // get the eve history records for the last days (ema.HistoryRange)
                eveHistoryData = response.Data.FindAll(GetHistoryRecordsInRange);
                // compute the average volume
                if (eveHistoryData.Count > 0) item.Sell_volume = fillItemHistoryHelper2(eveHistoryData);
                // create the ema history records
                emaHistoryData = CreateEMAHistoryList(eveHistoryData);
                item.SellHistory = new EMAHistory(emaHistoryData);
                // determine volumes and weighted sell prices per period of days
                item.SellHistory.WeightedSellPriceandVolumes(1);
                item.SellHistory.WeightedSellPriceandVolumes(3);
                item.SellHistory.WeightedSellPriceandVolumes(7);
                item.SellHistory.WeightedSellPriceandVolumes(14);
                item.SellHistory.WeightedSellPriceandVolumes(21);
                item.SellHistory.WeightedSellPriceandVolumes(28);

                retryCount = 0;
                try
                {
                    response = MarketApi.GetMarketsRegionIdHistoryWithHttpInfo(PurchaseStation.Region_id, item.Type_id);
                }
                catch (IO.Swagger.Client.ApiException ex)
                {
                    if (ex.Message.Contains("\"error\":\"Type not found!\"")) return;
                    retryCount++;
                    if (retryCount > retryMax)
                    {
                        throw;
                    }
                    Thread.Sleep(RetryDelay);
                    response = MarketApi.GetMarketsRegionIdHistoryWithHttpInfo(PurchaseStation.Region_id, item.Type_id);

                }
                eveHistoryData = response.Data.FindAll(GetHistoryRecordsInRange);
                if (eveHistoryData.Count > 0) item.Purchase_volume = fillItemHistoryHelper2(eveHistoryData);

            }
            return;

            bool GetHistoryRecordsInRange(GetMarketsRegionIdHistory200Ok order)
            {
                TimeSpan orderAge = (TimeSpan)(DateTime.Now - order.Date);
                DateTime testdate = DateTime.Now.AddDays(-HistoryRange);
                if (orderAge.Days > HistoryRange) return false;
                return true;
            }

            int fillItemHistoryHelper2(List<GetMarketsRegionIdHistory200Ok> history)
            {
                int volumeSum = 0;
                foreach (GetMarketsRegionIdHistory200Ok record in history)
                {
                    volumeSum += (int)record.Volume;
                }
                float averageVolume = (float)volumeSum / history.Count;
                return volumeSum / history.Count;
            }
        }

        public bool FindSelectedItemsHelper(EMASellItem item)
        {
            return item.IsSelected;
        }

        private List<EMAHistoryItem> CreateEMAHistoryList(List<GetMarketsRegionIdHistory200Ok> eveHistory)
        {
            EMAHistoryItem emaHistoryItem;
            List<EMAHistoryItem> emaHistory =  new List<EMAHistoryItem>();
            foreach (GetMarketsRegionIdHistory200Ok eveHistoryItem in eveHistory)
            {
                emaHistoryItem = new EMAHistoryItem(eveHistoryItem.Date, eveHistoryItem.Lowest, eveHistoryItem.Highest, eveHistoryItem.Average, eveHistoryItem.Volume);
                emaHistory.Add(emaHistoryItem);
            }
            return emaHistory;
        }

        public class EMAStation
        {
            public int Station_id { get; set; }
            public string Station_name { get; set; }
            public int Region_id { get; set; }
            public string Region_name { get; set; }
        }

        public class Station
        // a station with its id and name
        {
            private int stationId;
            private string stationName;

            public int Id { get => stationId; set => stationId = value; }
            public string Name { get => stationName; set => stationName = value; }

            Station(int id)
            {

            }
        }

        public class EMASellItem
        {
            private bool isSelected;
            private int type_id;
            private string name;
            private double purchase_price;
            private int purchase_volume;
            private double sell_price;
            private int sell_volume;
            private double margin;
            private double absoluteMargin;
            private int[] competition;
            private EMAHistory purchaseHistory;
            private EMAHistory sellHistory;

            public EMASellItem()
            {
                IsSelected = false;
                Type_id = 0;
                Name = null;
                Purchase_price = Double.MaxValue;
                Purchase_volume = 0;
                Sell_price = Double.MaxValue;
                Sell_volume = 0;
                Margin = 0;
                PurchaseHistory = null;
                SellHistory = null;
                Competition = new int[] { 0, 0, 0, 0, 0 };  //# of competitors in 1h, 3h, 1d, 3d, all
            }

            public bool IsSelected { get => isSelected; set => isSelected = value; }
            public int Type_id { get => type_id; set => type_id = value; }
            public string Name { get => name; set => name = value; }
            public double Purchase_price { get => purchase_price; set => purchase_price = value; }
            public int Purchase_volume { get => purchase_volume; set => purchase_volume = value; }
            public double Sell_price { get => sell_price; set => sell_price = value; }
            public int Sell_volume { get => sell_volume; set => sell_volume = value; }
            public double Margin { get => margin; set => margin = value; }
            public double AbsoluteMargin { get => absoluteMargin; set => absoluteMargin = value; }
            public int[] Competition { get => competition; set => competition = value; }
            public EMAHistory PurchaseHistory { get => purchaseHistory; set => purchaseHistory = value; }
            public EMAHistory SellHistory { get => sellHistory; set => sellHistory = value; }
        }

        public class EMAProgressInfo
        {
            private bool _isNewItem;
            private string _message;
            private EMASellItem _item;

            public EMAProgressInfo(EMASellItem item, bool isNewItem, string message)
            {
                this.IsNewItem = isNewItem;
                this.Message = message;
                this.Item = item;

            }
            public EMAProgressInfo(string message)
            {
                this.Item = null;
                this.IsNewItem = false;
                this.Message = message;
            }

            public EMASellItem Item { get => _item; set => _item = value; }
            public bool IsNewItem { get => _isNewItem; set => _isNewItem = value; }
            public string Message { get => _message; set => _message = value; }
        }

        public class EMAHistoryItem
        {
            private DateTime date;
            private double sellPrice;
            private double buyPrice;
            private long sellVolume;
            private long buyVolume;

            public EMAHistoryItem(DateTime? newDate, double? lowPrice, double? highPrice, double? averagePrice, long? dayVolume)
            {
                Date = (DateTime)newDate;

                SellPrice = 0;
                BuyPrice = 0;
                SellVolume = 0;
                BuyVolume = 0;

                if (dayVolume > 0)
                {
                    if (highPrice != lowPrice)
                    {
                        double priceRange = (double)highPrice - (double)lowPrice;
                        SellPrice = (double)highPrice;
                        BuyPrice = (double)lowPrice;
                        SellVolume = (long)(dayVolume * (double)((averagePrice - lowPrice) / priceRange));
                        BuyVolume = (long)dayVolume - SellVolume;
                    }
                    else
                    {
                        BuyVolume = (long)dayVolume;
                        BuyPrice = (double)lowPrice;
                    }
                }
            }

            public DateTime Date { get => date; set => date = value; }
            public double SellPrice { get => sellPrice; set => sellPrice = value; }
            public double BuyPrice { get => buyPrice; set => buyPrice = value; }
            public long SellVolume { get => sellVolume; set => sellVolume = value; }
            public long BuyVolume { get => buyVolume; set => buyVolume = value; }

        }

        public class EMAHistory
        {
            private List<EMAHistoryItem> historyItems;
            private Dictionary<int, double> weightedSellPrices;
            private Dictionary<int, long> sellVolumes;

            public List<EMAHistoryItem> HistoryItems { get => historyItems; set => historyItems = value; }
            public Dictionary<int, double> WeightedSellPrices { get => weightedSellPrices; set => weightedSellPrices = value; }
            public Dictionary<int, long> SellVolumes { get => sellVolumes; set => sellVolumes = value; }

            public EMAHistory()
            {
                HistoryItems = null;
                WeightedSellPrices = new Dictionary<int, double>();
                SellVolumes = new Dictionary<int, long>();
            }

            public EMAHistory(List<EMAHistoryItem> historyitemList) : this() => HistoryItems = historyitemList;

            public void WeightedSellPriceandVolumes(int timespan)
            {
                double weightedSellPrice = 0;
                long volume = 0;

                foreach (EMAHistoryItem historyItem in HistoryItems)
                {
                    TimeSpan itemAge = historyItem.Date - DateTime.Now;
                    if (itemAge.Days < timespan)
                    {
                        volume += historyItem.SellVolume;
                        weightedSellPrice += historyItem.SellPrice * historyItem.SellVolume;

                    }
                }
                weightedSellPrice /= volume;
                SellVolumes.Add(timespan, volume);
                WeightedSellPrices.Add(timespan, weightedSellPrice);
            }


        }



    } /* class EMA */



}

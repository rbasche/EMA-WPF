using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using ESISharp;
using ESISharp.Enumerations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        private static SearchApi searchApi = new SearchApi();
        private static UniverseApi universeApi = new UniverseApi();
        private static MarketApi marketApi = new MarketApi();
        private static ESIEve.Public publicEve = new ESIEve.Public();


        public SearchApi SearchApi { get => searchApi; }
        public UniverseApi UniverseApi { get => universeApi; }
        public MarketApi MarketApi { get => marketApi; }
        public ESIEve.Public PublicEve { get => publicEve; }

        public EMAStation PurchaseStation { get => purchaseStation; set => purchaseStation = value; }
        private EMAStation purchaseStation;

        public EMAStation SellStation { get => sellStation; set => sellStation = value; }
        private EMAStation sellStation;

        public List<EMASellItem> SellItems { get => sellItems; set => sellItems = value; }
        private List<EMASellItem> sellItems;

        public List<PostUniverseNames200Ok> SellItemNames { get => sellItemNames; set => sellItemNames = value; }
        private List<PostUniverseNames200Ok> sellItemNames;

        static EMA()
        {
        }

        private EMA()
        {
            ESIEve.Public PublicEve = new ESIEve.Public();

            SellItems = new List<EMASellItem>();
            PurchaseStation = new EMAStation();
            SellStation = new EMAStation();
            SellItemNames = new List<PostUniverseNames200Ok>
            {
                Capacity = 200000
            };

        }
        public static EMA Instance
        {
            get
            {
                return instance;
            }
        }


        public static EveSearch Search(string item, List<string> categories)
        {
            EMA ema = EMA.Instance;

            ApiResponse<GetSearchOk> response = ema.SearchApi.GetSearchWithHttpInfo(categories, item);

            EveSearch search = new EveSearch
            {
                Station = response.Data.Station
            };
            //search = JsonConvert.DeserializeObject<EveSearch>(data.ToString());

            return search;
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


        public static List<PostUniverseNames200Ok> GetEveNames(List<int?> ids)
        {
            EMA ema = EMA.Instance;

            //EsiResponse response = ema.PublicEve.Universe.GetTypeNamesAndCategories(ids).Execute();
            ema.UniverseApi.PostUniverseNamesWithHttpInfo(ids);
            ApiResponse<List<PostUniverseNames200Ok>> response = ema.UniverseApi.PostUniverseNamesWithHttpInfo(ids);
            if (!response.StatusCode.Equals((int)HttpStatusCode.OK))
            {
                return null;
            }

            
            //list = JsonConvert.DeserializeObject<List<EveName>>(response.Body);
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

        private List<GetMarketsRegionIdOrders200Ok> GetEveOrders(EMAStation station, IProgress<EMAProgress> progress)
        {
            List<GetMarketsRegionIdOrders200Ok> orderList;
            ApiResponse<List<GetMarketsRegionIdOrders200Ok>> response;

            orderList = new List<GetMarketsRegionIdOrders200Ok>();
            if (station != null)
            {
                List<GetMarketsRegionIdOrders200Ok> orderPage;
                response = GetMarketOrdersHelper(station.Region_id, 1, null, progress);
                //response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", station.Region_id, null, 1, name.Id);
                //response = PublicEve.Market.GetRegionOrders(station.Region_id, name.Id, MarketOrderType.Sell).Execute();
                int pages = int.Parse(response.Headers["X-Pages"]);
                for (int page = 1; page < pages; page++)
                {
                    orderPage = response.Data;
                    orderList.AddRange(orderPage);
                    response = GetMarketOrdersHelper(station.Region_id, page, null, progress);
                    //response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", station.Region_id,null,page+1,name.Id);
                }
                orderPage = response.Data;
                orderList.AddRange(orderPage);
            }
            return orderList;
        }
        private List<GetMarketsRegionIdOrders200Ok> GetEveOrdersByName(EMAStation station, PostUniverseNames200Ok name, IProgress<EMAProgress> progress)
        {
            List<GetMarketsRegionIdOrders200Ok> orderList;
            ApiResponse<List<GetMarketsRegionIdOrders200Ok>> response;

            orderList = new List<GetMarketsRegionIdOrders200Ok>();
            if (station != null)
            {
                List<GetMarketsRegionIdOrders200Ok> orderPage;
                response = GetMarketOrdersHelper(station.Region_id, 1, name.Id, progress);
                //response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", station.Region_id, null, 1, name.Id);
                //response = PublicEve.Market.GetRegionOrders(station.Region_id, name.Id, MarketOrderType.Sell).Execute();
                int pages = int.Parse(response.Headers["X-Pages"]);
                for (int page = 1; page < pages; page++)
                {
                    orderPage = response.Data;
                    orderList.AddRange(orderPage);
                    response = GetMarketOrdersHelper(station.Region_id, page, name.Id, progress);
                    //response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", station.Region_id,null,page+1,name.Id);
                }
                orderPage = response.Data;
                orderList.AddRange(orderPage);
            }
            return orderList;
        }

        private ApiResponse<List<GetMarketsRegionIdOrders200Ok>> GetMarketOrdersHelper(int regionid, int page, int? nameid, IProgress<EMAProgress> progress)
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
                if (retry < 4)
                {
                    if (nameid == null)
                    {
                        progress.Report(new EMAProgress(String.Format(" -->GetMarketOrders: {0} retry {1}, region {2}, page{3}", ex.Message, retry, regionid, page)));
                    }
                    else
                    {
                        progress.Report(new EMAProgress(String.Format(" -->GetMarketOrders: {0} retry {1}, region {2}, item {3}, page{4}", ex.Message, retry, regionid, nameid, page)));
                    }
                    Thread.Sleep(500); //wait some time before retrying
                    response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", regionid, null, page, nameid);
                    return response;
                }
                throw;
            }
            return response;
        }

        public string GetItemNamesForRegions()
        {
            return GetItemNamesForRegions(null);
        }
        public string GetItemNamesForRegions(IProgress<string> progress)
        {
            DateTime start = DateTime.Now;
            List<int?> purchaseItemIDs, sellItemIDs;
            SellItemNames.Clear();

            purchaseItemIDs = GetItemIDs(PurchaseStation.Region_id, progress);
            sellItemIDs = GetItemIDs(SellStation.Region_id, progress);

            List<int?> itemIDs = new List<int?>
            {
                Capacity = 50000
            };

            itemIDs.AddRange(purchaseItemIDs);
            itemIDs = itemIDs.Intersect(sellItemIDs).ToList();
            if (progress != null)
            {
                progress.Report(String.Format("region {0}: {1} ids, region {2}: {3} ids, consolidated {4} ids",
                    PurchaseStation.Region_id, purchaseItemIDs.Count.ToString(),
                    SellStation.Region_id, sellItemIDs.Count.ToString(),
                    itemIDs.Count.ToString()));
            }

            List<int?> slice = new List<int?>();
            ApiResponse<List<PostUniverseNames200Ok>> response;
            int increment = 1000;
            for (int i = 0; i < itemIDs.Count; i += increment)
            {
                slice = itemIDs.GetRange(i, Math.Min(increment, itemIDs.Count - i));
                response = UniverseApi.PostUniverseNamesWithHttpInfo(slice);
                SellItemNames.AddRange(response.Data);
                //SellItemNames.AddRange(JsonConvert.DeserializeObject<List<EveName>>(response.Body));
                if (progress != null)
                {
                    progress.Report(String.Format("names: {0}/{1}", SellItemNames.Count.ToString(), itemIDs.Count.ToString()));
                }

            }
            return String.Format(" finished, time elapsed: {0}", DateTime.Now - start);
        }

        private List<int?> GetItemIDs(int region)
        {
            return GetItemIDs(region, null);
        }
        private List<int?> GetItemIDs(int region,IProgress<string> progress)
        {
            List<int?> itemIDs = new List<int?>
            {
                Capacity = 100000
            };

            ApiResponse<List<int?>> response = GetItemIdsHelper(1);
            //ApiResponse<List<int?>> response = MarketApi.GetMarketsRegionIdTypesWithHttpInfo(region);
            //response = PublicEve.Market.GetMarketTypes(region).Execute();
            if (!response.StatusCode.Equals((int)HttpStatusCode.OK))
            {
                return null;
            }


            int pages = int.Parse(response.Headers["X-Pages"]);
            for (int page = 1; page < pages; page++)
            {
                itemIDs.AddRange(response.Data);
                response = GetItemIdsHelper(page + 1);
                //response = MarketApi.GetMarketsRegionIdTypesWithHttpInfo(region, null, page + 1);
            }
            itemIDs.AddRange(response.Data);
            itemIDs = itemIDs.Distinct().ToList();

            if (progress != null)
            {
                progress.Report(String.Format("region {0}: ids {1}", region.ToString(), itemIDs.Count.ToString()));
            }
            return itemIDs;
            ApiResponse<List<int?>> GetItemIdsHelper(int _page)
            {
                ApiResponse<List<int?>> _response;
                int retry = 0;
                try
                {
                    _response = MarketApi.GetMarketsRegionIdTypesWithHttpInfo(region, null, _page);
                    if (progress != null)
                    {
                        progress.Report(String.Format("region {0}: ids {1}", region.ToString(), itemIDs.Count.ToString()));
                    }
                }
                catch (IO.Swagger.Client.ApiException ex)
                {
                    retry++;
                    if (retry < 4)
                    {
                        if (progress != null)
                        {
                            progress.Report(String.Format("region {0}: ids {1}, -->Exception {2}, retrying", region.ToString(), itemIDs.Count.ToString(),ex.Message));
                        }
                        Thread.Sleep(500); //wait some time before retrying
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
            return GetSellItems(null);
        }
        public string GetSellItems(IProgress<EMAProgress> progress)
        {
            DateTime start = DateTime.Now;
            List<GetMarketsRegionIdOrders200Ok> purchaseOrders, sellOrders, pOrders, sOrders;

            EMASellItem item;
            purchaseOrders = GetEveOrders(PurchaseStation, progress);
            int removedPurchaseRegionBuyOrders = purchaseOrders.RemoveAll(RemoveBuyOrdersHelper);
            int removedWrongStationPurchaseOrders = purchaseOrders.RemoveAll(RemoveWrongPurchaseStationsHelper);

            sellOrders = GetEveOrders(SellStation, progress);
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
                    progress.Report(new EMAProgress(item,true,String.Format("{0}: Item {1}: {2}", SellItems.Count, item.Type_id.ToString(), item.Name)));
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
            return GetSellItemsByName(null);
        }
        public string GetSellItemsByName(IProgress<EMAProgress> progress)
        {
            DateTime start = DateTime.Now;
            List<GetMarketsRegionIdOrders200Ok> purchaseOrders, sellOrders;
 
            EMASellItem item;
            foreach (PostUniverseNames200Ok name in SellItemNames)
            {
                purchaseOrders = GetEveOrdersByName(PurchaseStation, name, progress);
                if (purchaseOrders.Count == 0)
                {
                    //if the item is not sold in the region of purchase station skip it
                    continue;
                }
                sellOrders = GetEveOrdersByName(SellStation, name, progress);
                item = FillItem(PurchaseStation.Station_id, SellStation.Station_id, name, purchaseOrders, sellOrders);
                if (item != null)
                {
                    if (item.Margin < 20)
                    {
                        continue;
                    }
                    SellItems.Add(item);
                    progress.Report(new EMAProgress(item,true,String.Format("{0}: Item {1}: {2}",SellItems.Count, item.Type_id.ToString(), item.Name)));
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
                    item.Purchase_price = Math.Max(item.Purchase_price, (double)order.Price);
                }
            }
            if (item.Purchase_price == 0) //not sold in purchase station
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
                    item.Sell_price = Math.Max(item.Sell_price, (double)order.Price);
                    for (int i = 0; i < 4; i++)
                    {
                        if (activeFor <= timeSpan[i]) item.Competition[i]++;
                    }
                }
            }
            if (item.Sell_price == 0)
            {
                item.Margin = 10;
            }
            else
            {
                item.Margin = (decimal)((item.Sell_price-item.Purchase_price)/item.Sell_price);
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
        private double _purchase_price;
        private int _purchase_volume;
        private double _sell_price;
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
            _competition = new int[] { 0, 0, 0, 0, 0 };
        }

        public int Type_id { get => _type_id; set => _type_id = value; }
        public string Name { get => _name; set => _name = value; }
        public double Purchase_price { get => _purchase_price; set => _purchase_price = value; }
        public int Purchase_volume { get => _purchase_volume; set => _purchase_volume = value; }
        public double Sell_price { get => _sell_price; set => _sell_price = value; }
        public int Sell_volume { get => _sell_volume; set => _sell_volume = value; }
        public decimal Margin { get => _margin; set => _margin = value; }
        public int[] Competition { get => _competition; set => _competition = value; }
    }

    public class EMAProgress
    {
        private bool _isNewItem;
        private string _message;
        private EMASellItem _item;

        public EMAProgress(EMASellItem item,bool isNewItem, string message)
        {
            this.IsNewItem = isNewItem;
            this.Message = message;
            this.Item = item;

        }
        public EMAProgress(string message)
        {
            this.Item = null;
            this.IsNewItem = false;
            this.Message = message;
        }

        public EMASellItem Item { get => _item; set => _item = value; }
        public bool IsNewItem { get => _isNewItem; set => _isNewItem = value; }
        public string Message { get => _message; set => _message = value; }
    }

}

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

            SellItems = new List<EMASellItem>
            {
                Capacity = 10000
            };
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

        private List<GetMarketsRegionIdOrders200Ok> GetEveOrders(EMAStation station, PostUniverseNames200Ok name)
        {
            List<GetMarketsRegionIdOrders200Ok> orderList;
            ApiResponse<List<GetMarketsRegionIdOrders200Ok>> response;

            orderList = new List<GetMarketsRegionIdOrders200Ok>();
            if (station != null)
            {
                List<GetMarketsRegionIdOrders200Ok> orderPage;
                response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", station.Region_id,null,1,name.Id);
                //response = PublicEve.Market.GetRegionOrders(station.Region_id, name.Id, MarketOrderType.Sell).Execute();
                int pages = int.Parse(response.Headers["X-Pages"]);
                for (int page = 1; page < pages; page++)
                {
                    orderPage = response.Data;
                    orderList.AddRange(orderPage);
                    response = MarketApi.GetMarketsRegionIdOrdersWithHttpInfo("sell", station.Region_id,null,page+1,name.Id);
                }
                orderPage = response.Data;
                orderList.AddRange(orderPage);
            }
            return orderList;
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
            ApiResponse<List<int?>> response = MarketApi.GetMarketsRegionIdTypesWithHttpInfo(region); 
            //response = PublicEve.Market.GetMarketTypes(region).Execute();
            if (!response.StatusCode.Equals((int)HttpStatusCode.OK))
            {
                return null;
            }

            //List<int> itemIDPage = new List<int>();
            List<int?> itemIDs = new List<int?>
            {
                Capacity = 100000
            };

            int pages = int.Parse(response.Headers["X-Pages"]);
            for (int page = 1; page < pages; page++)
            {
                itemIDs.AddRange(response.Data);
                if (progress != null)
                {
                    progress.Report(String.Format("region {0}: ids {1}", region.ToString(), itemIDs.Count.ToString()));
                }
                response = MarketApi.GetMarketsRegionIdTypesWithHttpInfo(region,null,page+1);
            }
            itemIDs.AddRange(response.Data);
            itemIDs = itemIDs.Distinct().ToList();

            if (progress != null)
            {
                progress.Report(String.Format("region {0}: ids {1}", region.ToString(), itemIDs.Count.ToString()));
            }
            return itemIDs;
        }

        public string GetSellItems()
        {
            return GetSellItems(null);
        }
        public string GetSellItems(IProgress<string> progress)
        {
            DateTime start = DateTime.Now;
            List<GetMarketsRegionIdOrders200Ok> purchaseOrders, sellOrders;
 
            EMASellItem item;
            foreach (PostUniverseNames200Ok name in SellItemNames)
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
            return String.Format(" finished, time elapsed: {0}",DateTime.Now - start);
        }

        private EMASellItem FillItem(PostUniverseNames200Ok name, List<GetMarketsRegionIdOrders200Ok> pOrders, List<GetMarketsRegionIdOrders200Ok> sOrders)
        {
            EMASellItem item = new EMASellItem();
            DateTime now = DateTime.Now;
            TimeSpan[] timeSpan = new TimeSpan[] { TimeSpan.FromHours(1), TimeSpan.FromHours(3), TimeSpan.FromHours(24), TimeSpan.FromDays(3) };

            TimeSpan activeFor = new TimeSpan();
            item.Name = name.Name;
            item.Type_id = (int)name.Id;
            foreach (GetMarketsRegionIdOrders200Ok oItem in pOrders)
            {
                item.Purchase_price = Math.Max(item.Purchase_price, (double)oItem.Price);
            }

            item.Competition[4] = sOrders.Count;
            foreach (GetMarketsRegionIdOrders200Ok sItem in sOrders)
            {
                activeFor = now - (DateTime)sItem.Issued;
                item.Sell_price = Math.Max(item.Sell_price, (double)sItem.Price);
                activeFor = now - (DateTime)sItem.Issued;
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
                item.Margin = Decimal.Round(100*(decimal)((item.Sell_price-item.Purchase_price)/item.Sell_price),0);
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

}

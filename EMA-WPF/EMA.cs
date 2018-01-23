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
            PurchaseStation = new EMAStation();
            SellStation = new EMAStation();
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

        private List<EveOrder> GetEveOrders(EMAStation station)
        {
            List<EveOrder> orderList;
            EsiResponse response;

            orderList = new List<EveOrder>();
            if (station != null)
            {
                List<EveOrder> orderPage;
                response = PublicEve.Market.GetRegionOrders(station.Region_id, null, MarketOrderType.Sell, 1).Execute();
                int pages = response.Headers.Pages;
                for (int page = 1; page < pages; page++)
                {
                    orderPage = JsonConvert.DeserializeObject<List<EveOrder>>(response.Body);
                    orderList.AddRange(orderPage);
                    response = PublicEve.Market.GetRegionOrders(station.Region_id, null, MarketOrderType.Sell, page + 1).Execute();
                }
                orderPage = JsonConvert.DeserializeObject<List<EveOrder>>(response.Body);
                orderList.AddRange(orderPage);
            }
            return orderList;
        }

        public void GetItemNamesForRegions()
        {
            List<int> purchaseItemIDs, sellItemIDs;
            sellItemNames.Clear();
            EsiResponse response;

            purchaseItemIDs = GetItemIDs(purchaseStation.Region_id);
            sellItemIDs = GetItemIDs(sellStation.Region_id);

            List<int> itemIDs = new List<int>();
            
            itemIDs.AddRange(purchaseItemIDs);
            itemIDs = itemIDs.Intersect(sellItemIDs).ToList<int>();
            List<int> slice = new List<int>();
            int increment = 1000;
            for (int i = 0; i < itemIDs.Count; i += increment)
            {
                slice = itemIDs.GetRange(i, Math.Min(increment, itemIDs.Count - i));
                response = PublicEve.Universe.GetTypeNamesAndCategories(slice).Execute();
                sellItemNames.AddRange(JsonConvert.DeserializeObject<List<EveName>>(response.Body));
            }
        }

        private List<int> GetItemIDs(int region)
        {
            EsiResponse response;
            response = PublicEve.Market.GetMarketTypes(region).Execute();
            if (response.Code != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            //List<int> itemIDPage = new List<int>();
            List<int> itemIDs = new List<int>();

            int pages = response.Headers.Pages;
            for (int page = 1; page < pages; page++)
            {
                itemIDs.AddRange(JsonConvert.DeserializeObject<List<int>>(response.Body).Distinct().ToList());
                response = PublicEve.Market.GetMarketTypes(region, page + 1).Execute();
            }
            itemIDs.AddRange(JsonConvert.DeserializeObject<List<int>>(response.Body).Distinct().ToList());
            itemIDs = itemIDs.Distinct().ToList();

            return itemIDs;
        }

        public void GetSellItems()
        {
            List<EveOrder> purchaseOrder, sellorder;
            EsiResponse response;

            foreach (EveName name in sellItemNames)
            {
                //purchaseOrder = GetOrders(purchaseRegionID,;
                //if (response.Code != System.Net.HttpStatusCode.OK)
                //{
                //    return null;
                //}
            }
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
        public int Type_id { get; set; }
        public string Name { get; set; }
        public decimal Purchase_price { get; set; }
        public int Purchase_volume { get; set; }
        public decimal Sell_price { get; set; }
        public int Sell_volume { get; set; }
        public decimal Margin { get; set; }
        public int[] Competition { get; set; }
    }

}

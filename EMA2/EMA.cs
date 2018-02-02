using System;
using System.Collections.Generic;
using IO.Swagger.Api;
using IO.Swagger.Model;

namespace EMA2
{
    public sealed class EMA
    {

        private static readonly EMA instance = new EMA();

        private static SearchApi searchApi = new SearchApi();
        private static UniverseApi universeApi = new UniverseApi();
        private static MarketApi marketApi = new MarketApi();

        public SearchApi SearchApi { get => searchApi; }
        public UniverseApi UniverseApi { get => universeApi; }
        public MarketApi MarketApi { get => marketApi; }

        //public EMAStation PurchaseStation { get => purchaseStation; set => purchaseStation = value; }
        //private EMAStation purchaseStation;

        //public EMAStation SellStation { get => sellStation; set => sellStation = value; }
        //private EMAStation sellStation;

        //public List<EMASellItem> SellItems { get => sellItems; set => sellItems = value; }
        //private List<EMASellItem> sellItems;

        public List<PostUniverseNames200Ok> SellItemNames { get => sellItemNames; set => sellItemNames = value; }
        private List<PostUniverseNames200Ok> sellItemNames;

        private int retryDelay;
        public int RetryDelay { get => retryDelay; set => retryDelay = value; }

        private int retryMax;
        public int RetryMax { get => retryMax; set => retryMax = value; }

        private int historyRange;
        public int HistoryRange { get => historyRange; set => historyRange = value; }

        private double minMargin;
        public double MinMargin { get => minMargin; set => minMargin = value; }

        //private IProgress<EMAProgressInfo> progress;
        //public IProgress<EMAProgressInfo> Progress { get => progress; set => progress = value; }

        static EMA()
        {
        }

        private EMA()
        {
            //ESIEve.Public PublicEve = new ESIEve.Public();

            //SellItems = new List<EMASellItem>
            //{
            //    Capacity = 20000
            //};
            //PurchaseStation = new EMAStation();
            //SellStation = new EMAStation();
            SellItemNames = new List<PostUniverseNames200Ok>
            {
                Capacity = 20000
            };
            int timeout = IO.Swagger.Client.Configuration.Default.Timeout;
            RetryDelay = 400;
            RetryMax = 10;
            HistoryRange = 30;
            MinMargin = 0.05; //filter out all items with a margin less than 5%

        }
        public static EMA Instance
        {
            get
            {
                return instance;
            }
        }

    }
}

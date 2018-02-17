using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMA_MVVM.Models.EveModel;


namespace EMA_MVVM.Models.EmaModel
{
    class Ema
    {
        #region properties
        private static readonly Ema ema = new Ema();
        private Eve eve = Eve.Instance;

        private EmaStation purchasteStation;
        private EmaStation sellStation;

    
        #endregion properties

        static Ema()
        {
        }

        private Ema()
        {
            /*
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
            */

        }
        public static Ema Instance
        {
            get
            {
                return ema;
            }
        }

        class StationSearch
        {
            //
            // Contains a list of ids and names
            //
        }


    }
}

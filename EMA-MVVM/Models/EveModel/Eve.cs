using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IO.Swagger.Api;

namespace EMA_MVVM.Models.EveModel
{
    class Eve
    {
        private static readonly Eve eve = new Eve();

        // Define fields and properties fpr the swagger api
        private static SearchApi searchApi = new SearchApi();
        private static UniverseApi universeApi = new UniverseApi();
        private static MarketApi marketApi = new MarketApi();

        public SearchApi SearchApi { get => searchApi; }
        public UniverseApi UniverseApi { get => universeApi; }
        public MarketApi MarketApi { get => marketApi; }

        static Eve()
        {
        }

        private Eve()
        {
        }
        public static Eve Instance { get => eve; }

        public static bool IsStatusOK(int statusCode)
        {
            return statusCode.Equals((int)HttpStatusCode.OK);
        }

    }
}

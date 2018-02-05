using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace EMA_WPF
{
    public sealed class Eve
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

        public class Search
        {
            private List<int?> agent;
            public List<int?> Agent { get => agent; }

            private List<int?> alliance;
            public List<int?> Alliance { get => alliance; }

            private List<int?> character;
            public List<int?> Character { get => character; }

            private List<int?> constellation;
            public List<int?> Constellation { get => constellation;}

            private List<int?> corporation;
            public List<int?> Corporation { get => corporation; }

            private List<int?> faction;
            public List<int?> Faction { get => faction; }

            private List<int?> inventoryType;
            public List<int?> InventoryType { get => inventoryType; }

            private List<int?> region;
            public List<int?> Region { get => region; }

            private List<int?> solarSystem;
            public List<int?> SolarSystem { get => solarSystem; }

            private List<int?> station;
            public List<int?> Station { get => station;  }

            private static bool IsStatusOK(int statusCode)
            {
                return statusCode.Equals((int)HttpStatusCode.OK);
            }

            private Search()
            {
                agent = null;
                alliance = null;
                character = null;
                constellation = null;
                corporation = null;
                faction = null;
                inventoryType = null;
                region = null;
                solarSystem = null;
                station = null;
            }
            public Search(string searchString, List<string> categories) : this()
            {
                ApiResponse<GetSearchOk> response = eve.SearchApi.GetSearchWithHttpInfo(categories, searchString);
                if (!IsStatusOK(response.StatusCode)) return;
                agent = response.Data.Agent;
                alliance = response.Data.Alliance;
                character = response.Data.Character;
                constellation = response.Data.Constellation;
                corporation = response.Data.Corporation;
                faction = response.Data.Faction;
                inventoryType = response.Data.InventoryType;
                region = response.Data.Region;
                solarSystem = response.Data.SolarSystem;
                station = response.Data.Station;
            }
        }


        public class Category
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public bool Published { get; set; }
            public List<int> Groups { get; set; }
        }

        public class Constellation
        {
            /*
            {
                "application/json": {
                "constellation_id": 20000009,
                "name": "Mekashtad",
                "position": {
                    "x": 67796138757472320,
                    "y": -70591121348560960,
                    "z": -59587016159270070
                },
                "region_id": 10000001,
                "systems": [
                20000302,
                20000303
                ]
            }
            */
            public int Constellation_id { get; set; }
            public string Name { get; set; }
            public Eve.Position Position { get; set; }
            public int Region_id { get; set; }
            public List<int> Systems { get; set; }
        }

        public class Group
        {
            public int Group_id { get; set; }
            public string Name { get; set; }
            public bool Published { get; set; }
            public int Category { get; set; }
            public List<int> Types { get; set; }
        }

        public class EveName
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public PostUniverseNames200Ok.CategoryEnum Category { get; set; }
        }

        public class Planet
        {
            public int Id { get; set; }
            public List<int> Moons { get; set; }
        }

        public class Order
        {
            /*
            {
                "order_id": 5050990626,
                "type_id": 24690,
                "location_id": 60003760,
                "volume_total": 10,
                "volume_remain": 4,
                "min_volume": 1,
                "price": 187082695.41,
                "is_buy_order": false,
                "duration": 30,
                "issued": "2018-01-20T08:20:44Z",
                "range": "region"
            }
            */
            public long Order_id { get; set; }
            public int Type_id { get; set; }
            public int Location_id { get; set; }
            public int Volume_total { get; set; }
            public int Volume_remain { get; set; }
            public int Min_volume { get; set; }
            public decimal Price { get; set; }
            public bool Is_buy_order { get; set; }
            public int Duration { get; set; }
            public DateTime Issued { get; set; }
            public string Range { get; set; }
        }

        public class Position
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }

        public class Region
        {
            /*
                "application/json": {
                "region_id": 10000042,
                "name": "Metropolis",
                "description": "It has long been an established fact of civilization...",
                "constellations": [
                    20000302,
                    20000303
                ]
            */
            public int Region_id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public List<int> Constellations { get; set; }
        }

        public class Station
        {
            /*"{\"station_id\":60002959,
             * \"name\":\"Jita IV - Moon 10 - Caldari Constructions Production Plant\",
             * \"type_id\":1529,
             * \"position\":{\"x\":-105999360000.0,\"y\":-18516664320.0,\"z\":435947888640.0},
             * \"system_id\":30000142,
             * \"reprocessing_efficiency\":0.5,
             * \"reprocessing_stations_take\":0.05,
             * \"max_dockable_ship_volume\":50000000.0,
             * \"office_rental_cost\":4595833.0,
             * \"services\":
             *  [\"bounty-missions\",
             *   \"courier-missions\",
             *   \"reprocessing-plant\",
             *   \"market\",
             *   \"repair-facilities\",
             *   \"factory\",\"fitting\",
             *   \"news\",
             *   \"storage\",
             *   \"insurance\",
             *   \"docking\",
             *   \"office-rental\",
             *   \"loyalty-point-store\",
             *   \"navy-offices\"],
             * \"owner\":1000026,
             * \"race_id\":1}" 
            */
            public int Station_id { get; set; }
            public string Name { get; set; }
            public int Type_id { get; set; }
            public Position Position { get; set; }
            public int System_id { get; set; }
            public decimal Reprocessing_efficiency { get; set; }
            public decimal Reprocessing_stations_take { get; set; }
            public float Max_dockable_ship_volume { get; set; }
            public decimal Office_rental_cost { get; set; }
            public List<string> Services { get; set; }
            public int Owner { get; set; }
            public int Race_id { get; set; }
        }

        public class System
        {
            /*
            {
                "application/json": {
                "system_id": 30000003,
                "name": "Akpivem",
                "position": {
                  "x": -91174141133075340,
                  "y": 43938227486247170,
                  "z": -56482824383339900
                },
                "security_status": 0.8462923765182495,
                "constellation_id": 20000001,
                "planets": [
                  {
                    "planet_id": 40000041,
                    "moons": [
                      40000042
                    ]
                  },
                  {
                    "planet_id": 40000043
                  }
                ],
                "stargates": [
                  50000342
                ],
                "star_id": 40000040,
                "security_class": "B"
              }
            */
            public int System_id { get; set; }
            public string Name { get; set; }
            public Position Position { get; set; }
            public decimal Security_Status { get; set; }
            public int Constellation_id { get; set; }
            public List<Planet> Planets { get; set; }
            public List<int> Stargates { get; set; }
            public int Star_id { get; set; }
            public string Security_class { get; set; }
        }

        public class Type
        {
            public int Type_id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Published { get; set; }
            public int Groupid { get; set; }
        }

    }


}

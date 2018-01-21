using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMA_WPF
{
    class EMA
    {
    }

    public class EMAConstellation
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
        public EMAPosition Position { get; set; }
        public int Region_id { get; set; }
        public List<int> Systems { get; set; }
    }

    public class EMAName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }

    public class EMAPlanet
    {
        public int Id { get; set; }
        public List<int> Moons { get; set; }
    }

    public class User : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class EMAOrder : INotifyPropertyChanged
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
        private long order_id;
        public long Order_id
        {
            get { return this.order_id; }
            set
            {
                if (this.order_id != value)
                {
                    this.order_id = value;
                    this.NotifyPropertyChanged("Order_id");
                }
            }
        }
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }

    public class EMAPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public class EMARegion
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

    public class EMASearch
    {
        public List<int> Agent { get; set; }
        public List<int> Alliance { get; set; }
        public List<int> Character { get; set; }
        public List<int> Constellation { get; set; }
        public List<int> Corporation { get; set; }
        public List<int> Faction { get; set; }
        public List<int> Inventory_type { get; set; }
        public List<int> Region { get; set; }
        public List<int> Solar_system { get; set; }
        public List<int> Station { get; set; }
    }

    public class EMAStation
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
        public EMAPosition Position { get; set; }
        public int System_id { get; set; }
        public decimal Reprocessing_efficiency { get; set; }
        public decimal Reprocessing_stations_take { get; set; }
        public float Max_dockable_ship_volume { get; set; }
        public decimal Office_rental_cost { get; set; }
        public List<string> Services { get; set; }
        public int Owner { get; set; }
        public int Race_id { get; set; }
    }

    public class EMASystem
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
        public EMAPosition Position { get; set; }
        public decimal Security_Status { get; set; }
        public int Constellation_id { get; set; }
        public List<EMAPlanet> Planets { get; set; }
        public List<int> Stargates { get; set; }
        public int Star_id { get; set; }
        public string Security_class { get; set; }
    }

}

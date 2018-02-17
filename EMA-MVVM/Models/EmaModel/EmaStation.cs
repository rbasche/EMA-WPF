using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMA_MVVM.Models.EmaModel
{
    class EmaStation
    {
        //
        // Stores the basic information about a station
        //
        private int stationId;
        private string stationName;
        private int regionId;
        private string regionName;

        public int StationIdId { get => stationId; set => stationId = value; }
        public string StationName { get => stationName; set => stationName = value; }
        public int RegionIdId { get => regionId; set => regionId = value; }
        public string RegionName { get => regionName; set => regionName = value; }

    }
}

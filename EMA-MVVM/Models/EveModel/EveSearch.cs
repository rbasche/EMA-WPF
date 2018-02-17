using IO.Swagger.Client;
using IO.Swagger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMA_MVVM.Models.EveModel
{
    class EveSearch
    {
        private Eve eve;

        private List<int?> agent;
        public List<int?> Agent { get => agent; }

        private List<int?> alliance;
        public List<int?> Alliance { get => alliance; }

        private List<int?> character;
        public List<int?> Character { get => character; }

        private List<int?> constellation;
        public List<int?> Constellation { get => constellation; }

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
        public List<int?> Station { get => station; }

        private EveSearch()
        {
            eve = Eve.Instance;
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
        public EveSearch(string searchString, List<string> categories) : this()
        {
            ApiResponse<GetSearchOk> response = eve.SearchApi.GetSearchWithHttpInfo(categories, searchString);
            if (!Eve.IsStatusOK(response.StatusCode)) return;
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
}

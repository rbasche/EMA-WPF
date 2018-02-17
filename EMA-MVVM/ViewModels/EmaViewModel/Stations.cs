using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMA_MVVM.Models.EmaModel;

namespace EMA_MVVM.ViewModels.EmaViewModel
{
    class Stations
    {
        private List<EmaStation> purchaseStations;
        private List<EmaStation> sellStations;
        public List<EmaStation> PurchaseStations { get => purchaseStations; set => purchaseStations = value; }
        public List<EmaStation> SellStations;
    }
}

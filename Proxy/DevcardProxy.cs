using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Catan.Proxy
{
    /// <summary>
    ///     A proxy shared by client and service.  This is in both projects and can be found in https://github.com/joelong/catan
    /// </summary>
    public partial class CatanProxy : IDisposable
    {
        public Task<PlayerResources> DevCardPurchase(string gameName, string playerName)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/devcard/{gameName}/{playerName}";

            return Post<PlayerResources>(url, null);
        }
        public Task<PlayerResources> PlayYearOfPlenty(string gameName, string playerName, TradeResources tr)
        {

            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/devcard/play/yearofplenty/{gameName}/{playerName}";

            return Post<PlayerResources>(url, Serialize(tr));
        }

        public Task<PlayerResources> KnightPlayed(string gameName, string playerName, KnightPlayedLog log)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/devcard/play/knight/{gameName}/{playerName}";

            return Post<PlayerResources>(url, Serialize(log));
        }

        public Task<PlayerResources> PlayRoadBuilding(string gameName, string playerName)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/devcard/play/roadbuilding/{gameName}/{playerName}";

            return Post<PlayerResources>(url, null);
        }
        public Task<PlayerResources> PlayMonopoly(string gameName, string playerName, ResourceType resourceType)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/devcard/play/monopoly/{gameName}/{playerName}/{resourceType}";

            return Post<PlayerResources>(url, null);
        }

    }
}

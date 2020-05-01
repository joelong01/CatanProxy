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

        public Task<PlayerResources> TradeGold(string gameName, string playerName, TradeResources tradeResources)
        {
            if (String.IsNullOrEmpty(playerName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/resource/tradegold/{gameName}/{playerName}";
            return Post<PlayerResources>(url, Serialize(tradeResources));
        }




        public Task<PlayerResources> GetResources(string gameName, string playerName)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/resource/{gameName}/{playerName}";
            return Get<PlayerResources>(url);
        }


        /// <summary>
        ///     Takes resources (Ore, Wheat, etc.) from global pool and assigns to playerName
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerName"></param>
        /// <param name="resources"></param>
        /// <returns></returns>

        public Task<PlayerResources> GrantResources(string gameName, string playerName, TradeResources resources)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new Exception("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/resource/grant/{gameName}/{playerName}";
            var body = CatanProxy.Serialize<TradeResources>(resources);
            return Post<PlayerResources>(url, body);
        }
        public Task<PlayerResources> ReturnResource(string gameName, string playerName, TradeResources resources)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new Exception("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/resource/return/{gameName}/{playerName}";
            var body = CatanProxy.Serialize<TradeResources>(resources);
            return Post<PlayerResources>(url, body);
        }

        public Task<PlayerResources> UndoGrantResource(string gameName, ResourceLog lastLogRecord)
        {
            if (lastLogRecord is null)
            {
                throw new Exception("log record can't be null");
            }
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(lastLogRecord.PlayerName))
            {
                throw new Exception("names can't be null or empty");
            }

            string url = $"{HostName}/api/catan/resource/undo/{gameName}";
            var body = CatanProxy.Serialize<ResourceLog>(lastLogRecord);
            return Post<PlayerResources>(url, body);
        }
        public Task<List<PlayerResources>> Trade(string gameName, string fromPlayer, TradeResources from, string toPlayer, TradeResources to)
        {
            if (String.IsNullOrEmpty(fromPlayer) || String.IsNullOrEmpty(toPlayer) || String.IsNullOrEmpty(gameName))
            {
                throw new Exception("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/resource/trade/{gameName}/{fromPlayer}/{toPlayer}";
            var body = CatanProxy.Serialize<TradeResources[]>(new TradeResources[] { from, to });
            return Post<List<PlayerResources>>(url, body);
        }
    }
}

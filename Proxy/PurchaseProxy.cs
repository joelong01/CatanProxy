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
        public Task<PlayerResources> RefundEntitlement(string gameName, PurchaseLog log)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            if (log is null)
            {
                throw new ArgumentException("Purchase Log cannot be null in RefundEntitlment");
            }
            string url = $"{HostName}/api/catan/purchase/refund/{gameName}";

            return Post<PlayerResources>(url, Serialize<PurchaseLog>(log));
        }
        public Task<PlayerResources> BuyEntitlement(string gameName, string playerName, Entitlement entitlement)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/purchase/{gameName}/{playerName}/{entitlement}";

            return Post<PlayerResources>(url, null);
        }

    }
}

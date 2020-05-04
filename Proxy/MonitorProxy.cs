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
        public async Task<List<LogHeader>> Monitor(string gameName, string playerName)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/monitor/{gameName}/{playerName}";
            string json = await Get<string>(url);

            ServiceLogCollection serviceLogCollection = CatanProxy.Deserialize<ServiceLogCollection>(json);
            List<LogHeader> records = ParseLogRecords(serviceLogCollection);
            //Debug.WriteLine($"[Game={gameName}] [Player={playerName}] [LogCount={logList.Count}]");
            return records;
        }

        public async Task<List<LogHeader>> GetAllLogs(string gameName, string playerName, int startAt)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new Exception("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/monitor/logs/{gameName}/{playerName}/{startAt}";
            string json = await Get<string>(url);
            if (String.IsNullOrEmpty(json)) return null;

            ServiceLogCollection serviceLogCollection = CatanProxy.Deserialize<ServiceLogCollection>(json);
            List<LogHeader> records = ParseLogRecords(serviceLogCollection);
            return records;
        }
        // JsonDocument.Parse(json).RootElement.GetProperty("typeName").GetString()
        public Task<T> PostClientLog<T>(string gameName, string playerName, T logEntry)
        {

            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/monitor/postclientlog/{gameName}/{playerName}";

            string json = Serialize<object>((object)logEntry);

            Task<T> returnTask = Post<T>(url, json);
            if (returnTask == null)
            {
                Debug.WriteLine("Post call in SendClientLog failed.");
            }

            return returnTask;

        }

    }
}

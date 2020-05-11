using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catan.Proxy
{
    public partial class CatanProxy : IDisposable
    {
        public object JsonConvert { get; private set; }
        private static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

        /// <summary>
        ///     Joing a session and return a list of player names
        /// </summary>
        public Task<SessionInfo> JoinSession(string sessionId, string playerName)
        {

            if (String.IsNullOrEmpty(sessionId) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/session/join/{sessionId}/{playerName}";

            return Post<SessionInfo>(url, null);

        }
        public Task<List<SessionInfo>> CreateSession(SessionInfo sessionInfo)
        {
            if (sessionInfo == null)
            {
                throw new ArgumentException("SessionInfo can't be null");
            }
            if (String.IsNullOrEmpty(sessionInfo.Creator) || String.IsNullOrEmpty(sessionInfo.Description) || String.IsNullOrEmpty(sessionInfo.Id))
            {
                throw new ArgumentException("sessionId and description can't be null or empty");
            }

            string url = $"{HostName}/api/catan/session/";

            return Post<List<SessionInfo>>(url, Serialize(sessionInfo));
        }
        public Task<List<SessionInfo>> DeleteSession(string sessionId)
        {
            if (String.IsNullOrEmpty(sessionId) )
            {
                throw new ArgumentException("sessionId and description can't be null or empty");
            }

            string url = $"{HostName}/api/catan/session/{sessionId}";

            return Delete<List<SessionInfo>>(url);
        }
        public Task<List<SessionInfo>> GetSessions()
        {
            string url = $"{HostName}/api/catan/session";
            return Get<List<SessionInfo>>(url);
        }
        public Task<List<string>> GetPlayers(string sessionId)
        {
            if (String.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/session/players/{sessionId}";

            return Get<List<string>>(url);

        }
        public async Task<bool> PostLogMessage(string sessionName, CatanMessage message)
        {
            if (String.IsNullOrEmpty(sessionName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            if (message == null || message.Data == null)
            {
                throw new ArgumentException("LogHeader cannot be null");
            }

            if (String.IsNullOrEmpty(message.TypeName))
            {
                message.TypeName = message.Data.GetType().FullName;
            }

            string url = $"{HostName}/api/catan/session/message/{sessionName}";
         
            
            CatanMessage returnedMessage = await Post<CatanMessage>(url, CatanProxy.Serialize(message));
            return (returnedMessage != null);
           
        }
        /// <summary>
        ///     does a hanging Get for the Logs from the service
        ///     NOTE:  the .Data property is not Deserialized from JsonElement because the nuget package won't have the right types.
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public async Task<List<CatanMessage>> Monitor(string gameName, string playerName)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/session/monitor/{gameName}/{playerName}";
            string json = await Get<string>(url);

            List<CatanMessage> messageList = JsonSerializer.Deserialize(json, typeof(List<CatanMessage>), GetJsonOptions()) as List<CatanMessage>;
            return messageList;
        }

         public async Task<List<CatanMessage>> GetAllLogRecords(string gameName, string playerName)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/session/game/{gameName}/{playerName}";
            string json = await Get<string>(url);

            List<CatanMessage> messageList = JsonSerializer.Deserialize(json, typeof(List<CatanMessage>), GetJsonOptions()) as List<CatanMessage>;
            return messageList;
        }

        private object ParseCatanMessage(CatanMessage message)
        {
            Type type = CurrentAssembly.GetType(message.TypeName);
            if (type == null) throw new ArgumentException("Unknown type!");
            return JsonSerializer.Deserialize(message.Data.ToString(), type, GetJsonOptions()) as object;
        }
      
    }
}

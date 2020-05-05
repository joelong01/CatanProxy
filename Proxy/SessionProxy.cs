using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catan.Proxy
{
    public partial class CatanProxy : IDisposable
    {
        public object JsonConvert { get; private set; }

        /// <summary>
        ///     Joing a session and return a list of player names
        /// </summary>
        public Task<List<string>> JoinSession(string sessionId, string playerName)
        {

            if (String.IsNullOrEmpty(sessionId) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/session/join/{sessionId}/{playerName}";

            return Post<List<string>>(url, null);

        }
        public Task<List<SessionId>> CreateSession(string sessionId, string description)
        {
            if (String.IsNullOrEmpty(sessionId) || String.IsNullOrEmpty(description))
            {
                throw new ArgumentException("sessionId and description can't be null or empty");
            }

            string url = $"{HostName}/api/catan/session/{sessionId}/{description}";

            return Post<List<SessionId>>(url, null);
        }
        public Task<List<SessionId>> DeleteSession(string sessionId)
        {
            if (String.IsNullOrEmpty(sessionId) )
            {
                throw new ArgumentException("sessionId and description can't be null or empty");
            }

            string url = $"{HostName}/api/catan/session/{sessionId}";

            return Delete<List<SessionId>>(url);
        }
        public Task<List<SessionId>> GetSessions()
        {
            string url = $"{HostName}/api/catan/session";
            return Get<List<SessionId>>(url);
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
        public async Task<CatanMessage> PostLogMessage(string sessionName, object logHeader)
        {
            if (String.IsNullOrEmpty(sessionName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            if (logHeader == null)
            {
                throw new ArgumentException("LogHeader cannot be null");
            }

            string url = $"{HostName}/api/catan/session/message/{sessionName}";
            CatanMessage message = new CatanMessage()
            {
                TypeName = logHeader.GetType().FullName,
                Data = logHeader
            };
            
            CatanMessage returnedMessage = await Post<CatanMessage>(url, CatanProxy.Serialize(message));
            returnedMessage.Data = ParseCatanMessage(returnedMessage);
            return returnedMessage;

        }
        public async Task<List<CatanMessage>> GetLogs(string gameName, string playerName)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/session/monitor/{gameName}/{playerName}";
            string json = await Get<string>(url);

            List<CatanMessage> messageList = JsonSerializer.Deserialize(json, typeof(List<CatanMessage>), GetJsonOptions()) as List<CatanMessage>;
            foreach (CatanMessage message in messageList)
            {
                message.Data = ParseCatanMessage(message);
            }
            
            return messageList;
        }
        private object ParseCatanMessage(CatanMessage message)
        {
            Type type = Type.GetType(message.TypeName);
            if (type == null) throw new ArgumentException("Unknown type!");
            return JsonSerializer.Deserialize(message.Data.ToString(), type, GetJsonOptions()) as object;
        }
      
    }
}

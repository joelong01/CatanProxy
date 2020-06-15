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
        private static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

        private object ParseCatanMessage(CatanMessage message)
        {
            Type type = CurrentAssembly.GetType(message.DataTypeName);
            if (type == null) throw new ArgumentException("Unknown type!");
            return JsonSerializer.Deserialize(message.Data.ToString(), type, GetJsonOptions()) as object;
        }

        public object JsonConvert { get; private set; }

        public Task<List<GameInfo>> CreateGame(GameInfo gameInfo)
        {
            if (gameInfo == null)
            {
                throw new ArgumentException("GameInfo can't be null");
            }
            if (String.IsNullOrEmpty(gameInfo.Creator) || String.IsNullOrEmpty(gameInfo.Name) || gameInfo.Id == Guid.Empty)
            {
                throw new ArgumentException("gameId and description can't be null or empty");
            }

            string url = $"{HostName}/api/catan/game/";

            return Post<List<GameInfo>>(url, Serialize(gameInfo));
        }

        public Task<List<GameInfo>> DeleteGame(Guid id, string by)
        {
            if (id == Guid.Empty || id == null)
            {
                throw new ArgumentException("gameId can't be null or empty");
            }

            if (string.IsNullOrEmpty(by))
            {
                throw new ArgumentException("message", nameof(by));
            }

            string url = $"{HostName}/api/catan/game/{id}/{by}";

            return Delete<List<GameInfo>>(url);
        }

        public async Task<List<CatanMessage>> GetAllLogRecords(Guid id, string playerName)
        {
            if (String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty", "playerName");
            }

            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentException("can't be null", "id");
            }

            string url = $"{HostName}/api/catan/game/{id}/{playerName}";
            string json = await Get<string>(url);

            List<CatanMessage> messageList = JsonSerializer.Deserialize(json, typeof(List<CatanMessage>), GetJsonOptions()) as List<CatanMessage>;
            return messageList;
        }

        public Task<List<GameInfo>> GetGames()
        {
            string url = $"{HostName}/api/catan/game";
            return Get<List<GameInfo>>(url);
        }

        public Task<List<string>> GetPlayers(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentException("can't be null", "id");
            }
            string url = $"{HostName}/api/catan/game/players/{id}";

            return Get<List<string>>(url);
        }

        /// <summary>
        ///     Joing a game and return a list of player names
        /// </summary>
        public Task<GameInfo> JoinGame(GameInfo gameInfo, string playerName)
        {
            if (String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }

            string url = $"{HostName}/api/catan/game/join/{playerName}";
            string json = Serialize(gameInfo);
            return Post<GameInfo>(url, json);
        }
        /// <summary>
        ///     Joing a game and return a list of player names
        /// </summary>
        public Task<List<string>> LeaveGame(GameInfo gameInfo, string playerName)
        {
            if (String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }

            string url = $"{HostName}/api/catan/game/leave/{playerName}";
            string json = Serialize(gameInfo);           
            return Post<List<string>>(url, json);
        }

        /// <summary>
        ///     does a hanging Get for the Logs from the service
        ///     NOTE:  the .Data property is not Deserialized from JsonElement because the nuget package won't have the right types.
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public async Task<List<CatanMessage>> Monitor(GameInfo gameInfo, string playerName, bool delete)
        {
            if (String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/game/monitor/{gameInfo.Id}/{gameInfo.Name}/{playerName}/{gameInfo.RequestAutoJoin}/{delete}";
            string json = await Get<string>(url);

            List<CatanMessage> messageList = JsonSerializer.Deserialize(json, typeof(List<CatanMessage>), GetJsonOptions()) as List<CatanMessage>;
            return messageList;
        }

        /// <summary>
        ///     does a hanging Get for changes to Games
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public async Task<List<GameInfo>> MonitorGames()
        {
            string url = $"{HostName}/api/catan/game/monitor/games";
            List<GameInfo> games = await Get<List<GameInfo>>(url);
            return games;
        }

        public Task BroadcastMessage(Guid id, CatanMessage message)
        {
            if (id == Guid.Empty || id == null)
            {
                throw new ArgumentException("it can't be null or empty", "id");
            }
            if (message == null || message.Data == null)
            {
                throw new ArgumentException("LogHeader cannot be null", "message");
            }

            if (String.IsNullOrEmpty(message.DataTypeName))
            {
                message.DataTypeName = message.Data.GetType().FullName;
            }

            string url = $"{HostName}/api/catan/game/message/{id}";
            string json = CatanProxy.Serialize(message);

            return Post<CatanMessage>(url, json);
        }

        public Task SendPrivateMessage(Guid id, string to, CatanMessage message)
        {
            if (id == Guid.Empty || id == null)
            {
                throw new ArgumentException("it can't be null or empty", "id");
            }

            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException("message", nameof(to));
            }

            if (message == null || message.Data == null)
            {
                throw new ArgumentException("LogHeader cannot be null", "message");
            }
            

            if (String.IsNullOrEmpty(message.DataTypeName))
            {
                message.DataTypeName = message.Data.GetType().FullName;
            }

            string url = $"{HostName}/api/catan/game/privatemessage/{id}/{to}";
            string json = CatanProxy.Serialize(message);

            return Post<CatanMessage>(url, json);
        }

        public Task<List<GameInfo>> StartGame(GameInfo gameInfo)
        {
            if (gameInfo == null)
            {
                throw new ArgumentException("GameInfo can't be null");
            }
            if (String.IsNullOrEmpty(gameInfo.Creator) || String.IsNullOrEmpty(gameInfo.Name) || gameInfo.Id == Guid.Empty)
            {
                throw new ArgumentException("gameId and description can't be null or empty");
            }

            gameInfo.Started = true;

            string url = $"{HostName}/api/catan/Game/start";
            string json = Serialize(gameInfo);

            return Post<List<GameInfo>>(url, json);
        }
    }
}

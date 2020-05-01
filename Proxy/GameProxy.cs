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

        public Task<StateChangeLog> SetState(string gameName, string playerName, LogStateTranstion lst)
        {

            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/game/state/{gameName}/{playerName}";

            return Post<StateChangeLog>(url, Serialize(lst));

        }

        public Task<GameLog> JoinGame(string gameName, string playerName)
        {

            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/game/joingame/{gameName}/{playerName}";

            return Post<GameLog>(url, null);

        }
        public Task<List<string>> CreateGame(string gameName, GameInfo gameInfo)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            if (gameInfo == null)
            {
                throw new ArgumentException("gameInfo can't be null");
            }
            string url = $"{HostName}/api/catan/game/create/{gameName}";

            return Post<List<string>>(url, CatanProxy.Serialize<GameInfo>(gameInfo));
        }
        public Task<object> GetGameData(string gameName)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/game/gamedata/{gameName}";
            return Get<object>(url);
        }
        public Task<List<string>> GetGames()
        {
            string url = $"{HostName}/api/catan/game";

            return Get<List<string>>(url);

        }
        public Task<List<string>> GetUsers(string gameName)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }

            string url = $"{HostName}/api/catan/game/users/{gameName}";

            return Get<List<string>>(url);

        }
        public Task<GameInfo> GetGameInfo(string gameName)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }

            string url = $"{HostName}/api/catan/game/gameInfo/{gameName}";

            return Get<GameInfo>(url);

        }
        public Task<T> PostUndoRequest<T>(CatanRequest undoRequest)
        {
            if (undoRequest == null || String.IsNullOrEmpty(undoRequest.Url))
            {
                throw new Exception("names can't be null or empty");
            }
            string url = $"{HostName}/{undoRequest.Url}";
            string body = CatanProxy.Serialize<object>(undoRequest.Body);
            return Post<T>(url, body);
        }


        public Task<CatanResult> DeleteGame(string gameName)
        {

            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/game/{gameName}";

            return Delete<CatanResult>(url);


        }
        public Task<CatanResult> DeleteAllTestGames()
        {

            string url = $"{HostName}/api/catan/game/alltestgames";
            return Delete<CatanResult>(url);

        }

        public Task<string> StartGame(string gameName)
        {
            if (String.IsNullOrEmpty(gameName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            string url = $"{HostName}/api/catan/game/start/{gameName}";
            return Post<string>(url, null);
        }
        public async Task<RandomBoardLog> PostRandomBoard(string gameName, string playerName, RandomBoardLog log)
        {
            if (String.IsNullOrEmpty(gameName) || String.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("names can't be null or empty");
            }
            if (log == null)
            {
                throw new ArgumentException("board can't be null");
            }

            string url = $"{HostName}/api/catan/game/board/{gameName}/{playerName}";
            string json = Serialize(log);
            var obj = await Post<object>(url, json );
            var logRecord = ParseLogRecord(obj);

            return logRecord as RandomBoardLog;

        }
    }
}

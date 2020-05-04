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

        public HttpClient Client { get; set; } = new HttpClient() { Timeout = TimeSpan.FromDays(1) };
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(TimeSpan.FromDays(1));
        public TimeSpan Timeout { get => Client.Timeout; set => Client.Timeout = value; }
        public string HostName { get; set; } // "http://localhost:5000";
        public CatanResult LastError { get; set; } = null;
        public string LastErrorString { get; set; } = "";
        public static string ProxyVersion { get; } = "1.11";

        public CatanProxy()
        {
        }
        private async Task<T> Post<T>(string url, string body)
        {
            if (String.IsNullOrEmpty(HostName))
            {
                throw new ArgumentException("HostName cannot be null!");
            }

            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentException("the URL can't be null or empty");
            }



            LastError = null;
            LastErrorString = "";
            string json = "";

            try
            {
                HttpResponseMessage response;
                if (body != null)
                {
                    response = await Client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"), _cts.Token);
                }
                else
                {
                    response = await Client.PostAsync(url, new StringContent("", Encoding.UTF8, "application/json"));
                }

                if (typeof(T) == typeof(void)) return default;
                
                json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(string))
                    {
                        T workaround = (T)(object)json;
                        return workaround;
                    }

                    T obj = CatanProxy.Deserialize<T>(json);
                    return obj;
                }
                else
                {
                    LastErrorString = json;
                    LastError = ParseCatanResult(json);
                    return default;
                }
            }
            catch (Exception e)
            {
                LastErrorString = $"Url: {url} Response: {json} Exception:{e}";
                return default;
            }
        }
        private async Task<T> Delete<T>(string url)
        {

            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentException("the URL can't be null or empty");
            }



            LastError = null;
            LastErrorString = "";
            string json = "";
            try
            {

                var response = await Client.DeleteAsync(url, _cts.Token);
                json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    T obj = CatanProxy.Deserialize<T>(json);
                    return obj;
                }
                else
                {
                    LastErrorString = json;
                    try
                    {
                        LastError = ParseCatanResult(json);
                    }
                    catch { }
                    return default;
                }
            }
            catch (Exception e)
            {
                LastErrorString = $"Url: {url} Response: {json} Exception:{e}";
                return default;

            }
        }
        private async Task<T> Get<T>(string url)
        {


            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentException("the URL can't be null or empty");
            }



            LastError = null;
            LastErrorString = "";
            string json = "";
            try
            {
                var response = await Client.GetAsync(url, _cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();

                    if (typeof(T) == typeof(string))
                    {
                        T workaround = (T)(object)json;
                        return workaround;
                    }
                    T obj = CatanProxy.Deserialize<T>(json);
                    return obj;
                }
                else
                {
                    Debug.WriteLine($"Error grom GetAsync: {response} {Environment.NewLine} {response.ReasonPhrase}");
                }


            }
            catch (HttpRequestException)
            {
                // see if there is a Catan Exception

                LastErrorString = json;
                try
                {
                    LastError = ParseCatanResult(json);
                }
                catch
                {
                    return default;
                }

            }
            catch (Exception e)
            {
                LastErrorString = $"Url: {url} Response: {json} Exception:{e}";
                return default;
            }
            return default;
        }

        private List<LogHeader> ParseLogRecords(ServiceLogCollection serviceLogCollection)
        {
            List<LogHeader> records = new List<LogHeader>();
            foreach (var unparsedObject in serviceLogCollection.LogRecords)
            {
                var logRecord = ParseLogRecord((JsonElement)(object)unparsedObject);
                records.Add(logRecord);
            }
            return records;
        }

        private LogHeader ParseLogRecord(JsonElement unparsedObject)
        {
            string action = ((JsonElement)unparsedObject).GetProperty("action").GetString();
            if (String.IsNullOrEmpty(action)) return null;
            bool ret = Enum.TryParse<CatanAction>(action, out CatanAction catanAction);
            if (!ret) return null;

            string json = unparsedObject.ToString();
            switch (catanAction)
            { 
                case CatanAction.Purchased:
                    PurchaseLog purchaseLog = CatanProxy.Deserialize<PurchaseLog>(json);
                    ParseCatanRequest(purchaseLog.UndoRequest);
                    return purchaseLog;
                case CatanAction.GameDeleted:
                case CatanAction.AddPlayer:
                case CatanAction.Started:
                    GameLog gameLog = CatanProxy.Deserialize<GameLog>(json);
                    return gameLog;
                case CatanAction.PlayedDevCard:                   
                    return PlayedDevCardModel.Deserialize(unparsedObject);
                case CatanAction.PlayerTrade:
                case CatanAction.TradeGold:
                case CatanAction.GrantResources:
                    ResourceLog resourceLog = CatanProxy.Deserialize<ResourceLog>(json);
                    return resourceLog;
                case CatanAction.CardTaken:
                    TakeLog takeLog = CatanProxy.Deserialize<TakeLog>(json);
                    return takeLog;
                case CatanAction.MeritimeTrade:
                    MeritimeTradeLog mtLog = CatanProxy.Deserialize<MeritimeTradeLog>(json);
                    return mtLog;
                case CatanAction.ChangedPlayer:
                    TurnLog tLog = CatanProxy.Deserialize<TurnLog>(json);
                    return tLog;
                case CatanAction.RandomizeBoard:
                    var boardLog = CatanProxy.Deserialize<RandomBoardLog>(json);
                    boardLog.RandomBoardSettings = CatanProxy.Deserialize<RandomBoardSettings>(boardLog.RandomBoardSettings.ToString()); // more "we don't do polymorphic deserialization
                    return boardLog;
                case CatanAction.ChangedState:
                    var stateChangeLog = CatanProxy.Deserialize<StateChangeLog>(json);
                    stateChangeLog.LogStateTranstion = CatanProxy.Deserialize<LogStateTranstion>(stateChangeLog.LogStateTranstion.ToString());
                    return stateChangeLog;

                case CatanAction.GameCreated:
                case CatanAction.Undefined:
                case CatanAction.TradeResources:
                default:
                    throw new Exception($"{catanAction} has no Deserializer! logEntry: {unparsedObject}");
            }
        }

        public void CancelAllRequests()
        {
            _cts.Cancel();
        }
        public void Dispose()
        {
            CancelAllRequests();
            Client.Dispose();
        }

        private CatanResult ParseCatanResult(string json)
        {
            if (String.IsNullOrEmpty(json)) return null;

            var result = CatanProxy.Deserialize<CatanResult>(json);
            ParseCatanRequest(result.CantanRequest);

            return result;
        }

        /// <summary>
        ///     System.Text.Json does not do polymorphic Deserialization. So we serialize the object and its type.  here 
        ///     we switch on the type and then covert the JSON returned by ASP.net to string and then deserialize it into the 
        ///     right type.
        /// </summary>
        /// <param name="unparsedRequest"></param>
        private void ParseCatanRequest(CatanRequest unparsedRequest)
        {
            if (unparsedRequest == null) return;
            switch (unparsedRequest.BodyType)
            {
                case BodyType.TradeResources:
                    unparsedRequest.Body = CatanProxy.Deserialize<TradeResources>(unparsedRequest.Body.ToString());
                    break;
                case BodyType.GameInfo:
                    unparsedRequest.Body = CatanProxy.Deserialize<GameInfo>(unparsedRequest.Body.ToString());
                    break;
                case BodyType.TradeResourcesList:
                    unparsedRequest.Body = CatanProxy.Deserialize<TradeResources[]>(unparsedRequest.Body.ToString());
                    break;
                case BodyType.None:
                default:
                    break;
            }
        }
        public static JsonSerializerOptions GetJsonOptions(bool indented = false)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = indented

            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
        static public string Serialize<T>(T obj, bool indented = false)
        {
            if (obj == null) return null;
            return JsonSerializer.Serialize<T>(obj, GetJsonOptions(indented));
        }
        static public T Deserialize<T>(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<T>(json, options);
        }
        static public LogHeader DeserializeLogHeader(string json)
        {
            LogHeader logHeader = Deserialize<LogHeader>(json);
            if (logHeader == null) return null;
            switch (logHeader.Action)
            {
                case CatanAction.Rolled:
                    break;
                case CatanAction.ChangedState:
                    break;
                case CatanAction.ChangedPlayer:
                    break;
                case CatanAction.Dealt:
                    break;
                case CatanAction.CardsLost:
                    break;
                case CatanAction.CardsLostToSeven:
                    break;
                case CatanAction.MissedOpportunity:
                    break;
                case CatanAction.DoneSupplemental:
                    break;
                case CatanAction.DoneResourceAllocation:
                    break;
                case CatanAction.RolledSeven:
                    break;
                case CatanAction.AssignedBaron:
                    break;
                case CatanAction.UpdatedRoadState:
                    break;
                case CatanAction.UpdateBuildingState:
                    break;
                case CatanAction.AssignedPirateShip:
                    break;
                case CatanAction.AddPlayer:
                    return CatanProxy.Deserialize<AddPlayerModel>(json);

                case CatanAction.SelectGame:
                    break;
                case CatanAction.InitialAssignBaron:
                    break;
                case CatanAction.None:
                    break;
                case CatanAction.SetFirstPlayer:
                    break;
                case CatanAction.RoadTrackingChanged:
                    break;
                case CatanAction.AddResourceCount:
                    break;
                case CatanAction.ChangedPlayerProperty:
                    break;
                case CatanAction.SetRandomTileToGold:
                    break;
                case CatanAction.ChangePlayerAndSetState:
                    break;
                case CatanAction.Started:
                    break;
                case CatanAction.RandomizeBoard:
                    break;
                default:
                    break;
            }

            return null;
        }
    }
}

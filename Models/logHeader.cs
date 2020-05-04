
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Catan.Proxy
{
    public enum LogType { Normal, Undo, Redo };
    public enum CatanAction
    {
        Rolled,
        ChangedState,
        ChangedPlayer,
        Dealt,
        CardsLost,
        CardsLostToSeven,
        MissedOpportunity,
        DoneSupplemental,
        DoneResourceAllocation,
        RolledSeven,
        AssignedBaron,
        UpdatedRoadState,
        UpdateBuildingState,
        AssignedPirateShip,
        AddPlayer,
        SelectGame,
        InitialAssignBaron,
        None,
        SetFirstPlayer,
        RoadTrackingChanged,
        AddResourceCount,
        ChangedPlayerProperty,
        SetRandomTileToGold,
        ChangePlayerAndSetState,
        Started,
        RandomizeBoard,
        Purchased,
        GameDeleted,
        PlayedDevCard,
        PlayerTrade,
        TradeGold,
        GrantResources,
        CardTaken,
        MeritimeTrade,
        GameCreated,
        Undefined,
        TradeResources
    };



    public interface ILogHeader
    {
        int PlayerIndex { get; set; }
        string PlayerName { get; set; }
        string GameName { get; set; }
        GameState OldState { get; set; }
        GameState NewState { get; set; }
        CatanAction Action { get; set; }
        Guid LogId { get; set; }
        DateTime Time { get; set; }
        string RequestUrl { get; set; }
        CatanRequest UndoRequest { get; set; }
        LogType LogType { get; set; }
        string TypeName { get; set; }


    }

   /// <summary>
   ///  This is the class that we send to the service to synchronize state.
   ///  it is Deserialized in the service.
   ///  
   ///  JsonElement is a LogHeader of some type
   ///  TypeName is the name of the derived LogHeader type
   ///  Sequence is set by the service and is the order of the log it has received
   ///  
   /// </summary>
    public class CatanMessage
    {
        public object Data { get; set; }
        public string TypeName { get; set; } = "";
        public int Sequence { get; set; } = 0;

    }


    public class LogHeader : ILogHeader
    {
        public LogType LogType { get; set; } = LogType.Normal;
        public int PlayerIndex { get; set; }
        public string PlayerName { get; set; }
        public string GameName { get; set; }
        public GameState OldState { get; set; }
        public GameState NewState { get; set; }
        public CatanAction Action { get; set; }
        public CatanGames CatanGame { get; set; }
        public Guid LogId { get; set; } = Guid.NewGuid();
        public DateTime Time { get; set; } = DateTime.Now;
        public string RequestUrl { get; set; } = "";
        public string TypeName { get; set; } 
        public CatanRequest UndoRequest { get; set; } = null; // the request to undo this action
        



        public LogHeader()
        {
            TypeName = this.GetType().FullName;

        }
        public string Serialize()
        {
            return CatanProxy.Serialize<object>(this);
        }

        //
        //  because of the way S.T.Json works, we have to Deserialize in two passes.
        //  first pass gets the header, then we switch on the Action and Deserialize
        //  the full object.
        //
        public static LogHeader DeserializeHeader(string json)
        {
            return CatanProxy.Deserialize<LogHeader>(json);
        }

    }


}

using System;
using System.Collections.Generic;


namespace Catan.Proxy
{
    public enum CatanError
    {
        DevCardsSoldOut,
        NoMoreResource,
        LimitExceeded,
        NoGameWithThatName,
        NoPlayerWithThatName,
        NotEnoughResourcesToPurchase,
        MissingData,
        BadTradeResources,
        NoResource,
        BadEntitlement,
        BadParameter,
        BadLogRecord,
        PlayerAlreadyRegistered,
        GameAlreadStarted,
        Unknown,
        InsufficientResource,
        Unexpected,
        NoError,
    }

    /// <summary>
    ///     returned by Monitor.  
    ///         Sequence number used to ensure that no records are missed at the client
    ///         Count is used to verify/test marshaling of LogRecords
    ///         LogRecords are the data you actually want
    ///         
    ///         LogRecords is a List<object> so that the Serializer will serialize
    ///         all of the information in the derived classes
    /// </summary>
    public class ServiceLogCollection
    {
        public int SequenceNumber { get; set; }
        public int Count { get; set; }
        public List<object> LogRecords { get; set; }
        public Guid CollectionId { get; set; }
    }






    public class ResourceLog : LogHeader
    {
        
        public PlayerResources PlayerResources { get; set; } // this is not needed for Undo, but is needed for each of the games to update their UI
        public TradeResources TradeResource { get; set; } // needed for Undo

        public override string ToString()
        {
            return base.ToString() + PlayerResources.ToString();
        }
    }



    public class TurnLog : LogHeader
    {
        public string NewPlayer { get; set; } = "";
        public TurnLog() { Action = CatanAction.ChangedPlayer; }
    }

    public class TradeLog : LogHeader
    {
        public TradeLog() { Action = CatanAction.TradeResources; }
        public TradeResources FromTrade { get; set; }
        public TradeResources ToTrade { get; set; }
        public PlayerResources FromResources { get; set; }
        public PlayerResources ToResources { get; set; }

        public string FromName { get; set; }
        public string ToName { get; set; }

    }
    public class TakeLog : LogHeader
    {
        public TakeLog() { Action = CatanAction.CardTaken; }
        public ResourceType Taken { get; set; }
        public PlayerResources FromResources { get; set; }
        public PlayerResources ToResources { get; set; }

        public string FromName { get; set; }
        public string ToName { get; set; }

    }


    public class MeritimeTradeLog : LogHeader
    {
        public MeritimeTradeLog() { Action = CatanAction.MeritimeTrade; }
        public ResourceType Traded { get; set; }
        public int Cost { get; set; }
        public PlayerResources Resources { get; set; }

    }
    public class PurchaseLog : LogHeader
    {
        public Entitlement Entitlement { get; set; }
        public PlayerResources PlayerResources { get; set; }
        public PurchaseLog() { Action = CatanAction.Purchased; }
        public override string ToString()
        {
            return $"[Entitlement={Entitlement}]" + base.ToString();
        }
    }
    public class GameLog : LogHeader
    {
        public ICollection<string> Players { get; set; }
        public GameInfo GameInfo { get; set; }
        public GameLog() { }
    }

    public class RandomBoardLog : LogHeader
    {
        public RandomBoardSettings RandomBoardSettings { get; set; } = null;
        public RandomBoardLog()
        {
            Action = CatanAction.RandomizeBoard;

        }
    }

    public class StateChangeLog : LogHeader
    {
        public object LogStateTranstion { get; set; } = null;

        public StateChangeLog()
        {

            Action = CatanAction.ChangedState;
        }

    }
}

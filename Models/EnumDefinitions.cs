using System;
using System.Collections.Generic;
using System.Text;

namespace Catan.Proxy
{
    public enum CatanGames { Regular, Expansion, Seafarers, Seafarers4Player };
    public enum GameType { Test, Normal };
    public enum TileOrientation { FaceDown, FaceUp, None };
    public enum HarborType { Sheep, Wood, Ore, Wheat, Brick, ThreeForOne, Uninitialized, None };
    public enum Entitlement { Undefined, DevCard, Settlement, City, Road }
    public enum GameState
    {
        Uninitialized,                      // 0
        WaitingForNewGame,                  // 1 
        Starting,                           // 2
        Dealing,                            // 3
        WaitingForStart,                    // 4
        AllocateResourceForward,            // 5
        AllocateResourceReverse,            // 6
        DoneResourceAllocation,             // 7
        WaitingForRoll,                     // 8
        Targeted,                           // 9
        LostToCardsLikeMonopoly,            // 10
        Supplemental,                       // 11
        DoneSupplemental,                   // 12
        WaitingForNext,                     // 13
        LostCardsToSeven,                   // 14
        MissedOpportunity,                  // 15
        GamePicked,                         // 16
        MustMoveBaron,                       // 17
        Unknown,
    };
    public enum ResourceType { Sheep, Wood, Ore, Wheat, Brick, GoldMine, Desert, Back, None, Sea };
    //
    // Back is needed because the resource control flips from its back to front..this makes a front look like the back of a dev card
    public enum DevCardType { Knight, VictoryPoint, YearOfPlenty, RoadBuilding, Monopoly, Unknown, Back };
    public enum BodyType
    {
        TradeResources,
        None,
        GameInfo,
        TradeResourcesList
    }
}

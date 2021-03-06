﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System;


namespace Catan.Proxy
{
   
    public class CatanRequest
    {
        public string Url { get; set; } = "";
        public object Body { get; set; } = null;
        public BodyType BodyType { get; set; } = BodyType.None;
        public CatanRequest() { }
        public CatanRequest(string u, object b, BodyType t) { Url = u; Body = b; BodyType = t; }
        public override string ToString()
        {
            return $"[BodyType={BodyType}][Url={Url}][Body={Body?.ToString()}]";
        }
    }

    public class GameInfo
    {
        public int MaxRoads { get; set; } = 15;
        public int MaxCities { get; set; } = 4;
        public int MaxSettlements { get; set; } = 5;
        public int MaxResourceAllocated { get; set; } = 19; // most aggregate resource per type
        public bool AllowShips { get; set; } = false;
        public int Knight { get; set; } = 14;
        public int VictoryPoint { get; set; } = 5;
        public int YearOfPlenty { get; set; } = 2;
        public int RoadBuilding { get; set; } = 2;
        public int Monopoly { get; set; } = 2;
        public int HarborCount { get; set; } = 9; // for "Regular Game"
        public int TileCount { get; set; } = 19; // for "Regular Game"
        public RandomBoardSettings BoardSettings { get; set; } = new RandomBoardSettings();
        public CatanGames GameName { get; set; } = CatanGames.Regular;
        public GameType GameType { get; set; } = GameType.Normal;
        public GameInfo() { }
        public GameInfo(GameInfo info)
        {
            MaxRoads = info.MaxRoads;
            MaxCities = info.MaxCities;
            MaxSettlements = info.MaxSettlements;
            MaxResourceAllocated = info.MaxResourceAllocated;
            AllowShips = info.AllowShips;
            Knight = info.Knight;
            VictoryPoint = info.Knight;
            YearOfPlenty = info.VictoryPoint;
            RoadBuilding = info.YearOfPlenty;
            Monopoly = info.Monopoly;

        }
        public static bool operator ==(GameInfo a, GameInfo b)
        {
            if (a is null)
            {
                if (b is null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (b is null) // a is not null!
            {
                return false;
            }

            return
                (
                    a.MaxRoads == b.MaxRoads &&
                    a.MaxCities == b.MaxCities &&
                    a.MaxSettlements == b.MaxSettlements &&
                    a.Knight == b.Knight &&
                    a.MaxResourceAllocated == b.MaxResourceAllocated &&
                    a.AllowShips == b.AllowShips &&
                    a.VictoryPoint == b.VictoryPoint &&
                    a.YearOfPlenty == b.YearOfPlenty &&
                    a.RoadBuilding == b.RoadBuilding &&
                    a.Monopoly == b.Monopoly
                );


        }
        public static bool operator !=(GameInfo a, GameInfo b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return (GameInfo)obj == this;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class CatanResult
    {
        private CatanRequest _request = new CatanRequest();
        private string request;

        public CatanRequest CantanRequest
        {
            get
            {
                return _request;
            }
            set
            {
                if (value != _request)
                {
                    _request = value;
                }
            }
        }

        public List<KeyValuePair<string, object>> ExtendedInformation { get; } = new List<KeyValuePair<string, object>>();
        public string Description { get; set; }
        public string FunctionName { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Request { get => _request.Url; set => request = value; }
        public Guid ID { get; set; } = Guid.NewGuid(); // this gives us an ID at creation time that survives serialization and is globally unique
        public CatanError Error { get; set; } = CatanError.Unknown;
        public string Version { get; set; } = CatanProxy.ProxyVersion;
        public CatanResult() // for the Serializer
        {

        }
       
        public CatanResult(CatanError error, [CallerMemberName] string fName = "", [CallerFilePath] string codeFile = "", [CallerLineNumber] int lineNumber = -1)
        {
            Error = error;
            FunctionName = fName;
            FilePath = codeFile;
            LineNumber = lineNumber;
        }
        public static bool operator ==(CatanResult a, CatanResult b)
        {
            if (a is null || b is null)
            {
                if (b is null && a is null)
                {
                    return true;
                }

                return false;
            }

            if (a.ExtendedInformation?.Count != b.ExtendedInformation?.Count)
            {
                return false;
            }
            if (a.ExtendedInformation != null)
            {
                if (b.ExtendedInformation == null) return false;
                for (int i = 0; i < a.ExtendedInformation?.Count; i++)
                {
                    if (a.ExtendedInformation[i].Key != b.ExtendedInformation[i].Key)
                    {
                        return false;
                    }

                    //
                    //  going to ignore the value for now
                }
            }

            return
                (
                    a.Description == b.Description &&
                    a.FunctionName == b.FunctionName &&
                    a.FilePath == b.FilePath &&
                    a.LineNumber == b.LineNumber &&
                    a.Request == b.Request &&
                    a.Error == b.Error
                 );

        }
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 97 + Description.GetHashCode();
            hash = hash * 97 + FunctionName.GetHashCode();
            hash = hash * 97 + FilePath.GetHashCode();
            hash = hash * 97 + LineNumber.GetHashCode();
            hash = hash * 97 + Request.GetHashCode();
            hash = hash * 97 + Error.GetHashCode();
            return hash;
        }
        public static bool operator !=(CatanResult a, CatanResult b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return (CatanResult)obj == this;
        }
    }



    public class TradeResources : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _wheat = 0;
        private int _wood = 0;
        private int _ore = 0;
        private int _sheep = 0;
        private int _brick = 0;
        private int _goldMine = 0;

        public TradeResources() { }
        //
        //  useful for the Resource Tests
        public static TradeResources operator +(TradeResources a, TradeResources b)
        {
            return new TradeResources()
            {
                Wheat = a.Wheat + b.Wheat,
                Wood = a.Wood + b.Wood,
                Ore = a.Ore + b.Ore,
                Sheep = a.Sheep + b.Sheep,
                Brick = a.Brick + b.Brick,
                GoldMine = a.GoldMine + b.GoldMine
            };
        }
        public TradeResources(TradeResources tradeResources)
        {
            Wheat = this.Wheat;
            Wood = this.Wood;
            Brick = this.Brick;
            Ore = this.Ore;
            Sheep = this.Sheep;
            GoldMine = this.GoldMine;

        }

        public int Wheat
        {
            get
            {
                return _wheat;
            }
            set
            {
                if (value != _wheat)
                {
                    _wheat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Wood
        {
            get
            {
                return _wood;
            }
            set
            {
                if (value != _wood)
                {
                    _wood = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Ore
        {
            get
            {
                return _ore;
            }
            set
            {
                if (value != _ore)
                {
                    _ore = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Sheep
        {
            get
            {
                return _sheep;
            }
            set
            {
                if (value != _sheep)
                {
                    _sheep = value;
                }
            }
        }

        public int Brick
        {
            get
            {
                return _brick;
            }
            set
            {
                if (value != _brick)
                {
                    _brick = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int GoldMine
        {
            get
            {
                return _goldMine;
            }
            set
            {
                if (value != _goldMine)
                {
                    _goldMine = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        public TradeResources GetNegated()
        {
            return new TradeResources()
            {
                Wheat = -Wheat,
                Wood = -Wood,
                Ore = -Ore,
                Sheep = -Sheep,
                Brick = -Brick,
                GoldMine = -GoldMine
            };
        }
        public bool Equivalent(TradeResources tradeResources)
        {
            if (Wheat != tradeResources.Wheat || Wood != tradeResources.Wood || Ore != tradeResources.Ore ||
                Sheep != tradeResources.Sheep || Brick != tradeResources.Brick || GoldMine != tradeResources.GoldMine)
            {

                return false;

            }

            return true;
        }

        [JsonIgnore]
        public int Total => Wheat + Wood + Brick + Ore + Sheep + GoldMine;
        public override string ToString()
        {
            return $"[Total={Total}][Ore={Ore}][Brick={Brick}][Wheat={Wheat}][Wood={Wood}][Sheep={Sheep}]";
        }
    }

    public class PlayerResources
    {
        public Guid ID { get; set; } = Guid.NewGuid(); // this gives us an ID at creation time that survives serialization and is globally unique
        public string PlayerName { get; set; } = "";
        public string GameName { get; set; } = "";
        public int Wheat { get; set; } = 0;
        public int Wood { get; set; } = 0;
        public int Ore { get; set; } = 0;
        public int Sheep { get; set; } = 0;
        public int Brick { get; set; } = 0;
        public int GoldMine { get; set; } = 0;
        public int Settlements { get; set; } = 0;
        public int Cities { get; set; } = 0;
        public int Roads { get; set; } = 0;

        /// <summary>
        ///     Unplayed Dev cards. Private information.
        /// </summary>
        public int Knights { get; set; } = 0; // # of knights that have NOT been played
        public int VictoryPoints { get; set; } = 0;
        public int YearOfPlenty { get; set; } = 0;
        public int RoadBuilding { get; set; } = 0;
        public int Monopoly { get; set; } = 0;
        [JsonIgnore]
        public int UnplayedDevCards => Knights + VictoryPoints + YearOfPlenty + RoadBuilding + Monopoly;
        public List<DevCardType> PlayedDevCards { get; set; } = new List<DevCardType>();  // the list of cards that have been played.  this is public information!


        [JsonIgnore]
        public int TotalResources => Wheat + Wood + Brick + Ore + Sheep + GoldMine;
        public override string ToString()
        {
            return $"[Total={TotalResources}][Ore={Ore}][Brick={Brick}][Wheat={Wheat}][Wood={Wood}][Sheep={Sheep}] [DevCards={PlayedDevCards?.Count}][Stuff={Settlements + Roads + Cities}]";
        }
        public PlayerResources() { }

        public int GetResourceCount(ResourceType rt)
        {
            switch (rt)
            {
                case ResourceType.Sheep:
                    return Sheep;
                case ResourceType.Wood:
                    return Wood;
                case ResourceType.Ore:
                    return Ore;
                case ResourceType.Wheat:
                    return Wheat;
                case ResourceType.Brick:
                    return Brick;
                case ResourceType.GoldMine:
                    return GoldMine;
                case ResourceType.Desert:
                case ResourceType.Back:
                case ResourceType.None:
                case ResourceType.Sea:
                default:
                    break;
            }
            return 0;
        }

        public bool Equivalent(TradeResources tradeResources)
        {
            if (Wheat != tradeResources.Wheat || Wood != tradeResources.Wood || Ore != tradeResources.Ore ||
                Sheep != tradeResources.Sheep || Brick != tradeResources.Brick || GoldMine != tradeResources.GoldMine)
            {

                return false;

            }

            return true;
        }
        public bool Equivalent(PlayerResources pr)
        {
            if (pr.PlayedDevCards == null)
            {
                if (this.PlayedDevCards != null)
                {
                    return false;
                }
            }

            if (PlayerName != pr.PlayerName || GameName != pr.GameName ||
                Wheat != pr.Wheat || Wood != pr.Wood || Ore != pr.Ore ||
                Sheep != pr.Sheep || Brick != pr.Brick || GoldMine != pr.GoldMine ||
                Settlements != pr.Settlements || Cities != pr.Cities || Roads != pr.Roads)
            {

                return false;

            }

            if (pr.PlayedDevCards != null)
            {
                if (pr.PlayedDevCards.Count != this.PlayedDevCards.Count)
                {
                    return false;
                }
                for (int i = 0; i < PlayedDevCards.Count; i++)
                {
                    if (!pr.PlayedDevCards[i].Equals(PlayedDevCards[i]))
                        return false;
                }
            }


            return true;



        }
    }

    public class RandomLists
    {
        public List<int> TileList { get; set; } = null;
        public List<int> NumberList { get; set; } = null;

        public RandomLists() { }
        public RandomLists(string saved)
        {
            Deserialize(saved);
        }

        public RandomLists(List<int> tiles, List<int> numbers)
        {
            TileList = tiles;
            NumberList = numbers;
        }


        public string Serialize()
        {
            return CatanProxy.Serialize<RandomLists>(this);
        }

        public static RandomLists Deserialize(string saved)
        {
            return CatanProxy.Deserialize<RandomLists>(saved);

        }


    }
    public class RandomBoardSettings
    {
        //
        // every TileGroup has a list that says where to put the tiles
        // and another list that says what number to put on the tiles
        //
        // the int here is the TileGroup Index - System.Text.Json currently only Deserializes Dictionaries keyed by strings.
        //
        public Dictionary<string, RandomLists> TileGroupToRandomListsDictionary { get; set; } = new Dictionary<string, RandomLists>();

        //
        //  every Board has a random list of harbors
        public List<int> RandomHarborTypeList { get; set; } = null;
        public RandomBoardSettings() { }

        public override string ToString()
        {
            return Serialize();
        }

        public RandomBoardSettings(Dictionary<string, RandomLists> Tiles, List<int> Harbors)
        {
            RandomHarborTypeList = Harbors;
            TileGroupToRandomListsDictionary = Tiles;
        }

        public string Serialize()
        {
            return CatanProxy.Serialize<RandomBoardSettings>(this);
        }

        public static RandomBoardSettings Deserialize(string saved)
        {
            return CatanProxy.Deserialize<RandomBoardSettings>(saved);

        }



    }

    public class LogStateTranstion
    {
        public GameState OldState { get; set; } = GameState.Uninitialized;
        public GameState NewState { get; set; } = GameState.Uninitialized;
        public List<int> RandomGoldTiles { get; set; } = new List<int>();

        public LogStateTranstion() { }

        public LogStateTranstion(GameState old, GameState newState)
        {
            OldState = old;
            NewState = newState;

        }

        public LogStateTranstion(string saved)
        {
            Deserialize(saved);
        }

        public override string ToString()
        {
            return CatanProxy.Serialize(this);

        }

        public static LogStateTranstion Deserialize(string json)
        {
            return CatanProxy.Deserialize<LogStateTranstion>(json);
        }

    }

}


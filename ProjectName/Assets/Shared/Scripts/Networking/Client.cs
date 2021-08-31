using Assets.Generic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Network {
    /// <summary>
    /// This is the main class that holds all data for connections. Please use this to hold data that will need to be syncable.
    /// </summary>
    [Serializable]
    public class Client
    {
        public byte ConnectionID {get; set;} //Player internal connection ID
        public GameState CurrentState;
        [NonSerialized]public bool IsConnected; //Is still connected to server?
        [NonSerialized]public bool IsReady; //Is ready for match?
        [NonSerialized]public bool IsFirstPlayer;
        public string Name; //Player public name.
        public ClientColor ClientColor; //Player color.
        public List<string> Cards;
        public ushort Reputation {get; set;}
        [NonSerialized]public short ReputationDelta;
        public float Money { get; set; }
        [NonSerialized]public float MoneyDelta;
        public PerceptionEnum perceptionDiseases;

        //TODO: ADD ALL DATA RELATED TO CLIENTS. SCORE? CARDS WATEVER?



        public Client(int connectionID)
        {
            this.ConnectionID = (byte)connectionID;
            this.IsConnected = true;
            this.Name = "";
            this.ClientColor = new ClientColor(Color.white);
            this.perceptionDiseases = PerceptionEnum.none;
            Reset();
        }

        public void Clone(Client other){
            this.CurrentState = other.CurrentState;
            this.Name = other.Name;
            this.ClientColor = other.ClientColor;
            this.Reputation = other.Reputation;
            this.perceptionDiseases = PerceptionEnum.none;
            this.Cards = other.Cards;
            this.Money = other.Money;
            this.Reputation = other.Reputation;
        }
        public void Reset(){
            CurrentState = GameState.MenuState;
            IsReady = false;
            IsFirstPlayer = false;
            Reputation = 0;
            ReputationDelta = 0;
            Money = 0.0f;
            MoneyDelta = 0.0f;
            Cards = new List<string>();
        }
    }
}

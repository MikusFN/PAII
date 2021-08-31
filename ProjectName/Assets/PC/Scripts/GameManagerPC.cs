using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Generic;
using Assets.Network;
using Assets.PC.PCG;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PC
{
    /// <summary>
    /// This class is the main manager of the PC.
    /// 
    /// Here is where all logic is processed including dealing with clients, and game.
    /// </summary>
    public sealed class GameManagerPC : SingletonBehaviour<GameManagerPC>
    {
        //---------------------------- CONTS
        public int DRAW_CARD_COST = 100;
        public uint MIN_PLAYERS = 1;
        public byte START_CARDS = 5;
        public float START_MONEY = 100;
        public ushort START_REPUTATION = 0;
        public float START_MONEY_DELTA = 10;
        public short START_REPUTATION_DELTA = 1;


        private Dictionary<string, Client> IPtoClientMap;

        public Queue<KeyValuePair<string, int>> reasearchsOnGoing = new Queue<KeyValuePair<string, int>>();
        int timer = 0;
        public int reasearchTime = 10;

        #region CLIENT DICTIONARY GET SETTERS
        /// <summary>
        /// Returns true if the IP is already connected.
        public bool DoesIPExist(string ip)
        {
            return IPtoClientMap.ContainsKey(ip);
        }
        public Client FindClientByIP(string ip)
        {
            if (DoesIPExist(ip))
                return IPtoClientMap[ip];
            else
                return null;
        }
        /// <summary>
        /// Returns the Client with the provided connectionID.
        /// </summary>
        public Client FindClientByID(int id)
        {
            foreach (Client c in IPtoClientMap.Values)
            {
                if (c.ConnectionID == id)
                    return c;
            }
            return null;
        }
        /// <summary>
        /// Returns the IP of the Client.
        /// </summary>
        public string FindIPByClient(Client c)
        {
            foreach (KeyValuePair<string, Client> kvp in IPtoClientMap)
            {
                if (kvp.Value.ConnectionID == c.ConnectionID)
                    return kvp.Key;
            }
            return null;
        }
        /// <summary>
        /// Retyrbs the current IP of an ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FindIPByID(int id)
        {
            return FindIPByClient(FindClientByID(id));
        }
        /// <summary>
        /// Changes the Unity ConnectionID to always match the IP of the user.
        /// </summary>
        public Client SetClientID(string ip, int id)
        {
            if (IPtoClientMap.ContainsKey(ip))
            {
                IPtoClientMap[ip].ConnectionID = (byte)id;
                IPtoClientMap[ip].IsConnected = true;
                DebugApplication.DebugConsole.Instance.Log(string.Format("Reconnected: {0} with ID: {1}", ip, id));

                Network.MessageBase connectMsg = new Network.MessageBase(IPtoClientMap[ip], MessageType.ClientSendData);
                NetworkPC.Instance.SendMessage(connectMsg, id, NetworkPC.reliableChannelID);
            }
            else
            {
                IPtoClientMap.Add(ip, new Client(id));
                DebugApplication.DebugConsole.Instance.Log(string.Format("Connected: {0} with ID: {1}", ip, id));
            }
            return IPtoClientMap[ip];
        }
        /// <summary>
        /// Clears the clients values.false Does not remove it from the dictionay. (ITS NOT SUPPOSED TO!!!)
        /// </summary>
        public Client RemoveClientID(int id)
        {
            Client c = FindClientByID(id);
            if (c != null)
            {
                c.ConnectionID = 0;
                c.IsReady = false;
                c.IsConnected = false;
            }
            DebugApplication.DebugConsole.Instance.Log(string.Format("Disconnected: {0}", FindIPByClient(c)));
            return c;
        }
        /// <summary>
        /// Returns a list with all connected clients as ShareableClients
        /// </summary>
        public List<ShareableClient> GetAllShareableClients(Client except = null)
        {
            List<ShareableClient> result = new List<ShareableClient>();
            foreach (var pair in IPtoClientMap)
            {
                if (pair.Value.IsConnected && (except != null ? (pair.Value.ConnectionID != except.ConnectionID) : true))
                    result.Add(new ShareableClient(pair.Value, pair.Key));
            }
            return result;
        }
        /// <summary>
        /// Returns a list of all connection ID present.
        /// </summary>
        public List<int> GetAllConnections(Client except = null, Client except2 = null)
        {
            List<int> result = new List<int>();

            foreach (var pair in IPtoClientMap)
            {
                if (pair.Value.IsConnected && (except != null ? (pair.Value.ConnectionID != except.ConnectionID) : true) && (except2 != null ? (pair.Value.ConnectionID != except2.ConnectionID) : true))
                    result.Add(pair.Value.ConnectionID);
            }
            return result;
        }
        #endregion

        //---------------------------- Match
        //private ProceduralCity currentCity;
        public ProceduralCity cityPrefab;
        public GameObject cityParent;
        public bool gameStarted;
        public ushort MatchSeconds;
        public Text gameTime;
        private float currentSecond;
        private Action updateActions;


        //---------------------------- Diseases
        private List<DiseaseData> allDiseasesData;
        public List<Card> GetAvailableDiseases()
        {
            List<Card> result = new List<Card>();
            foreach (var dd in allDiseasesData)
            {
                result.Add(dd.card);
            }

            return AssetManager.Instance.GetAvaiableDiseasesFromList(result);
        }
        public List<ShareableDisease> GetAllShareabledDiseases()
        {
            var result = new List<ShareableDisease>();
            foreach (var dd in allDiseasesData)
            {
                result.Add(new ShareableDisease(dd.card, dd.color));
            }
            return result;
        }
        public void AddMentalDisease(string clientIP, Card card)
        {
            Client c = IPtoClientMap[clientIP];
            if (c.perceptionDiseases == PerceptionEnum.none)
            {
                c.perceptionDiseases = card.mentalDiseaseType;
                NetworkPC.Instance.SendMessage(new MessageBase(card.mentalDiseaseType, MessageType.SetMentalDisease), c.ConnectionID, NetworkPC.reliableChannelID);
            }
        }
        //---------------------------- Card History
        private List<CardPlayerWrapper> allCardsPlayed;

        //---------------------------- vote
        public bool isGuity;
        public Dictionary<int, bool> playersDecsion = new Dictionary<int, bool>();
        public Client clientAcuserT;
        public Client clientSuspectT;

        protected override void Awake()
        {
            base.Awake();
            IPtoClientMap = new Dictionary<string, Client>();
            gameStarted = false;
        }
        private void Start()
        {
            UIManagerPC.Instance.EnqueueNews("BREAKING NEWS : PERSON STARTS GAME SUCESSFULLY! WELCOME!");
            UIManagerPC.Instance.EnqueueNewsLoop("WAITING FOR PLAYERS");
        }
        private void Update()
        {
            if (updateActions != null)
            {
                updateActions();
            }

        }
        private void UpdateEverySecond()
        {
            removeFromQueueAndGiveCard();
            foreach (Client c in IPtoClientMap.Values)
            {
                IncrementPlayerMoney(c, c.MoneyDelta);
                IncrementPlayerReputation(c, c.ReputationDelta);
            }
        }
        //---------------------------- Match Methods
        public void OnGameStart()
        {
            if (IPtoClientMap.Count >= MIN_PLAYERS)
            {
                foreach (Client c in IPtoClientMap.Values)
                {
                    if (c.IsReady == false)
                        return;
                }

                //GAMES STARTS!
                gameStarted = true;
                allDiseasesData = new List<DiseaseData>();
                currentSecond = 0;
                foreach (Client c in IPtoClientMap.Values)
                {
                    c.Money = START_MONEY;
                    c.MoneyDelta = START_MONEY_DELTA;
                    c.Reputation = START_REPUTATION;
                    c.ReputationDelta = START_REPUTATION_DELTA;
                    UIManagerPC.Instance.UpdatePlayerResourcesUI(c);
                    c.CurrentState = GameState.InGameState;
                    NetworkPC.Instance.SendMessage(new Network.MessageBase(c, MessageType.GameStarting), c.ConnectionID, NetworkPC.reliableChannelID);
                }

                NetworkPC.Instance.SetGameState(GameState.InGameState);
                DebugApplication.DebugConsole.Instance.Log("Starting Game.");

                updateActions += MatchUpdate;
                InvokeRepeating("CycleDiseases", 0, 3);

                allCardsPlayed = new List<CardPlayerWrapper>();

                ProceduralCity.Instance?.DestroyInstances();
                Instantiate(cityPrefab, Vector3.zero, Quaternion.identity, cityParent.transform);

                UIManagerPC.Instance.StopNewsLoop();
                UIManagerPC.Instance.EnqueueNews("GAME START! GOOD LUCK!");
                InvokeRepeating("UpdateEverySecond", 0, 1);

            }
        }
        private void MatchUpdate()
        {
            currentSecond += Time.deltaTime;
            float timeLeft = MatchSeconds - currentSecond;
            gameTime.text = String.Format("{0}:{1}", (uint)(timeLeft / 60.0f), (timeLeft % 60.0f).ToString("00"));

            if (currentSecond >= MatchSeconds)
            {
                GameTimeUp();
                updateActions -= MatchUpdate;
            }
        }
        private void GameTimeUp()
        {
            DebugApplication.DebugConsole.Instance.Log("Game Time ended");
            ushort MaxReputation = IPtoClientMap.Values.Max(cli => cli.Reputation);

            foreach (Client c in IPtoClientMap.Values)
            {
                bool hasWon = (c.Reputation == MaxReputation) ? true : false;
                NetworkPC.Instance.SendMessage(new MessageBase(hasWon, MessageType.TimesUp), c.ConnectionID, NetworkPC.reliableChannelID);
                if (hasWon)
                {
                    DebugApplication.DebugConsole.Instance.Log($"Player {c.ConnectionID} won");
                }
            }
            gameStarted = false;
            updateActions -= MatchUpdate;

            CancelInvoke();
        }

        //PC Methods
        public void OnClientUIUpdate(Client c)
        {
            string ip = FindIPByClient(c);
            Client currentClient = IPtoClientMap[ip];
            currentClient.Name = c.Name;
            currentClient.ClientColor = c.ClientColor;

            UIManagerPC.Instance.UpdatePlayerUI(currentClient);
        }
        public void OnClientSetReady(int connectionID)
        {
            /*Client c = FindClientByID(connectionID);
            if (!c.IsReady)
            {
                c.IsReady = true;
                DebugApplication.DebugConsole.Instance.Log(string.Format("{0} is ready", connectionID));
            }*/
        }
        public void ResetGame()
        {
            DebugApplication.DebugConsole.Instance.Log("Reseting Game.");
            NetworkPC.Instance.SetGameState(GameState.MenuState);
            foreach (Client c in IPtoClientMap.Values)
            {
                c.Reset();
                NetworkPC.Instance.SendMessage(new Network.MessageBase(null, MessageType.GameEnd), c.ConnectionID, NetworkPC.reliableChannelID);
            }

            ProceduralCity.Instance?.DestroyInstances();

            updateActions -= MatchUpdate;
            gameStarted = false;

            UIManagerPC.Instance.ResetUI();
            gameTime.text = "0:00";
            CancelInvoke();
        }
        public void DisconnectAll()
        {
            foreach (Client c in IPtoClientMap.Values)
            {
                NetworkPC.Instance.DisconnectClient(c);
            }
            UIManagerPC.Instance.ClearUI();
            IPtoClientMap = new Dictionary<string, Client>();
        }

        //---------------------------- CARDS
        public void AddCardToPlayer(int connectionID, string card)
        {
            Client c = FindClientByID(connectionID);
            if (c != null)
            {
                c.Cards.Add(card);
            }
        }
        public void RemoveCardFromPlayer(int connectionID, string card)
        {
            Client c = FindClientByID(connectionID);
            if (c != null)
            {
                c.Cards.Remove(card);
                allCardsPlayed.Add(new CardPlayerWrapper(card, FindIPByClient(c)));
            }
        }

        void removeFromQueueAndGiveCard()
        {

            if (reasearchsOnGoing.Count == 0)
            {
                timer = 0;
            }
            else timer += 1;

            if (timer >= reasearchTime)
            {
                var cardConect = reasearchsOnGoing.Dequeue();
                NetworkPC.Instance.SendMessage(new MessageBase(cardConect.Key, MessageType.DrawSpecificCard), cardConect.Value, NetworkPC.reliableChannelID);
                timer = 0;
            }
        }

        //---------------------------- CURRENCIES
        public void IncrementPlayerMoney(int connectionID, float money)
        {
            Client c = FindClientByID(connectionID);
            IncrementPlayerMoney(c, money);
        }
        public void IncrementPlayerMoney(Client c, float money)
        {
            if (float.MaxValue - money > c.Money)
            {
                c.Money += money;
                NetworkPC.Instance.SendMessage(new Network.MessageBase(c.Money, MessageType.SetMoney), c.ConnectionID, NetworkPC.reliableChannelID);
                UIManagerPC.Instance.UpdatePlayerUI(c);
            }
        }
        public void IncrementPlayerReputation(int connectionID, short reputation)
        {
            Client c = FindClientByID(connectionID);
            IncrementPlayerReputation(c, reputation);
        }
        public void IncrementPlayerReputation(Client c, short reputation)
        {
            if ((short)c.Reputation + reputation < 0)
                c.Reputation = 0;
            else
                c.Reputation = (ushort)(c.Reputation + reputation);
            NetworkPC.Instance.SendMessage(new Network.MessageBase(c.Reputation, MessageType.SetReputation), c.ConnectionID, NetworkPC.reliableChannelID);
            UIManagerPC.Instance.UpdatePlayerUI(c);
        }

        //---------------------------- Diseases
        public void AddDisease(Card disease, int playerID, int infectedCount, List<Transform> infectedPeople, Vector3 infectPos)
        {
            foreach (var dd in allDiseasesData)
            {
                if (dd.card.id == disease.id)
                {
                    dd.infectedCount += infectedCount;
                    dd.infectedPeople.AddRange(infectedPeople);
                    UIManagerPC.Instance.UpdateDisease(dd);
                    SelectParticlesManager.Instance.Shoot(infectPos, dd.color);
                    return;
                }
            }

            Color color = AssetManager.Instance.allPlayerColors[UnityEngine.Random.Range(0, AssetManager.Instance.allPlayerColors.Count)];
            SelectParticlesManager.Instance.Shoot(infectPos, color);

            if (infectedPeople.Count > 0)
            {
                string playerIP = playerID >= 0 ? FindIPByID(playerID) : "";
                DiseaseData newData = new DiseaseData(disease, playerIP, infectedCount, infectedPeople, color);
                allDiseasesData.Add(newData);
                UIManagerPC.Instance.CreateDisease(newData);
            }

            var connections = GetAllConnections();
            foreach (var con in connections)
            {
                NetworkPC.Instance.SendMessage(new MessageBase(new List<ShareableDisease>() { new ShareableDisease(disease, color) }, MessageType.SetDiseases), con, NetworkPC.reliableChannelID);
            }

        }
        public void AddTransformToDisease(Transform t, Card disease)
        {
            foreach (var dd in allDiseasesData)
            {
                if (dd.card.id == disease.id)
                {
                    dd.infectedCount += 1;
                    dd.infectedPeople.Add(t);
                    UIManagerPC.Instance.UpdateDisease(dd);
                }
            }
        }
        public void RemoveTransfromFromDisease(Transform t, Card disease){
            DiseaseData deleteDisease = null;
            foreach(var dd in allDiseasesData){
                if(dd.card.id == disease.id){
                    Transform deletePerson = null;
                    foreach(var person in dd.infectedPeople){
                        if(person == t){
                            deletePerson = person;
                        }
                    }
                    if(deletePerson != null){
                        dd.infectedPeople.Remove(deletePerson);
                        dd.infectedCount--;
                        
                        UIManagerPC.Instance.UpdateDisease(dd);
                        if(dd.infectedCount == 0){
                            deleteDisease = dd;
                        }
                    }
                    break;
                }
            }

            if(deleteDisease != null){
                allDiseasesData.Remove(deleteDisease);
                UIManagerPC.Instance.RemoveDisease(deleteDisease);
            }
        }

        public void cleanPlayersDecision()
        {
            playersDecsion.Clear();
        }

        public void setGuilt(string GultyPlayerIP, string cardID)
        {

            foreach (DiseaseData d in allDiseasesData )
            {
                if(d.card.id==cardID && GultyPlayerIP == d.playerIP)
                {
                    isGuity = false;
                    return;
                }
            }
            isGuity = true;
         }


            private void CycleDiseases()
        {
            UIManagerPC.Instance.CycleDiseases();
        }

        //private check time to finish reaserach

    }
}

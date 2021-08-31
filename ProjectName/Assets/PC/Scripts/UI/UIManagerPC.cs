using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Generic;
using Assets.Network;
using UnityEngine.UI;
using System;
using System.Linq;
using Assets.PC.Npcs;

namespace Assets.PC
{
    public class UIManagerPC : SingletonBehaviour<UIManagerPC>
    {
        //CARD BOTTOM AREA. USED ONLY FOR ANIMATING PLAYED CARDS
        [Space(10)]public RectTransform cardArea;
        public PlayerCardAreaBehaviour playerCardTemplate;
        private List<PlayerCardAreaBehaviour> playerCardList;
        public CardBehaviour cardTemplate;


        //PLAYER TOP AREA. HERE IS WHERE THE NAME AND COLOR APPEARS.
        [Space(10)]public RectTransform playerArea;
        public PlayerNameBehaviour playerNameTemplate;
        private Dictionary<Client, PlayerNameBehaviour> playerAreaDictionary;


        //NEWS FEED AREA:
        [Space(10)]public NewsBehaviour newsBehaviour;


        //DISEASES AREA:
        [Space(10)]public RectTransform diseasesArea;
        public DiseaseBehaviour diseaseTemplate;
        private Queue<DiseaseBehaviour> currentDiseases;

        //STATS AREA:
        [Space(10)] public Text totalNpcs;
        public Text infectedNpcs;
        public Text deadNpcs;
        public Text mandNpcs;
        public Text femaleNpcs;
        public Text ambiguousNpcs;
        public Text hornyNpcs;
        public Text infectableNpcs;

        protected override void Awake()
        {
            base.Awake();
            playerCardList = new List<PlayerCardAreaBehaviour>();
            playerAreaDictionary = new Dictionary<Client, PlayerNameBehaviour>();
            currentDiseases = new Queue<DiseaseBehaviour>();
        }

        public void ResetUI(){
            foreach(var db in currentDiseases){
                Destroy(db.gameObject);
            }
            BlobManager.Instance.SetObjects(null);
            currentDiseases.Clear();
            ClearAllNews();
        }
        public void ClearUI()
        {
            foreach (var pct in playerCardList)
            {
                Destroy(pct.gameObject);
            }
            playerCardList.Clear();

            foreach (var pnt in playerAreaDictionary)
            {
                Destroy(pnt.Value.gameObject);
            }
            playerAreaDictionary.Clear();

            ResetUI();
        }

        //--------------------------- PLAYER UI
        public void AddPlayerArea(Client c)
        {
            if (!playerAreaDictionary.ContainsKey(c))
            {
                PlayerCardAreaBehaviour pct = Instantiate(playerCardTemplate, cardArea);
                PlayerNameBehaviour pnt = Instantiate(playerNameTemplate, playerArea);

                if(GameManagerPC.Instance.gameStarted){
                    pnt.SetNameAndColor(c);
                }

                playerCardList.Add(pct);
                playerAreaDictionary.Add(c, pnt);
                return;
            }
            DebugApplication.DebugConsole.Instance.Log("Tryed to add UI player when it already exists");
        }
        public void UpdatePlayerUI(Client client)
        {
            if (playerAreaDictionary.ContainsKey(client))
            {
                playerAreaDictionary[client].SetNameAndColor(client);
            }
        }
        public void RemovePlayerUI(Client client)
        {
            if (playerAreaDictionary.ContainsKey(client))
            {

                var pcb = FindPlayerCard(client.ConnectionID);
                if (pcb != null)
                    pcb.RemoveCurrentCard();

                Destroy(playerCardList[0].gameObject);
                playerCardList.RemoveAt(0);

                Destroy(playerAreaDictionary[client].gameObject);
                playerAreaDictionary.Remove(client);
            }
        }
        public void UpdatePlayerResourcesUI(Client client){
            if(playerAreaDictionary.ContainsKey(client)){
                playerAreaDictionary[client].SetReputation(client.Reputation);
                playerAreaDictionary[client].SetMoney(client.Money);
            }
        }

        //--------------------------- CARDS UI
        public void PlayerHoldsCard(int ConnectionID)
        {
            PlayerCardAreaBehaviour pcb = GetAvailablePlayerCard();
            pcb?.SetCurrentCard(cardTemplate, ConnectionID);
        }
        public void PlayerDropsCard(int ConnectionID)
        {
            PlayerCardAreaBehaviour pcb = FindPlayerCard(ConnectionID);
            pcb?.DropCard();
        }
        public void PlayerPlaysCard(int ConnectionID, Card card)
        {
            PlayerCardAreaBehaviour pcb = FindPlayerCard(ConnectionID);
            pcb?.PlayCard(card);
        }

        private PlayerCardAreaBehaviour FindPlayerCard(int ConnectionID)
        {
            foreach (var pc in playerCardList)
            {
                if (pc.connectionID == ConnectionID)
                {
                    return pc;
                }
            }
            return null;
        }
        private PlayerCardAreaBehaviour GetAvailablePlayerCard()
        {
            List<PlayerCardAreaBehaviour> availableCards = new List<PlayerCardAreaBehaviour>();
            foreach (var pcb in playerCardList)
            {
                if (pcb.connectionID == -1)
                {
                    availableCards.Add(pcb);
                }
            }

            PlayerCardAreaBehaviour result = null;
            if (availableCards.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, availableCards.Count);
                result = availableCards[index];
            }

            return result;
        }

        //--------------------------- NEWS UI -----CHECK NEWS BEHAVIOUR
        public void EnqueueNews(string news){
            newsBehaviour.Enqueue(news);
        }
        public void EnqueueNewsLoop(string newsLoop){
            newsBehaviour.EnqueueLoop(newsLoop);
        }
        public void ClearAllNews(){
            newsBehaviour.ClearAll();
        }
        public void StopNewsLoop(){
            newsBehaviour.StopLoop();
        }


        //--------------------------- DISEASES UI
        public void CreateDisease(DiseaseData newData)
        {
            var obj = Instantiate(diseaseTemplate);
            obj.transform.SetParent(diseasesArea);
            obj.transform.localScale = Vector3.one;
            currentDiseases.Enqueue(obj);
            obj.Init(newData);
            if(currentDiseases.Count > 1)
                currentDiseases.Last().SetSelected(false);
            obj.SetSelected(true);

            BlobManager.Instance.SetBoarderColor(newData.color);
            BlobManager.Instance.SetObjects(newData.infectedPeople);
        }
        public void UpdateDisease(DiseaseData dd)
        {
            foreach(DiseaseBehaviour db in currentDiseases){
                if(db.data.card.id == dd.card.id){
                    db.UpdateValues(dd);
                    return;
                }
            }
        }
        
        public void RemoveDisease(DiseaseData deleteDisease)
        {
            foreach(DiseaseBehaviour db in currentDiseases){
                if(db.data.card.id == deleteDisease.card.id){
                    Destroy(db.gameObject);
                    return;
                }
            }
        }
        public void CycleDiseases(){
            if(currentDiseases.Count > 0){
                currentDiseases.Last().SetSelected(false);
                
                var db = currentDiseases.Dequeue();
                db.SetSelected(true);
                BlobManager.Instance.SetObjects(db.data.infectedPeople);
                BlobManager.Instance.SetBoarderColor(db.data.color);
                currentDiseases.Enqueue(db);
            }
        }
        
        //--------------------------- CITY STATS
        public void UpdateCityStats(){
            totalNpcs.text =        $"of {CityStats.Instance.PopulationAliveCount}";
            infectedNpcs.text =     $"{CityStats.Instance.PopulationAliveCount * CityStats.Instance.Infected100} sick";
            deadNpcs.text =         $"Dead : {CityStats.Instance.PopulationCount * CityStats.Instance.Dead100 }";
            mandNpcs.text =         $"Male : {CityStats.Instance.PopulationAliveCount * CityStats.Instance.Male100}";
            femaleNpcs.text =       $"Female : {CityStats.Instance.PopulationAliveCount * CityStats.Instance.Female100}";
            ambiguousNpcs.text =    $"Amiguous : {CityStats.Instance.PopulationAliveCount * CityStats.Instance.Ambigous100}";
            hornyNpcs.text =        $"Horny : {CityStats.Instance.PopulationAliveCount * CityStats.Instance.Horny100}";
            infectableNpcs.text =   $"Infectable : {CityStats.Instance.PopulationAliveCount * CityStats.Instance.Infectable100}";
        }
    }
}

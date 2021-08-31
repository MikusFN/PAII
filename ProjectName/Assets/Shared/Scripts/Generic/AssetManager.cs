using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Generic
{
    /// <summary>
    /// Class that will load and store all asset data of the game. Use GetObject() to return a NetworkObject from the database.
    /// Please place all your cards in the /Resources/Cards folder, so that the manager can read them.
    /// </summary>
    public class AssetManager : SingletonBehaviour<AssetManager>
    {
        public Texture2D cardTexture;
        private Dictionary<string, NetworkObject> allObjects;
        private List<string> allCards;
        public List<Color> allPlayerColors;
        
        protected override void Awake()
        {
            base.Awake();
            allObjects = new Dictionary<string, NetworkObject>();
            allCards = new List<string>();
            var cards = Resources.LoadAll<NetworkObject>("Cards");

            foreach (NetworkObject o in cards)
            {
                allObjects.Add(o.id, o);
                if(o is Card)
                    allCards.Add(o.id);
                DebugApplication.DebugConsole.Instance.Log("Loaded: " + o.id);
            }
        }

        /// <summary>
        /// Returns the NetworkObject matching a given ID.
        /// </summary>
        /// <param name="id">The ID to search for.</param>
        public NetworkObject GetObject(string id)
        {
            if (allObjects.ContainsKey(id))
            {
                return allObjects[id];
            }
            DebugApplication.DebugConsole.Instance.Log("ASSET ID ERROR! " + id);
            return null;
        }

        /// <summary>
        /// Returns a random card from all cards available.
        /// </summary>
        /// <returns></returns>
        public Card GetRandomCard(){
            int random = UnityEngine.Random.Range(0, allCards.Count - 1);
            return allObjects[allCards[random]] as Card;
        }

        /// <summary>
        /// Returns all cards in the game.
        /// </summary>
        /// <returns></returns>
        public List<Card> GetAllCards(){
            List<Card> result = new List<Card>();
            foreach(var c in allCards){
                result.Add(GetObject(c) as Card);
            }
            return result;
        }

        public List<Card> GetAvaiableDiseasesFromList(List<Card> usedDiseases)
        {
            List<Card> result = new List<Card>();
            foreach(var obj in allObjects.Values){
                if(obj is Card){
                    var card = obj as Card;
                    if(card.cardType == CardType.disease && !usedDiseases.Contains(card)){
                        result.Add(card);
                    }   
                }
            }

            return result;
        }
    }
}

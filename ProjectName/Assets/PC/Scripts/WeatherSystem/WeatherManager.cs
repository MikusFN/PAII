using Assets.Generic;
using Assets.PC.Npcs;
using Assets.PC.WeatherSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.PC.WeatherSystem
{
    public class WeatherManager : MonoBehaviour
    {
        public static WeatherManager weatherManager;
        public WeatherSeasons season;
        private WeatherSeasons previousSeason;
        private float currentTime, maxTime;
        //public RectTransform image, canvas, textRect;
        private Text text;
        private List<Card> diseases;

        public List<Card> Diseases
        { get => diseases; set => diseases = value; }

        private void Start()
        {
            // Singleton
            if (weatherManager == null)
            {
                weatherManager = this;
            }
            else if (weatherManager != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            //maxTime = GameManagerPC.Instance.MatchSeconds;
            currentTime = Time.realtimeSinceStartup;
            previousSeason = WeatherSeasons.Winter;
            diseases = new List<Card>();
            //diseases.Add(new Card());
            //diseases = AssetManager.Instance.GetAvaiableDiseasesFromList(diseases);
            //image.position = new Vector2(image.rect.width / 2, canvas.rect.height - image.rect.height / 2);
            ///textRect.position
            //    = new Vector3(image.position.x + image.rect.width / 2, image.position.y);
            //text = image.GetComponentInChildren<Text>();
            //text.text = season.ToString();
            
        }
        private void Update()
        {
            currentTime = Time.realtimeSinceStartup;
            season = OnSeason(currentTime);
            //text.text = season.ToString()+$"-> {CityStats.Instance.Infected100}";
            //if (previousSeason != season)
            //{
            //    OpenSeasonSpread();
            //}
            previousSeason = season;
        }

        WeatherSeasons OnSeason(float time)
        {
            float percentTime = (time * 4) / maxTime;
            WeatherSeasons calSeason = (WeatherSeasons)(percentTime % 4);

            //switch (calSeason)
            //{
            //    case WeatherSeasons.Spring:
            //        diseases.RemoveAll(x => x.Length > 0);
            //        diseases.Add("Spring1");
            //        diseases.Add("Spring2");
            //        diseases.Add("Spring3");
            //        diseases.Add("Spring4");
            //        break;
            //    case WeatherSeasons.Summer:
            //        diseases.RemoveAll(x => x.Length > 0);
            //        diseases.Add("Summer1");
            //        diseases.Add("Summer2");
            //        diseases.Add("Summer3");
            //        diseases.Add("Summer4");
            //        break;
            //    case WeatherSeasons.Fall:
            //        diseases.RemoveAll(x => x.Length > 0);
            //        diseases.Add("Fall1");
            //        diseases.Add("Fall2");
            //        diseases.Add("Fall3");
            //        diseases.Add("Fall4");
            //        break;
            //    case WeatherSeasons.Winter:
            //        diseases.RemoveAll(x => x.Length > 0);
            //        diseases.Add("Winter1");
            //        diseases.Add("Winter2");
            //        diseases.Add("Winter3");
            //        diseases.Add("Winter4");
            //        break;
            //}
            return calSeason;
        }
        void OpenSeasonSpread()
        {
            if (diseases.Count > 0)
            {
                Card d = Diseases[Random.Range(0, Diseases.Count)];
                int maxNpcContamination = 0;
                if((NpcStatsManager.componentListing.PopulationStats.Length / 10)>0)
                {
                    maxNpcContamination =( NpcStatsManager.componentListing.PopulationStats.Length / 10);
                }
                else 
                {
                    maxNpcContamination = 10;
                }
                int infectedCount = Random.Range(1, maxNpcContamination), trueCount = 0;
                Transform dt = null;
                List<Transform> infectedTrans = new List<Transform>();
                for (int i = 0; i < infectedCount; i++)
                {
                    dt = DiseaseSpread(NpcStatsManager.componentListing.GetWeakling(), d);
                    if (dt != null)
                    {
                        infectedTrans.Add(dt);
                        trueCount++;
                    }
                }
                if (infectedTrans.Count > 0)
                {
                    CityStats.Instance.Update100();
                    GameManagerPC.Instance.AddDisease(d, -1, trueCount, infectedTrans, infectedTrans[0].transform.position);
                }
            }
        }
        Transform DiseaseSpread(int randomIndex, Card disease)
        {
            Transform npcT = null;
            if (randomIndex >= 0)
            {
                NpcStats npc = NpcStatsManager.componentListing.PopulationStats[randomIndex];
                if (npc.Diseases.Count <= 5 && npc.Diseases.Find(x => x.Item2 == disease) == null)
                {
                    Tuple<WeatherSeasons, Card, int> dis = new Tuple<WeatherSeasons, Card, int>(season, disease, -1);
                    npc.Diseases.Add(dis);
                    npc.IsHazardous = true;
                    npcT = npc.GetComponentInParent<Transform>();
                }
            }
            return npcT;
        }
    }
}

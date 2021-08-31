using Assets.Generic;
using Assets.PC.Npcs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PC.Npcs
{

    public class CityStats : SingletonBehaviour<CityStats>
    {
        private float horny100, infectable100, infected100, dead100, time;
        private NpcStats[] population;
        private Tuple<float, float, float> gender100;
        private int populationCountStart;

        public Tuple<float, float, float> Gender100 { get => gender100; }
        public float Infected100 { get => infected100; }
        public int PopulationCount { get => population.Length; }
        public int PopulationAliveCount { get => (int)(population.Length * (1 - Dead100)); }
        public float Dead100 { get => dead100; }
        public float Male100 { get => gender100.Item1; }
        public float Female100 { get => gender100.Item2; }
        public float Ambigous100 { get => gender100.Item3; }
        public float Horny100 { get => horny100; }
        public float Infectable100 { get => infectable100; }

        void Start()
        {
            time = 1;

            population = new NpcStats[NpcStatsManager.componentListing.PopulationStats.Length];
            for (int i = 0; i < NpcStatsManager.componentListing.PopulationStats.Length; i++)
            {
                population[i] = NpcStatsManager.componentListing.PopulationStats[i];
            }
            populationCountStart = PopulationAliveCount;
            if (population.Length > 0)
            {
                Gender100Update();
                Update100();
            }
        }

        public void Gender100Update()
        {

            int i = 0, j = 0, k = 0;

            foreach (var item in population)
            {
                if (item.Life > 0)
                    switch (item.Gender)
                    {
                        case GenderNpc.Male:
                            i++;
                            break;
                        case GenderNpc.Female:
                            j++;
                            break;
                        case GenderNpc.Ambiguous:
                            k++;
                            break;
                    }
            }
            gender100 = new Tuple<float, float, float>((float)i / PopulationAliveCount, (float)j / PopulationAliveCount, (float)k / PopulationAliveCount);
            //Debug.Log("\n Gender -> " + gender100.Item1 + " " + gender100.Item2 + " " + gender100.Item3);
        }
        public void Update100()
        {
            TimeCounter(ref time);
            if (time >= 1)
            {
                int i = 0, j = 0, l = 0, k = 0;
                foreach (var item in population)
                {
                    if (item.life > 0)
                    {
                        if (item.IsHazardous)
                            j++;
                        if (item.state == NpcState.Horny)
                            l++;
                        if (item.IsInfectable)
                            k++;
                    }
                    else
                    {
                        i++;
                    }
                }
                dead100 = (float)i / PopulationCount;
                infected100 = (float)j / PopulationAliveCount;
                horny100 = (float)l / PopulationAliveCount;
                infectable100 = (float)k / PopulationAliveCount;
                //Debug.Log("mortos-> " + Dead100 + " infetados-> " + Infectable100 + " horny-> " + Horny100 + " infectaveis-> " + Infectable100);

                //UIManagerPC.Instance.UpdateCityStats();
            }
        }
        private void TimeCounter(ref float time)
        {
            if (time > 1.0f)
            {
                time = 0;
            }
            else
            {
                time += Time.deltaTime;
            }
        }
        //public void Infected100Update()
        //{
        //    //int i = 0;
        //    //foreach (var item in population)
        //    //{
        //    //    if (item.IsHazardous && item.life > 0)
        //    //        i++;
        //    //}
        //    //infected100 = (float)i / PopulationCount;

        //    //Debug.Log("\n Infected -> " + infected100);
        //}
        //public void Dead100Update()
        //{
        //    //int i = 0;
        //    //foreach (var item in population)
        //    //{
        //    //    if (item.life <= 0)
        //    //        i++;
        //    //}
        //    //dead100 = (float)i / PopulationCount;
        //    //Debug.Log("\n Dead -> " + dead100);
        //}
    }
}

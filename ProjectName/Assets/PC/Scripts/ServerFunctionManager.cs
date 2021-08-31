using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using Assets.Network;
using Assets.PC.Npcs;
using UnityEngine;

namespace Assets.PC
{
    public class ServerFunctionManager : MonoBehaviour
    {
        public static Vector3 touchPos;

        public void Template(int connectionID, Card card)
        {
            DebugApplication.DebugConsole.Instance.Log("Ran template!");
        }

        public void IrsRefund(int connectionID, Card card)
        {
            GameManagerPC.Instance.IncrementPlayerMoney(connectionID, 300.0f);
        }

        public void InfectPeople(int connectionID, Card card)
        {
            var affectedNpcs = NpcStatsManager.componentListing.GetNeighbours(touchPos, card.initialContagiousRadius);
            List<Transform> allInfectedTransforms = new List<Transform>();

            if (card.AdvancedVaccination)
            {
                foreach (var npc in affectedNpcs)
                {
                    if (!npc.HasCard(card) && !npc.isVacinatedAdvanced && Random.Range(0, 100) < card.initialContagiousRadius)
                    {
                        allInfectedTransforms.Add(npc.transform);
                        npc.AddCard(card, connectionID);
                    }
                }
            }
            else if (card.BasicVaccination){
                 foreach (var npc in affectedNpcs)
                {
                    if (!npc.HasCard(card) && !npc.isVacinated && Random.Range(0, 100) < card.initialContagiousRadius)
                    {
                        allInfectedTransforms.Add(npc.transform);
                        npc.AddCard(card, connectionID);
                    }
                }
            }
            else{
                 foreach (var npc in affectedNpcs)
                {
                    if (!npc.HasCard(card) && Random.Range(0, 100) < card.initialContagiousRadius)
                    {
                        allInfectedTransforms.Add(npc.transform);
                        npc.AddCard(card, connectionID);
                    }
                }
            }

            GameManagerPC.Instance.AddDisease(card, connectionID, allInfectedTransforms.Count, allInfectedTransforms, touchPos);
        }

        public void UseAntibiotic(int connectionID, Card card)
        {
            NpcStatsManager.componentListing.GetCurable(touchPos, card.effectRange, card);
        }

        public void UseVaccine(int connectionID, Card card)
        {
            NpcStatsManager.componentListing.VacinateNpcs(touchPos, card.effectRange, card, false);
        }
        public void UseVaccineAdvanced(int connectionID, Card card)
        {
            NpcStatsManager.componentListing.VacinateNpcs(touchPos, card.effectRange, card, true);
        }
    }
}

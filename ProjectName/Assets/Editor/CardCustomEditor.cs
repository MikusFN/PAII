using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Assets.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(Card))]
    public class CardCustomEditor : UnityEditor.Editor
    {
        SerializedProperty id;
        SerializedProperty name;
        SerializedProperty description;
        SerializedProperty textForTheNews;
        SerializedProperty texture;
        SerializedProperty cost;
        SerializedProperty reputationImpact;
        SerializedProperty moneyDelta;
        SerializedProperty reputationDelta;

        SerializedProperty serverEvents;
        SerializedProperty clientEvents;
        SerializedProperty bothEvents;
        SerializedProperty npcEvents;

        SerializedProperty cardType;
        SerializedProperty incubationTime;
        SerializedProperty mortalityRate;
        SerializedProperty contagiousRate;
        SerializedProperty initialContagiousRadius;
        SerializedProperty strandType;
        SerializedProperty healthCarePolicyType;
        SerializedProperty effectRange;
        
        
        

        SerializedProperty vectorsOfTransmission;
        SerializedProperty typeOfAgent;
        SerializedProperty BasicVaccination;
        SerializedProperty AdvancedVaccination;
        

        SerializedProperty mentalDiseaseType;
        void OnEnable()
        {
            id = serializedObject.FindProperty("id");
            name = serializedObject.FindProperty("name");
            description = serializedObject.FindProperty("description");
            textForTheNews = serializedObject.FindProperty("textForTheNews");
            texture = serializedObject.FindProperty("texture");
            cost = serializedObject.FindProperty("cost");
            reputationImpact = serializedObject.FindProperty("baseReputationImpact");
            moneyDelta = serializedObject.FindProperty("moneyDeltaImpact");
            reputationDelta = serializedObject.FindProperty("reputationDeltaImpact");
            serverEvents = serializedObject.FindProperty("serverEvents");
            clientEvents = serializedObject.FindProperty("clientEvents");
            bothEvents = serializedObject.FindProperty("bothEvents");
            npcEvents = serializedObject.FindProperty("npcEvents");
            cardType = serializedObject.FindProperty("cardType");
            incubationTime = serializedObject.FindProperty("incubationTime");
            mortalityRate = serializedObject.FindProperty("mortalityRate");
            contagiousRate = serializedObject.FindProperty("contagiousRate");
            initialContagiousRadius = serializedObject.FindProperty("initialContagiousRadius");
            strandType = serializedObject.FindProperty("strandType");
            healthCarePolicyType = serializedObject.FindProperty("healthCarePolicyType");
            effectRange = serializedObject.FindProperty("effectRange");


            vectorsOfTransmission = serializedObject.FindProperty("vectorsOfTransmission");
            typeOfAgent = serializedObject.FindProperty("typeOfAgent");
            BasicVaccination = serializedObject.FindProperty("BasicVaccination");
            AdvancedVaccination = serializedObject.FindProperty("AdvancedVaccination");

            mentalDiseaseType = serializedObject.FindProperty("mentalDiseaseType");
            return;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(id);
            GUI.enabled = true;
            if (GUILayout.Button("Generate ID"))
            {
                id.stringValue = System.Guid.NewGuid().ToString();
            }
            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(textForTheNews);
            EditorGUILayout.PropertyField(texture);
            EditorGUILayout.PropertyField(cost);
            EditorGUILayout.PropertyField(reputationImpact);
            EditorGUILayout.PropertyField(moneyDelta);
            EditorGUILayout.PropertyField(reputationDelta);
            EditorGUILayout.PropertyField(serverEvents);
            EditorGUILayout.PropertyField(clientEvents);
            EditorGUILayout.PropertyField(bothEvents);
            EditorGUILayout.PropertyField(npcEvents);
            EditorGUILayout.PropertyField(cardType);
            switch ((CardType)cardType.enumValueIndex)
            {
                case CardType.disease:
                    EditorGUILayout.PropertyField(incubationTime);
                    EditorGUILayout.PropertyField(mortalityRate);
                    EditorGUILayout.PropertyField(contagiousRate);
                    EditorGUILayout.PropertyField(initialContagiousRadius);
                    EditorGUILayout.PropertyField(strandType);
                    EditorGUILayout.PropertyField(BasicVaccination);
                    EditorGUILayout.PropertyField(AdvancedVaccination);

                    //GUILayout.Label("Vectors Of Transmission: ");

                    //Enum enumNew = EditorGUILayout.EnumFlagsField((VectorsOfTransmission)vectorsOfTransmission.enumValueIndex);
                    // vectorsOfTransmission = EditorGUILayout.EnumFlagsField((VectorsOfTransmission)vectorsOfTransmission.enumValueIndex);
		            //vectorsOfTransmission.intValue = (int)Convert.ChangeType(enumNew, enumNew.GetType());
                    EditorGUILayout.PropertyField(vectorsOfTransmission);
                    EditorGUILayout.PropertyField(typeOfAgent);
                    break;
                case CardType.funCards:
                    break;
                case CardType.healthCarePolicy:
                    EditorGUILayout.PropertyField(healthCarePolicyType);
                    EditorGUILayout.PropertyField(strandType);
                    EditorGUILayout.PropertyField(effectRange);
                    break;
                case CardType.mentalDisease:
                    EditorGUILayout.PropertyField(mentalDiseaseType);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

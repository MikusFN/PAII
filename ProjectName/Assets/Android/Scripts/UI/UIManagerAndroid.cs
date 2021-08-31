using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Generic;
using UnityEngine.UI;
using Assets.Network;
using System;

namespace Assets.Android
{
    public enum AndroidScene
    {
        connect,
        game,
        research,
        actions,
    }
    public class UIManagerAndroid : SingletonBehaviour<UIManagerAndroid>
    {
        private RectTransform currentScene;

        //TOP AREA
        public RectTransform topArea;
        public Button gameButton;
        public Button researchButton;
        public Button actionButton;
        public Text moneyText;
        public Text reputationText;

        [Space(20)]
        //CONNECTING
        public RectTransform connectScene;
        public Image watingImg;
        public Text playerNameText;
        public Image playerColor;
        public Button readyButton;
        public Button hostStartButton;
        public RectTransform beforeReady;
        public RectTransform afterReady;
        public ColorPickerBehaviour colorPickTemplate;
        public RectTransform colorPickerParent;

        //GAME
        [Space(20)]
        public RectTransform gameScene;
        public RectTransform cardVisualizer;
        public AreaPickerBehaviour areaPicker;
        public PlayerPickerBehaviour playerPickerTemplate;
        public RectTransform playerPickerParent;
        public Text cardName;
        public Text cardDescription;
        public Image cardImage;
        public Text diseaseText;

        //RESEARCH
        [Space(20)]
        public RectTransform researchScene;
        public GameObject layoutResearch;
        public GameObject instanciablePrefab;

        //ACTION
        [Space(20)]
        public GameObject layoutPlayerList;
        public GameObject layoutDieasesList;
        public GameObject instanciablePlayerName;
        public GameObject instanciableDieasesName;

        public Text ACP; //Acused Player
        public Text ACD; //Acused Dieases
        public string deasesIDAcused;
        public string playerIDAcused;


        //VOTE SYSTEM
        [Space(20)]
        public GameObject voteParent;
        public  Text textToShowAcused;


        public Button yesHeDid;
        public Button noHeDidnot;
        //For endGame
        [Space(20)]
        public GameObject parentView;
        public GameObject winView;
        public GameObject loseView;
        public Button backToLobby;
        //Getter and setters
        public string DeasesIDAcused
        {
            get { return deasesIDAcused; }
            set
            {
                if (!playerIDAcused.Equals(""))
                {
                    SendAcusation(value, playerIDAcused);
                    deasesIDAcused = value;
                }
                else { deasesIDAcused = value; }
               
            }
        }

        public string PlayerIDAcused
        {
            get { return playerIDAcused; }
            set
            {
                if (!deasesIDAcused.Equals(""))
                {
                    SendAcusation(deasesIDAcused, value);
                    playerIDAcused = value;
                }
                else { playerIDAcused = value; }

            }
        }


        [Space(20)]
        public RectTransform actionScene;

        private void Start()
        {
            currentScene = connectScene;
            FillReaserchLayout();



            gameButton.onClick.AddListener(() => {
                SetCurrentScene(AndroidScene.game);
            });
            researchButton.onClick.AddListener(() =>
            {
                SetCurrentScene(AndroidScene.research);
            });
            actionButton.onClick.AddListener(() =>
            {
                SetCurrentScene(AndroidScene.actions);
            });
            backToLobby.onClick.AddListener(() =>
            {
                /**HERE CODE TO BACK TO LOBBY**/
            });

            yesHeDid.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.SendMessage(new MessageBase(true, MessageType.DecsionVote), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
                voteParent.SetActive(false);
            });

            noHeDidnot.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.SendMessage(new MessageBase(false, MessageType.DecsionVote), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
                voteParent.SetActive(false);
            });

            readyButton.onClick.AddListener(() =>
            {
                if (playerNameText.text != "" && playerColor.color != Color.white)
                {
                    NetworkAndroid.Instance.CurrentClient.IsReady = true;
                    SendNameColor();
                    beforeReady.gameObject.SetActive(false);
                    afterReady.gameObject.SetActive(true);
                }
            });
            readyButton.gameObject.SetActive(false);

            hostStartButton.onClick.AddListener(() =>
            {
                StartGame();
            });

            var colors = AssetManager.Instance.allPlayerColors;
            foreach(var color in colors){
                var obj = Instantiate(colorPickTemplate, colorPickerParent);
                obj.Init(color);
            }
        }


        //--------------------- CHANGE SCENE
        public void SetCurrentScene(AndroidScene nextScene)
        {
            currentScene.gameObject.SetActive(false);
            switch (nextScene)
            {
                case AndroidScene.connect:
                    topArea.gameObject.SetActive(false);
                    beforeReady.gameObject.SetActive(true);
                    afterReady.gameObject.SetActive(false);
                    if (!NetworkAndroid.Instance.isConnected)
                    {
                        readyButton.gameObject.SetActive(false);
                        watingImg.gameObject.SetActive(true);
                        NetworkAndroid.Instance.AutoConnect();
                    }else
                    {
                        readyButton.gameObject.SetActive(true);
                        
                    }
                    currentScene = connectScene;
                    break;
                case AndroidScene.game:
                    currentScene = gameScene;
                    break;
                case AndroidScene.research:
                    currentScene = researchScene;
                    break;
                case AndroidScene.actions:
                    FillDiseasesLayout();
                    FillPlayerLayout();
                    currentScene = actionScene;
                    break;
            }
            currentScene.gameObject.SetActive(true);
        }

        //--------------------- CONNECING
        public void ShowButtonStart()
        {
            hostStartButton.gameObject.SetActive(true);
        }
        public void ShowButtonReady()
        {
            if (!NetworkAndroid.Instance.CurrentClient.IsReady)
            {
                watingImg.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(true);
            }
        }
        public void SendNameColor()
        {
            var test = NetworkAndroid.Instance.CurrentClient;
            string name = playerNameText.text;
            NetworkAndroid.Instance.CurrentClient.Name = name;
            NetworkAndroid.Instance.CurrentClient.ClientColor = new ClientColor(playerColor.color);

            NetworkAndroid.Instance.SendMessage(new MessageBase(NetworkAndroid.Instance.CurrentClient, MessageType.ClientReady), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
        }
        public void SetPlayerColor(Color color)
        {
            playerColor.color = color;
            NetworkAndroid.Instance.CurrentClient.ClientColor = new ClientColor(color);
        }
        public void StartGame()
        {
            NetworkAndroid.Instance.SendMessage(new Network.MessageBase(null, MessageType.GameStart), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
        }
        public void OnGameStart()
        {
            topArea.gameObject.SetActive(true);
            UpdatePlayerStats();
        }

        //--------------------- GAME
        public void ShowCardInfo(Card card)
        {
            cardVisualizer.gameObject.SetActive(true);
            cardImage.overrideSprite = card.texture;
            cardName.text = card.name;
            cardDescription.text = card.description;
        }
        public void HideCardInfo()
        {
            cardVisualizer.gameObject.SetActive(false);
        }
        public void ShowAreaPicker()
        {
            areaPicker.gameObject.SetActive(true);
        }
        
        public void ShowMentalDiseasePlayerPicker()
        {
            playerPickerParent.gameObject.SetActive(true);

            foreach(var player in NetworkAndroid.Instance.OtherClients){
                var obj = Instantiate(playerPickerTemplate, playerPickerParent);
                obj.Init(player, (ShareableClient c) => {
                    NetworkAndroid.Instance.SendMessage(new MessageBase(new MentalDiseaseCardWrapper(HandController.Instance.currentCard ,c.Ip), MessageType.PlayMentalDiseaseCard), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
                    HideMentaDiseaesPlayerPicker();
                });
            }
        }
        public void HideMentaDiseaesPlayerPicker(){
            foreach(var child in playerPickerParent.GetComponentsInChildren<PlayerPickerBehaviour>()){
                Destroy(child.gameObject);
            }
            playerPickerParent.gameObject.SetActive(false);
        }
        public void ShowMentalText(string text, float duration){
            StopCoroutine("ShowMentalTextCoroutine");
            StartCoroutine(ShowMentalTextCoroutine(text, duration));
        }
        private IEnumerator ShowMentalTextCoroutine(string text, float duration){
            diseaseText.gameObject.SetActive(true);
            diseaseText.text = text;
            yield return new WaitForSeconds(duration);
            diseaseText.gameObject.SetActive(false);
        }

        public void UpdatePlayerStats()
        {
            moneyText.text = NetworkAndroid.Instance.CurrentClient.Money.ToString();
            reputationText.text = NetworkAndroid.Instance.CurrentClient.Reputation.ToString();
        }

        //--------------------- RESEARCH
        public void FillReaserchLayout()
        {
            foreach (Card c in AssetManager.Instance.GetAllCards())
            {
                GameObject instanciable = Instantiate(instanciablePrefab, layoutResearch.transform);
                instanciable.GetComponent<ResearchCardTemplateManager>().Init(c);
            }
        }

        //--------------------- ACTIONS
        public void FillPlayerLayout()
        {
          
                foreach (Transform child in layoutPlayerList.transform)
                {
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }
            

            foreach (ShareableClient c in NetworkAndroid.Instance.OtherClients)
            {
                GameObject instanciable = Instantiate(instanciablePlayerName, layoutPlayerList.transform);
                instanciable.GetComponent<PlayerNameIDTemplate>().Init(c);
            }
        }

        public void FillDiseasesLayout()
        {
            
          foreach (Transform child in layoutDieasesList.transform)
            {
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            foreach (ShareableDisease c in NetworkAndroid.Instance.AllDiseases)
            {
                GameObject instanciable = Instantiate(instanciableDieasesName, layoutDieasesList.transform);
                instanciable.GetComponent<DieaseNameIDPlayerTemplate>().Init(c);
            }
        }

        public void FillOnGoingReaserchLayout()
        {

        }

        public void SendAcusation( string idDoença, string idPlayer)
        {
            /**passa Aqui**/
            DiseasePlayerAcusationWarper acusation = new DiseasePlayerAcusationWarper(idPlayer, idDoença);
            NetworkAndroid.Instance.SendMessage(new MessageBase(acusation, MessageType.AcusingAPlayer), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
        }

        public void FillAndShowVote(string nameSupect, string diseaseSuspect)
        {
            parentView.SetActive(true);
            voteParent.SetActive(true);
            textToShowAcused.text = "Is " + nameSupect + "responsable for " + diseaseSuspect + "?";
        }


        /**when game ends show this **/
        public void ShowWinOrLose(bool didIWin)
        {
            parentView.SetActive(true);

            if (didIWin) { winView.SetActive(true); }
            else loseView.SetActive(true);
        }
    }
}

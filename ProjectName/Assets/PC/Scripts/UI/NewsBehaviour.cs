using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewsBehaviour : MonoBehaviour
{
    private Queue<string> newsAwaiting;
    private Queue<Text> newsCurrentlyBeingProcess;
    private string loopableNews;


    public float newsVelocity;
    [Range(0, 1)]
    public float spaceBetweenNews;
    public Text newsTemplate;
    private void Awake() {
        newsAwaiting = new Queue<string>();
        newsCurrentlyBeingProcess = new Queue<Text>();
    }

    public void Enqueue(string news){

        if(newsCurrentlyBeingProcess.Count > 0 && !HasSpace())
            newsAwaiting.Enqueue(news);
        else {
            var obj = GenerateText(news);
            newsCurrentlyBeingProcess.Enqueue(obj);
        }        
    }
    public void ClearAll(){
        foreach(Text t in newsCurrentlyBeingProcess){
            Destroy(t.gameObject);
        }
        newsAwaiting = new Queue<string>();
        newsCurrentlyBeingProcess = new Queue<Text>();
        loopableNews = "";
    }
    public void EnqueueLoop(string newsLoop){
        loopableNews = newsLoop;
        Enqueue(newsLoop);
    }
    public void StopLoop(){
        loopableNews = "";
    }

    private void Update() {

        if(newsCurrentlyBeingProcess.Count > 0){
            foreach(Text t in newsCurrentlyBeingProcess) {
                (t.gameObject.transform as RectTransform).anchoredPosition += Vector2.left * newsVelocity * Time.deltaTime;
            }

            if(HasSpace()){
                if(newsAwaiting.Count > 0){
                    var textObj = GenerateText(newsAwaiting.Dequeue());
                    newsCurrentlyBeingProcess.Enqueue(textObj);
                }
                else if(loopableNews != ""){
                    Enqueue(loopableNews);
                }
            }
            if(HasFinsished()){
                Destroy(newsCurrentlyBeingProcess.Dequeue().gameObject);
            }         
        }
    }

    private bool HasFinsished(){
        var first = newsCurrentlyBeingProcess.First().transform as RectTransform;
        if(-first.anchoredPosition.x > first.sizeDelta.x)
            return true;
        return false;
    }
    private bool HasSpace(){
        var last = newsCurrentlyBeingProcess.Last().transform as RectTransform;
        var thisTransform = this.transform as RectTransform;
        if(last.anchoredPosition.x + last.rect.width < thisTransform.rect.width - thisTransform.rect.width * spaceBetweenNews)
            return true;
        return false;
    }
    private Text GenerateText(string text){
        var textObj = Instantiate(newsTemplate, Vector2.zero, Quaternion.identity, this.transform);
        textObj.text = text;
        (textObj.transform as RectTransform).anchoredPosition = new Vector2((this.transform as RectTransform).rect.width, 0);
        return textObj;
    }
}

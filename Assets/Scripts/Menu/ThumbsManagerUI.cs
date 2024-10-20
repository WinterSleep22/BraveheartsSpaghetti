using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ThumbsManagerUI : MonoBehaviour {


	public Text deckCounter;
	public GameObject prefabThumbUI;
	public VerticalLayoutGroup myDeckUI;

	public void CreateDeckUI(){
		
		myDeckUI.transform.DestroyAllChildren ();
		Dictionary<string,int> frequencyCardDeck = FindObjectOfType<MyDeckMenu> ().GetFrequencyCardDeck ();
		foreach (var titleCard in frequencyCardDeck.Keys) {
			AddThumbUI (titleCard);
		}
	}

	
	ThumbUI AddThumbUI(string title){
		GameObject newThumbUI = GameObject.Instantiate (prefabThumbUI);
		newThumbUI.transform.SetParent (myDeckUI.transform);
		newThumbUI.transform.SetAsFirstSibling ();
		newThumbUI.GetComponent<ThumbUI> ().myTitleCard = title;
		newThumbUI.transform.localScale = Vector3.one;
		UpdateDeckCounter ();
		return newThumbUI.GetComponent<ThumbUI> ();
	}

	void UpdateDeckCounter(){
		int counter = FindObjectOfType<MyDeckMenu> ().deck.Count;
		//Debug.Log ("UpdateDeckCounter title=" + counter);
		deckCounter.text = (counter-1) + "/40";
	}

	void RemoveThumbUI(string title){
		var thumbUI = GetComponentsInChildren<ThumbUI> ().ToList ().Find (t=>t.myTitleCard==title);
		Destroy(thumbUI.gameObject);
		UpdateDeckCounter ();
	}

	public void UpdateThumbUI(string title,int frequency){
		//Debug.Log ("UpdateThumbUI title=" + title +" frequency=" + frequency);
		var thumbUI = GetComponentsInChildren<ThumbUI> ().ToList ().Find (t=>t.myTitleCard==title);
		if (thumbUI == null) {
			if (frequency == 0) {
				return;
			} else {
				thumbUI = AddThumbUI (title);
			
			}
		} else {
			if(frequency==0){
				RemoveThumbUI (title);
				return;
			}
		}
		UpdateDeckCounter ();
		thumbUI.myTitleCard = title;
		thumbUI.GetComponent<Image> ().sprite = CardManager.GetCardPrefabByTitle (title).thumb;
		thumbUI.GetComponentInChildren<Text> ().text = frequency.ToString ();
		 
	}
		

	public void UpdateDeckUI(){

		//update card UI according to modifications
		Dictionary<string,int> frequencyCardDeck =  FindObjectOfType<MyDeckMenu>().GetFrequencyCardDeck();


		foreach(var titleCard in frequencyCardDeck.Keys){
			UpdateThumbUI (titleCard,frequencyCardDeck[titleCard]);

		}



	}

}

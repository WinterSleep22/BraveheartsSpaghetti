using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ThumbUI : MonoBehaviour,IPointerClickHandler, IPointerDownHandler,IPointerUpHandler{


	public string myTitleCard;

	public void OnPointerClick(PointerEventData data){

		//FindObjectOfType<MyDeckMenu>().RemoveCardFromMyDeck(myTitleCard);
		CardUI myCardUI = MenuManager.instance.GetComponentInChildren<CardsManagerUI>().listCards.Find(c=>c.myCardTitle == myTitleCard);

		if(myCardUI==null){
			Debug.Log ("Failed");
			return;
		}

		if (MenuManager.instance.GetComponentInChildren<MyDeckMenu> ().IsButton1Pressed) {
			bool sucess= MenuManager.instance.GetComponentInChildren<MyDeckMenu> ().RemoveCardFromMyDeck (myCardUI.myCardTitle);
			if (sucess) {
				SoundManagerMenu.instance.BackSound ();
			} else {
				SoundManagerMenu.instance.incorrectSound.Play ();
			}
		} else {
			myCardUI.ZoomIn (nextToDeck: true);
			SoundManager.instance.cardSelect.Play ();
		}
	}

	bool isPointerDown=false;

	public void OnPointerDown(PointerEventData eventData){
		isPointerDown = true;
		this.ExecuteIn (1.2f,()=>{
			if(isPointerDown){
				OnPressStill();
			}

		});

	}

	public void OnPointerUp(PointerEventData eventData){
		isPointerDown = false;
	}
	void OnPressStill(){
		Debug.Log ("OnPressStill");

		CardUI myCardUI = MenuManager.instance.GetComponentInChildren<CardsManagerUI>().listCards.Find(c=>c.myCardTitle == myTitleCard);

		if(myCardUI==null){
			Debug.Log ("Failed");
			return;
		}

		myCardUI.ZoomIn (nextToDeck: true);
		SoundManager.instance.cardSelect.Play ();

	}

}

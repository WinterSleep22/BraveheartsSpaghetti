using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler,IPointerDownHandler,IPointerUpHandler {


	public string myCardTitle;
	public Image cardImage;
	public Text counterText;

	Image thumbToStay=null;

	public float zoom=2f;



	Vector3 localpos;
	Transform myParent;
	bool openedByDeck=false;

	bool isPointerDown=false;

	void Start(){
		myParent = transform.parent;
		localpos = transform.localPosition;
	}

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


	public void OnPointerClick (PointerEventData eventData){

		if(GetComponent<iTween>()!=null){
			return;
		}
		FindObjectOfType<CardsManagerUI> ().OnClickInsideCard (this,openedByDeck);
	}





	public bool isZoomed{ get { return cardImage.transform.localScale != Vector3.one;} }


	void OnPressStill(){
		Debug.Log ("OnPressStill");
		ZoomIn ();
		SoundManager.instance.cardSelect.Play ();

	}


	public void ZoomIn(bool nextToDeck=false){

		//useful to decide which part of screen to move
		int colum = cardImage.transform.parent.GetSiblingIndex () % 4;
		int direction = (colum > 1 ) ? -1:1;//right or left?

		//make a copy to stay visible
		if(thumbToStay == null){
			thumbToStay = GameObject.Instantiate (cardImage.gameObject).GetComponent<Image>();
		
			thumbToStay.transform.SetParent (cardImage.transform.parent);
			thumbToStay.transform.position = cardImage.transform.position;
			thumbToStay.transform.localScale = Vector3.one;
			thumbToStay.gameObject.SetActive (true);
			thumbToStay.rectTransform.SetSize (cardImage.rectTransform.GetSize());

			//thumbToStay.rectTransform.SetBottomLeftPosition (Vector3.zero);
			Destroy(thumbToStay.GetComponent<CardUI> ());
		}

		cardImage.transform.SetParent (FindObjectOfType<Canvas>().transform);
		cardImage.transform.SetAsLastSibling ();
		if(nextToDeck){
			cardImage.transform.position = MyDeckMenu.instance.thumbsManagerUI.transform.position;
		}

	

		Vector3 worldPosNotOpenedByDeck = 
			Vector3.up * MenuManager.instance.canvas.transform.position.y  
			+ Vector3.right * thumbToStay.transform.position.x  
			+ Vector3.right * direction *( 300) * Screen.width/1280  ;
		//Debug.Log ("cardImage.width="+cardImage.rectTransform.GetWidth());

		Vector3 worldPosOpenByDeck =
			MyDeckMenu.instance.thumbsManagerUI.transform.position
			+ Vector3.right * 350 * Screen.width/1280;


		Hashtable hash = new Hashtable();
		hash.Add("position",(nextToDeck) ? worldPosOpenByDeck : worldPosNotOpenedByDeck );
		hash.Add ("time",0.5f);

		iTween.MoveTo (cardImage.gameObject,hash);
		iTween.ScaleTo (cardImage.gameObject,Vector3.one * zoom,0.5f);

		openedByDeck = nextToDeck;
		MyDeckMenu.instance.cardsManagerUI.backgroundForSelection.enabled = true;
		MyDeckMenu.instance.cardsManagerUI.currentSelectedCard = this;
	}

	public void ZoomOut(float duration=0.5f){

		Hashtable hash = new Hashtable();
		hash.Add ("position", ((openedByDeck) ? MenuManager.instance.canvas.transform.position +Vector3.left * Screen.width/2f*0.8f:  thumbToStay.transform.position));
		hash.Add ("time",duration);
		hash.Add ("oncompletetarget",gameObject);
		hash.Add ("oncomplete","Adjust");
		iTween.MoveTo (gameObject,hash);
		iTween.ScaleTo (cardImage.gameObject,Vector3.one ,0.5f);
		this.ExecuteIn (duration,()=>{
			cardImage.transform.SetParent (myParent);
		});

		//addPanel.gameObject.SetActive (false);
		openedByDeck = false;
	}


	public void Adjust(){
		transform.localPosition = localpos;
		thumbToStay.gameObject.SetActive (true);
	}

}

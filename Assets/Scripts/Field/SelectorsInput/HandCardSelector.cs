using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// provides basic input function callbacks to select card on hand
/// </summary>
public abstract class HandCardSelector : MonoBehaviour {

    public GraphicRaycaster graphicRaycaster;
    Hand _myHand;
    public Hand myHand
    {
        get
        {
            if(_myHand ==null){
                _myHand = GetComponent<Hand>();
            }
            return _myHand;
        }
    }

    bool pointerDown = false;
    float timeHold = 1;
    bool holding = false;
    float timerholding = 0;
	void Update () {

        if(Input.GetMouseButtonDown(0)){
            holding = false;
            pointerDown = true;
            timerholding = 0;
        }
        if(pointerDown){
            timerholding += Time.deltaTime;
        }
        if(timerholding > timeHold){
            holding = true;
        }
        if (Input.GetMouseButtonUp(0) && !holding)
        {
            holding = false;
            pointerDown = false;
            timerholding = 0;

            //Create the PointerEventData with null for the EventSystem
            PointerEventData ped = new PointerEventData(null);
            //Set required parameters, in this case, mouse position
            ped.position = Input.mousePosition;
            //Create list to receive all results
            List<RaycastResult> results = new List<RaycastResult>();
            //Raycast it
            graphicRaycaster.Raycast(ped, results);

            bool clickedOnACard = results.Exists(result =>
            {
                Card handCard = result.gameObject.GetComponent<Card>();
                return isClickedCard(handCard);
            });
            if (!clickedOnACard)
            {
                OnClickedOut();
            }
        }
	}

    bool isClickedCard(Card handCard)
    {
        if (handCard == null)
        {
            return false;
        }
        if (handCard.myDrag.IsDragging)
        {

            return false;
        }
      

        if (myHand.cards.Contains(handCard))
        {
            OnCardClicked(handCard);
            return true;
        }

        return false;
    }
   
    public virtual void OnClickedOut()
    {

    }
   
    public virtual void OnCardClicked(Card card)
    {
       
    }
   
  
}

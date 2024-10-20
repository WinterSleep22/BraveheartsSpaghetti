using UnityEngine;
using System.Collections;

public class UsedCardsSlot : CardSlot {

    public override void Awake()
    {

        base.Awake();
        myDropZone.max = 41;
        myDropZone.allowDrop = false;
        myDropZone.DetectDragChilds();
    }

    public override bool OnAllowCardAdd(Card card)
    {
        return base.OnAllowCardAdd(card);
    }
    public override void OnCardEndComeBackDrag(Card _card)
    {
        _card.myCardGraphics.myImage.enabled = false;
    }
    public override void OnCardEnter(Card _card)
    {
        // Debug.Log("DeckSlot cardAdded=" + drag.GetComponent<Card>().title);
        _card.myDrag.allowDrag = false;
       
        iTween.RotateTo(_card.gameObject,Vector3.zero,1f);
    }

    public override void OnCardExit(Card _card)
    {
        _card.myDrag.allowDrag = true;
        _card.myCardGraphics.myImage.enabled = true;

    }
}

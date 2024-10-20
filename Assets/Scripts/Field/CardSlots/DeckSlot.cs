using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DropZone))]
public class DeckSlot : CardSlot {


	public override void Awake () {

        
        base.Awake();
        myDropZone.max = 41;
        myDropZone.allowDrop = false;
    
      
    }

    public override void Start()
    {
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
       //Debug.Log("DeckSlot cardAdded=" + _card.title);
        base.OnCardEnter(_card);
        _card.myDrag.allowDrag = false;

    }

    public override void OnCardExit(Card _card)
    {
        _card.myDrag.allowDrag = true;
        _card.myCardGraphics.myImage.enabled = true;
        if(cards.Count == 0){
            OnDeckRunOutOfCards();
        }

    }

    void OnDeckRunOutOfCards()
    {
        LogTextUI.Log("Used Cards-> Deck Cards Shuffeling");
        var memory =  myPlayer.myBoard.usedCardsSlot.cards.ToArray();
        foreach(var usedCard  in memory){
            usedCard.myDrag.MoveToNewDropZone(this.myDropZone);
        }
        this.cards.Shuffle();
    }
}

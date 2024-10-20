using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public abstract class HelperCardSlot : CardSlot {

  
    public override void OnCardEndComeBackDrag(Card _card)
    {
        if (_card.myCardGraphics.isShowingBack)
        {

        }
        else
        {
            base.OnCardEndComeBackDrag(_card);
        }
        if (!_card.myCardGraphics.isShowingBack)
            AdjustCardPosition(_card);
    }
    public override void OnCardExit(Card _card)
    {
        base.OnCardExit(_card);
        GetComponent<Mask>().enabled = true;
    }
    public override void OnCardEnter(Card _card)
    {

        base.OnCardEnter(_card);
        if (!_card.myCardGraphics.isShowingBack)
        {
            AdjustCardPosition(_card);
            GetComponent<Mask>().enabled = false;
        }
    }
    public void AdjustCardPosition(Card card)
    {
        Vector3 adjust = Vector3.down*15;
        iTween.MoveTo(card.gameObject , iTween.Hash(
            "position", adjust,
            "time", 0.5f,
            "islocal",true
            ));
    }

    public override bool OnAllowCardAdd(Card card)
    {
        if(!base.OnAllowCardAdd(card))
        {
            return false;
        }
        return card is HelperCard;
    }
    


    public void RevealCard()
    {
        if (cards.Count == 0)
        {
           // Debug.Log("HelperCardSlot.RevealCard: no cards to reveal");
            return;
        }
        if (!cards[0].myCardGraphics.isFlipped)
        {
            //Debug.Log("HelperCardSlot.RevealCard: card was already show");
            return;
        }
        cards[0].myCardGraphics.ShowFrontOnField();
        AdjustCardPosition(cards[0]);
    }
}

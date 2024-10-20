using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public abstract class BuildingCard : ObjectCard {

    
    public int cost;
    public int tech;
    Card[] cardsToPay = null;

    public bool IsTechAllowed()
    {
        if(tech<=1){
            return true;
        }
        else
        {
            return myPlayer.myBoard.TryGetAllCardsOnField<BuildingCard>().Exists(b => { return b.tech == tech - 1; });
        }
    }
   
    
    bool AreCardsToPayAllowed(Card[] cards)
    {
        if(cost==0){
            return true;
        }
        if(cards==null){
            return false;
        }
        foreach (var card in cards)
        {
            if(card == this){
                Debug.Log("TRying to pay card with own building card");
                return false;
            }
        }
        if (cards.Length == cost)
        {
           // Debug.Log("cards to pay allowed!");
            return true;
        }
        else
        {
            Debug.Log("Incorrect amount of cards wrong=" + cards.Length +" right=" + cost);
            return false;
        }
    }
    public override void OnActivatingForcedCancel()
    {
        myPlayer.myBoard.myHand.GetOrAddComponent<PlayerHandCardPaySelector>().enabled = false;
    }

   
    public override bool IsAllowedToActivate()
    {
        //this is ugly: if it is normal builind check if any building was constructed before
        if (myPlayer.status.normalBuildingCardSetInThisTurn && this is NormalBuildingCard ) 
        {
            Debug.Log("Max contructed Normal Building cards in this turn");
            if (myPlayer is SelfPlayer)
            {               
                LogTextUI.Log("You already built in this turn");
            }
            return false;
        }
         if (myPlayer.status.specialBuildingCardSetInThisturn && this is SpecialBuildingCard )
         {
             Debug.Log("Max contructed Special Building cards in this turn");
             if (myPlayer is SelfPlayer)
             {
                 LogTextUI.Log("You already built in this turn");
             }
             return false;
         }
        if (!IsTechAllowed())
        {
            if (myPlayer is SelfPlayer)
            {
                LogTextUI.Log("Need a Tech" + (tech - 1) + " building on the field");
            }
            return false;

        }
        if(myPlayer.myBoard.GetFreeCardSlot(this)==null){
            Debug.Log("No free slot for " + title);
            return false;
        }

        if (myPlayer.myBoard.normalBuildingCardSlotLeft.cards.Count > 0)
            if (myPlayer.myBoard.normalBuildingCardSlotLeft.cards[0].title == this.title)
                return false;
        if (myPlayer.myBoard.normalBuildingCardSlotMiddle.cards.Count > 0)
            if (myPlayer.myBoard.normalBuildingCardSlotMiddle.cards[0].title == this.title)
                return false;
        if (myPlayer.myBoard.normalBuildingCardSlotRight.cards.Count > 0)
            if (myPlayer.myBoard.normalBuildingCardSlotRight.cards[0].title == this.title)
                return false;

        if (myPlayer.myBoard.specialBuildingCardSlotLeft.cards.Count > 0)
            if (myPlayer.myBoard.specialBuildingCardSlotLeft.cards[0].title == this.title)
                return false;
        if (myPlayer.myBoard.specialBuildingCardSlotRight.cards.Count > 0)
            if (myPlayer.myBoard.specialBuildingCardSlotRight.cards[0].title == this.title)
                return false;

        return true;
    }


    /// <summary>
    /// called when card is activate by Selectable in Hand slot, manages payment state to place building card
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator Activating()
    {
        
        if (IsAllowedToActivate()) //this is ugly: if it is normal builind check if any building was constructed before
        {
           
            //  Debug.Log("Building Card="+title+" tech=" + tech +" cost=" + cost);
            targetSlot = myPlayer.myBoard.GetFreeCardSlot(this);         
            if(targetSlot==null){
                Debug.Log("_________Error: no space for building " + title);
            }
            MoveToCenter();

            //player input to select card, if AI, another external script do the job
            if (myPlayer is SelfPlayer && cost > 0)
            {
                if ( myPlayer is SelfPlayer)
                    LogTextUI.Log("Select " + cost + " card" + (cost > 1 ? "s" : "") + " for the cost");
                           

                Debug.Log("PlayerHandCardPaySelector enabled ");
                PlayerHandCardPaySelector selector = myPlayer.myBoard.myHand.GetOrAddComponent<PlayerHandCardPaySelector>();
                selector.enabled = true;
                selector.Initialize(this);
            }
                        
            bool payAllow=false;

            float maxtime = 10;
            float currentTime = 0;
            while (true)
            {
                currentTime += 0.5f;
                if(currentTime > maxtime){
                    Debug.Log("Max time reached to pay cards " + gameObject.name);
                    break;
                }
                if (AreCardsToPayAllowed(cardsToPay))
                {
                    payAllow = true;
                    break;
                }
                Debug.Log("building " + title+" waiting pay");
                yield return new WaitForSeconds(0.5f);
            }

            if (payAllow)
            {
                yield return StartCoroutine(ApplyingPayment());
            }
            else
            {
                OnActivatingEnd();
            }

        }
        else
        {
            OnActivatingEnd();
        }
        yield return null;
    }


    public bool SelectCardsToPay(Card[] cards)
    {
        //LogTextUI.Log("Cards Selected to pay=" + cards.Length);

        if (AreCardsToPayAllowed(cards))
        {
            cardsToPay = cards;
            return true;
        }
        else
        {
           
            CancelActivating();
            myDrag.ComeBackToDropZone();
            Debug.Log("cards not allowed to pay card=" + gameObject.name);
            return false;
        }
    }

    IEnumerator ApplyingPayment()
    {

        
        Debug.Log("All Cards Paid");

        if (cardsToPay!=null) //null for cards with cost 0
        {
            //pay cards
            foreach (var cardToPay in cardsToPay)
            {
                cardToPay.myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone, 1f);
            }
        }


        OnActivatingEnd(new Action(instance_id, false, null, CardManager.GetIdByCards(cardsToPay)));
       
        yield return new WaitForSeconds(1f);

        myDrag.MoveToNewDropZone(targetSlot.myDropZone, 2f);

        yield return new WaitForSeconds(2f);
       
       

    }

}

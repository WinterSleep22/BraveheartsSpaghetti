using UnityEngine;
using System.Collections;

public class Insight : EffectCard{

    public override IEnumerator OnCardRevealedStartTurnPreAttack()
    {
        yield return  StartCoroutine(ActivitingInsightEffect());  

    }

    IEnumerator ActivitingInsightEffect()
    {

        Debug.Log(" Activiting insight Effect !");

        MoveToCenter();
        yield return new WaitForSeconds(1f);
        if (myPlayer.otherPlayer.myBoard.flippedCardSlot.cards.Count==1)
        {
            if (myPlayer.otherPlayer.myBoard.flippedCardSlot.cards[0] is BasicDefense)
            {

                myPlayer.otherPlayer.myBoard.flippedCardSlot.RevealCard();
                Debug.Log("insight: Basic Defense Revealed!");
            }
            else if(myPlayer.otherPlayer.myBoard.flippedCardSlot.cards[0] is Bash)
            {
                myPlayer.otherPlayer.myBoard.flippedCardSlot.RevealCard();
                Debug.Log("insight: Bash Revealed!");
            }
            else
            {
                Debug.Log("insight: No Basic Defense to be revealed");
                if(myPlayer is SelfPlayer)
                    LogTextUI.Log("That was not a "+CardManager.GetCardOfType<BasicDefense>().title+" card");
            }
        }
        else
        {
            Debug.Log("Insight Activating: no cards on flipped slot ");
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("There’s no flipped card to choose");
        }

        Debug.Log("Insight to Used Deck");
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
        yield return new WaitForSeconds(2f);
    }
   
    public override bool IsAllowedToActivate()
    {
        if(myCardGraphics.isShowingBack){
            if(myPlayer.myBoard.flippedCardSlot.cards.Count==1){
                Debug.Log("Insight: Already has flipped card on my board");
                return false;
            }
        }
        
        if(myCardGraphics.isShowingFront){
             //is it avaiable if there is no flipped opponent card
            if (myPlayer.otherPlayer.myBoard.flippedCardSlot.cards.Count==0)
            {
                Debug.Log("Insight: No cards flipped to be revealed on opponent board");
                return false;
            }
        }
        return true;
    }
    
    protected override IEnumerator Activating()
    {
        if(IsAllowedToActivate()){
            if(myCardGraphics.isShowingBack){
                myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
                OnActivatingEnd(new Action(instance_id, true));
            }
            else
            {
                yield return StartCoroutine(ActivitingInsightEffect());
                OnActivatingEnd(new Action(instance_id, false));
            }
        }
        else
        {
            OnActivatingEnd();
        }
    }

}

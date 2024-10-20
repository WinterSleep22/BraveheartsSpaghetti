using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meal : EffectCard {

    public override bool IsAllowedToActivate()
    {
        if (myPlayer.myBoard.TryGetCardOnField<Supplier>() == null)
        {
            Debug.Log("Cant Use Meal without Supplier Helper on Field");
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("Need a " + CardManager.GetCardOfType<Supplier>().title + " card");
            return false;
        }

        return base.IsAllowedToActivate();
    }

    protected override IEnumerator Activating()
    {
        if (!IsAllowedToActivate())
        {
            Debug.Log("Not allowed to activate", this);
            OnActivatingEnd();
            yield break;
        }

        if (myCardGraphics.isShowingFront)
        {
            //Normal usage
            Debug.Log("Activating meal [showing front]");
            yield return ActivatingMeal();
        }
        else
        {
            //Flipped
            Debug.Log("Activating meal [showing back]");
            yield return ActivatingMealFlipped();
        }
    }

    public IEnumerator ActivatingMeal()
    {
        MoveToCenter();
        yield return new WaitForSeconds(1.5f);
        yield return myPlayer.DrawTopCardFromMyDeck();

        yield return new WaitForSeconds(0.1f);
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
        OnActivatingEnd(new Action(instance_id, false));
    }

    public IEnumerator ActivatingMealFlipped()
    {
        if (myPlayer.myBoard.flippedCardSlot.cards.Count > 0)
        {
            Debug.Log("Meal - cancelling activationl; Flipped Cards Slot not empty", this);
            OnActivatingEnd();
            yield break;
        }
        myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
    }

    //bool attackSuffered = false;

    //public override IEnumerator OnCardRevealedStartTurnPreAttack()
    //{
    //    if (myPlayer.myBoard.attackWaitListSlot.cards.Count > 0)
    //        attackSuffered = true;
    //    yield return null;
    //}

    public override IEnumerator OnCardRevealedStartTurnPosAttack()
    {
        //if (attackSuffered)
        //{
        //    LogTextUI.Log("The " + CardManager.GetCardOfType<Meal>().title + " is not activated");
        //    Debug.Log("Meal Effect Not activated, player suffer attacks in the last turn");
        //    OnActivatingEnd();
        //}
        /*else*/ yield return ActivatingMeal();
    }
}

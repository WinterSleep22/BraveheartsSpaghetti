using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Provision : EffectCard {

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
            Debug.Log("Activating provision [showing front]");
            yield return ActivatingProvision();
        }
        else
        {
            Debug.Log("Activating provision [showing back]");
            yield return ActivatingProvisionFlipped();
        }
    }

    public IEnumerator ActivatingProvision()
    {
        Card c;
        MoveToCenter(1, 0.1f);
        yield return new WaitForSeconds(1.5f);
        c = myPlayer.DrawTopCardFromMyDeck();
        yield return null;
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);

        yield return new WaitForSeconds(0.1f);

        if (c.GetType() == typeof(BasicAttack))
        {
            //Debug.LogWarning("Its attack");
            yield return new WaitForSeconds(0.5f);
            LogTextUI.Log("Drawn Attack; drawing one more");
            myPlayer.DrawTopCardFromMyDeck();
        }
        yield return new WaitForSeconds(0.3f);

        OnActivatingEnd(new Action(instance_id, false));
    }

    public IEnumerator ActivatingProvisionFlipped()
    {
        if (myPlayer.myBoard.flippedCardSlot.cards.Count > 0)
        {
            Debug.Log("Provision - cancelling activationl; Flipped Cards Slot not empty", this);
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
        //    LogTextUI.Log("The " + CardManager.GetCardOfType<Provision>().title + " is not activated");
        //    Debug.Log("Meal Effect Not activated, player suffer attacks in the last turn");
        //    OnActivatingEnd();
        //}
        /*else*/
        yield return ActivatingProvision();
    }
}

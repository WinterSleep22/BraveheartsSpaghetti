using UnityEngine;
using System.Collections;

public class Supply : EffectCard {

    bool attackSuffered = false;

    public override IEnumerator OnCardRevealedStartTurnPreAttack()
    {
        if (myPlayer.myBoard.attackWaitListSlot.cards.Count > 0)
            attackSuffered = true;
        yield return null;
    }

    public override IEnumerator OnCardRevealedStartTurnPosAttack()
    {
        yield return StartCoroutine(ActivatingSupplyEffect());
    }

    IEnumerator ActivatingSupplyEffect()
    {
        Supplier supplier = myPlayer.myBoard.TryGetCardOnField<Supplier>();

        if (!attackSuffered)
        {
            if (supplier != null)
            {
                MoveToCenter();
                Debug.Log("Supply Effect: Draw 2+ Cards");
                yield return new WaitForSeconds(1f);
                myPlayer.DrawTopCardFromMyDeck();
                yield return new WaitForSeconds(1f);
                myPlayer.DrawTopCardFromMyDeck();
                yield return new WaitForSeconds(1f);

            }
            else
            {
                LogTextUI.Log("The " + CardManager.GetCardOfType<Supply>().title + " is not activated");
                Debug.Log("Supply Effect: no supplier on field");
            }
        }
        else
        {
            LogTextUI.Log("The " + CardManager.GetCardOfType<Supply>().title + " is not activated");
            Debug.Log("Supply Effect Not activated, player suffer attacks in the last turn");
        }
        Debug.Log("Supply to Used Deck");
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
    }




    public override bool IsAllowedToActivate()
    {

        if (myPlayer.otherPlayer.myBoard.attackWaitListSlot.cards.Count == 1)
        {
            if (myPlayer is SelfPlayer)
            {
                LogTextUI.Log("You already attacked in this turn");
            }
            Debug.Log("Cant Use Supply: PLayer already made a attack");
            return false;
        }



        if (myPlayer.myBoard.TryGetCardOnField<Supplier>() == null)
        {
            Debug.Log("Cant Use Supply Without Supplier Helper on Field");
            if (myPlayer is SelfPlayer)
            {
                LogTextUI.Log("Need a " + CardManager.GetCardOfType<Supplier>().title + " card");

            }
            return false;
        }
        return base.IsAllowedToActivate();
    }

    protected override IEnumerator Activating()
    {
        if (IsAllowedToActivate())
        {
            myCardGraphics.ShowBack();
            myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
            OnActivatingEnd(new Action(instance_id, true));
            yield return new WaitForSeconds(1f);
        }
        else
            OnActivatingEnd();
    }

}

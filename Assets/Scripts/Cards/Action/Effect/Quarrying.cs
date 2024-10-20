using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Quarrying : EffectCard {

    public override int cost { get { return 3; } }
    Card[] cardsToPay = null;

    public override bool IsAllowedToActivate()
    {
        if (myPlayer.myBoard.myHand.cards.Count <= cost)
        {
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("No enough cards for cost");
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
            Debug.Log("Activating quarrying [showing front]");
            isPayingFromFlipped = false;
            yield return ActivatingQuarrying();
        }
        else
        {
            //Flipped
            Debug.Log("Activating quarrying [showing back]");
            isPayingFromFlipped = true;
            yield return ActivatingQuarryingFlipped();
        }
    }

    IEnumerator ActivatingQuarrying()
    {
        if (IsAllowedToActivate())
        {
            if (myPlayer is SelfPlayer)
                MoveToCenter();
            else MoveToCenter(1, 0);

            //player input to select card, if AI, another external script do the job
            if (myPlayer is SelfPlayer && cost > 0)
            {
                if (myPlayer is SelfPlayer)
                    LogTextUI.Log("Select " + cost + " card" + (cost > 1 ? "s" : "") + " for the cost");

                Debug.Log("PlayerHandCardEffectPaySelector enabled ");
                PlayerHandCardEffectPaySelector selector = myPlayer.myBoard.myHand.GetOrAddComponent<PlayerHandCardEffectPaySelector>();
                selector.enabled = true;
                selector.Initialize(this);
            }
            else yield return new WaitForSeconds(1);

            bool payAllow = false;

            float maxtime = 10;
            float currentTime = 0;
            while (true)
            {
                currentTime += 0.5f;
                if (!isPayingFromFlipped)
                    if (currentTime > maxtime)
                    {
                        myPlayer.myBoard.myHand.GetComponent<PlayerHandCardEffectPaySelector>().OnClickedOut();
                        Debug.Log("Max time reached to pay cards " + gameObject.name);
                        break;
                    }
                if (AreCardsToPayAllowed(cardsToPay))
                {
                    payAllow = true;
                    break;
                }
                Debug.Log("effect " + title + " waiting pay");
                yield return new WaitForSeconds(0.5f);
            }

            if (payAllow)
                yield return StartCoroutine(ApplyingPayment());
            else
                OnActivatingEnd();
        }
        else
            OnActivatingEnd();
        yield return null;
    }

    IEnumerator ActivatingQuarryingFlipped()
    {
        if (myPlayer.myBoard.flippedCardSlot.cards.Count > 0)
        {
            Debug.Log("Quarrying - cancelling activationl; Flipped Cards Slot not empty", this);
            OnActivatingEnd();
            yield break;
        }
        myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
    }

    public override IEnumerator OnCardRevealedStartTurnPosAttack()
    {
        if (myPlayer is SelfPlayer)
            yield return ActivatingQuarrying();
        else yield return PlayerInputControl.instance.UseQuarryingFlippedPostattack();
    }

    bool AreCardsToPayAllowed(Card[] cards)
    {
        if (cost == 0)
            return true;
        if (cards == null)
            return false;

        foreach (Card card in cards)
            if (card == this)
            {
                Debug.Log("Trying to pay with own Effect card");
                return false;
            }

        if (cards.Length == cost)
            return true;
        else
        {
            Debug.Log("Incorrect amount of cards wrong=" + cards.Length + " right=" + cost);
            return false;
        }
    }

    public override bool SelectCardsToPay(Card[] cards)
    {
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
            cardsToPay = null;
            return false;
        }
    }

    IEnumerator ApplyingPayment()
    {
        Debug.Log("All Cards Paid");

        if (cardsToPay != null) //null for cards with cost 0
        {
            //pay cards
            foreach (var cardToPay in cardsToPay)
            {
                cardToPay.myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone, 1f);
                if (myPlayer is OpponentPlayer)
                    yield return new WaitForSeconds(0.1f);
            }
        }
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);

        yield return new WaitForSeconds(1);

        for (int i = 0; i < 2; i++)
        {
            myPlayer.DrawTopCardFromMyDeck();
            yield return new WaitForSeconds(0.7f);
        }
        OnActivatingEnd(new Action(instance_id, false, null, CardManager.GetIdByCards(cardsToPay)));
        cardsToPay = null;
        yield return new WaitForSeconds(2f);
    }

    public override void OnActivatingForcedCancel()
    {
        cardsToPay = null;
        myPlayer.myBoard.myHand.GetOrAddComponent<PlayerHandCardEffectPaySelector>().enabled = false;
    }
}

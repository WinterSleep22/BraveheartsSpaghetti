using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Raid : BasicAttack {

    public override IEnumerator OnCardRevealedStartTurnPreAttack()
    {
        if (attackSuffered)
        {
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("The " + title + " is not activated");

            Debug.Log("Raid: attack suffered, move to used cards");

            myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
        }
        else
        {
            yield return new WaitForSeconds(1f);

            Debug.Log("" + title + " Reavelead used as Basic Attack");
            myDrag.MoveToNewDropZone(myPlayer.otherPlayer.myBoard.attackWaitListSlot.myDropZone);
            raidActivated = true;

        }
    }

    bool raidActivated = false;
    bool attackSuffered = false;

    void OnBeingAttacked(CardSlot cardSlot, Card card)
    {
        Debug.Log("" + title + " detected attack");
        attackSuffered = true;
    }

    //raid happens when callback basic defense card is revealed
    IEnumerator OnOtherPlayerRevealFlippedCard(Player player)
    {
        if (player != myPlayer && raidActivated)
        {
            Debug.Log("" + title + ": flipped reveal detect, rading basic defense card of opponent");
            yield return StartCoroutine(ActivateRaidEffect());
        }
    }

    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        base.OnEnterCardSlot(cardSlot);
        if (cardSlot is FlippedCardSlot)
        {
            raidActivated = true;
            attackSuffered = false;
            myPlayer.myBoard.attackWaitListSlot.onCardEnterCallback += OnBeingAttacked;
        }

        if (cardSlot is AttackWaitListSlot)
            GameMechanics.instance.onPlayerRevealingFlippedCardDueStartTurn += OnOtherPlayerRevealFlippedCard;
    }

    public override void OnExitCardSlot(CardSlot cardSlot)
    {
        if (cardSlot is FlippedCardSlot)
        {

            attackSuffered = false;
            myPlayer.myBoard.attackWaitListSlot.onCardEnterCallback -= OnBeingAttacked;
        }

        if (cardSlot is AttackWaitListSlot)
        {
            raidActivated = false;
            GameMechanics.instance.onPlayerRevealingFlippedCardDueStartTurn -= OnOtherPlayerRevealFlippedCard;
        }
    }

    IEnumerator ActivateRaidEffect()
    {
        BasicDefense basicDefense = myPlayer.otherPlayer.myBoard.TryGetCardOnField<BasicDefense>();
        if (basicDefense != null)
        {
            Debug.Log("" + title + " Effect: Break Basic Defense");

            MoveToCenter();

            yield return new WaitForSeconds(0.5f);

            basicDefense.MoveToCenter();

            yield return new WaitForSeconds(1.5f);
            basicDefense.myDrag.MoveToNewDropZone(basicDefense.myPlayer.myBoard.usedCardsSlot.myDropZone);


        }
        else
            Debug.Log("" + title + ": no defense, move to used cards");

        //come back to wait list attack
        myDrag.ComeBackToDropZone();
        yield return new WaitForSeconds(1.5f);



    }

    public override bool IsAllowedToActivate()
    {
        var vanguardNumberOnField = GetNumberOfVanguardsOnField();
        int revealedVanguardNumberOnField = GetNumberOfRevealedVanguardsOnField();
        int totalRaids = myPlayer.otherPlayer.myBoard.attackWaitListSlot.FindCards<Raid>().Count + myPlayer.myBoard.flippedCardSlot.FindCards<Raid>().Count;
        int revealedRaids = GetNumberOfRevealedRaids();

        //  Debug.Log("vanguardNumberOnField=" + vanguardNumberOnField + " revealedVanguardNumberOnField=" + revealedVanguardNumberOnField + " totalRaids=" + totalRaids + " revealedRaids=" + revealedRaids);

        if (totalRaids >= vanguardNumberOnField)
        {
            if (vanguardNumberOnField == 0)
            {
                if (myPlayer is SelfPlayer)
                {
                    LogTextUI.Log("Need a " + CardManager.GetCardOfType<Vanguard>().title + " card");
                }
            }
            else
            {
                if (myPlayer is SelfPlayer)
                {
                    LogTextUI.Log("Need more " + CardManager.GetCardOfType<Vanguard>().title + " card");
                }
            }
            return false;
        }

        if (myCardGraphics.isShowingFront)
        {
            if (revealedRaids >= revealedVanguardNumberOnField)
            {
                if (myPlayer is SelfPlayer)
                {
                    if (vanguardNumberOnField == 0)
                    {
                        LogTextUI.Log("Need a revealed " + CardManager.GetCardOfType<Vanguard>().title + " card");
                    }
                    else
                    {
                        LogTextUI.Log("Need more revealed " + CardManager.GetCardOfType<Vanguard>().title + " card");
                    }
                }

                return false;
            }
        }

        return base.IsAllowedToActivate();
    }

    int GetNumberOfRevealedRaids()
    {
        int result = 0;
        result += myPlayer.otherPlayer.myBoard.attackWaitListSlot.FindCards<Raid>().Count;
        return result;
    }


    int GetNumberOfVanguardsOnField()
    {
        int result = 0;
        if (myPlayer.myBoard.helperCardSlotLeft.FindCards<Vanguard>().Count == 1)
        {
            result++;
        }
        if (myPlayer.myBoard.helperCardSlotMiddle.FindCards<Vanguard>().Count == 1)
        {
            result++;
        }
        if (myPlayer.myBoard.helperCardSlotRight.FindCards<Vanguard>().Count == 1)
        {
            result++;
        }
        return result;
    }

    int GetNumberOfRevealedVanguardsOnField()
    {
        int result = 0;

        if (myPlayer.myBoard.helperCardSlotLeft.FindCards<Vanguard>().Count == 1 && myPlayer.myBoard.helperCardSlotLeft.FindCards<Vanguard>()[0].myCardGraphics.isShowingFrontOnField)
        {
            result++;
        }
        if (myPlayer.myBoard.helperCardSlotMiddle.FindCards<Vanguard>().Count == 1 && myPlayer.myBoard.helperCardSlotMiddle.FindCards<Vanguard>()[0].myCardGraphics.isShowingFrontOnField)
        {
            result++;
        }
        if (myPlayer.myBoard.helperCardSlotRight.FindCards<Vanguard>().Count == 1 && myPlayer.myBoard.helperCardSlotRight.FindCards<Vanguard>()[0].myCardGraphics.isShowingFrontOnField)
        {
            result++;
        }
        return result;
    }


    protected override IEnumerator Activating()
    {

        if (IsAllowedToActivate())
        {
            if (myCardGraphics.isShowingFront)
            {
                //normal attack

                Debug.Log(title + " used as " + CardManager.GetCardOfType<BasicAttack>().title);
                yield return base.Activating();

            }
            else
            {

                Debug.Log("Set Flippable " + title);
                myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
                OnActivatingEnd(new Action(instance_id, true));

            }

        }
        else OnActivatingEnd();
    }
}

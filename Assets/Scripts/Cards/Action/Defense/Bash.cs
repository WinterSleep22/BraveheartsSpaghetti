using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bash : DefenseCard {

    public override bool IsAllowedToActivate()
    {
        if (myPlayer.myBoard.TryGetCardOnField<Vanguard>() == null)
        {
            Debug.Log("Cant Use Bash without Vanguard Helper on Field");
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("Need a " + CardManager.GetCardOfType<Vanguard>().title + " card");
            return false;
        }

        return base.IsAllowedToActivate();
    }

    protected override IEnumerator Activating()
    {
        yield return new WaitForSeconds(0.5f);
        myCardGraphics.ShowBack(true);
        if (IsAllowedToActivate())
        {
            myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
            OnActivatingEnd(new Action(instance_id, true));
        }
        else OnActivatingEnd();
    }


    bool attackSuffered = false;

    public override IEnumerator OnCardRevealedStartTurnPreAttack()
    {
        if (myPlayer.myBoard.attackWaitListSlot.cards.Count > 0)
        {
            //attackSuffered = true;
            myCardGraphics.ShowFront();
        }
        yield return null;
    }

    public override IEnumerator OnCardRevealedStartTurnPosAttack()
    {
        yield return StartCoroutine(ActivatingBashEffect());
    }

    IEnumerator ActivatingBashEffect()
    {
        Vanguard vanguard = myPlayer.myBoard.TryGetCardOnField<Vanguard>();

        if (!attackSuffered)
        {
            if (vanguard != null)
            {
                Debug.Log("Bash Effect: transforming into Attack");
                BasicAttack b = Instantiate(CardManager.GetCardPrefabByType<BasicAttack>(), this.transform.position, Quaternion.identity, myPlayer.myBoard.transform);
                myCardGraphics.FadeAlpha(0, 0, 0);
                b.myCardGraphics.ShowFront();
                yield return new WaitForSeconds(1);

                b.myDrag.MoveToNewDropZone(GameMechanics.instance.turnManager.lastPlayer.myBoard.attackWaitListSlot.myDropZone, 2f);
            }
            else
            {
                LogTextUI.Log("The " + CardManager.GetCardOfType<Bash>().title + " is not activated");
                myCardGraphics.ShowFront(true);
                Debug.Log("Bash Effect: no vanguard on field");
            }
        }
        else
        {
            LogTextUI.Log("The " + CardManager.GetCardOfType<Bash>().title + " is not activated");
            //myCardGraphics.ShowFront(true);
            Debug.Log("Bash Effect Not activated, player suffer attacks in the last turn");
        }
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);

    }
}

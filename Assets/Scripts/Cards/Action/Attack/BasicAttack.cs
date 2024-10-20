using UnityEngine;
using System.Collections;

public class BasicAttack : AttackCard
{
    /// <summary>
    /// detect if watch tower and turn on selection for attack
    /// </summary>
    /// <param name="cardSlot"></param>
    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        if (cardSlot is AttackWaitListSlot)
        {
            if (myPlayer.otherPlayer.myBoard.TryGetCardOnField<WatchTower>() != null)
            {
                if (myPlayer is SelfPlayer)
                {
                    Debug.Log("Watch Tower Detected, turn on card slot for attack selector");
                    myPlayer.myBoard.myHand.GetComponent<CardSlotSelectorForAttack>().enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// if showing front check supply on field,check number helper cards,check effect full scale 
    /// then use inherent behaviour 
    /// </summary>
    /// <param name="printLog"></param>
    /// <returns></returns>

    public override bool IsAllowedToActivate()
    {
        if (myCardGraphics.isShowingFront)
        {
            bool isThereSupplyOnField = myPlayer.myBoard.TryGetCardOnField<Supply>() != null;
            if (isThereSupplyOnField)
            {
                Debug.Log("Cant Attack, Supply On Field ");
                return false;
            }

            int currentAttackNumber = myPlayer.otherPlayer.myBoard.attackWaitListSlot.cards.Count;

            int personNumber = GetPersonCardNumberOnField();
            //  Debug.Log(" BasicAttack helper card revealed=" + personNumber);
            if (currentAttackNumber >= personNumber && !myPlayer.status.effectFullScaleAttack)
            {
                Debug.Log("Max of attack reached " + personNumber + " persons on field");
                return false;
            }

            if (myPlayer.status.effectFullScaleAttack && currentAttackNumber >= 4) //fullscaleattack is exception
            {
                Debug.Log("Full scale effect allows 4 attack cards");
                return false;
            }
        }

        return base.IsAllowedToActivate();

    }

    protected override IEnumerator Activating()
    {
        if (IsAllowedToActivate())
        {
            MoveToCenter(0.5f, 0);

            yield return new WaitForSeconds(0.5f);
            myDrag.MoveToNewDropZone(myPlayer.otherPlayer.myBoard.attackWaitListSlot.myDropZone, 2f);
            OnActivatingEnd(new Action(instance_id, false));

        }
        else
        {
            OnActivatingEnd();
        }
    }

    public int GetPersonCardNumberOnField()
    {
        int result = 0;
        if (myPlayer.myBoard.heroCardSlot.cards.Count == 1 && myPlayer.myBoard.heroCardSlot.cards[0].myCardGraphics.isShowingFrontOnField)
        {
            result++;
        }
        if (myPlayer.myBoard.helperCardSlotLeft.cards.Count == 1 && myPlayer.myBoard.helperCardSlotLeft.cards[0].myCardGraphics.isShowingFrontOnField)
        {
            result++;
        }
        if (myPlayer.myBoard.helperCardSlotMiddle.cards.Count == 1 && myPlayer.myBoard.helperCardSlotMiddle.cards[0].myCardGraphics.isShowingFrontOnField)
        {
            result++;
        }
        if (myPlayer.myBoard.helperCardSlotRight.cards.Count == 1 && myPlayer.myBoard.helperCardSlotRight.cards[0].myCardGraphics.isShowingFrontOnField)
        {
            result++;
        }
        return result;
    }


}

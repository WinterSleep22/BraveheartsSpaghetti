using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : BasicAttack {

    public override bool IsAllowedToActivate()
    {
        if (myPlayer.otherPlayer.myBoard.attackWaitListSlot.FindCardOfType<FullScaleAttack>())
        {
            LogTextUI.Log("Can't use Charge with Full Scale Attack");
            return false;
        }
        return base.IsAllowedToActivate();
    }

    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        base.OnEnterCardSlot(cardSlot);

        if(cardSlot is StackAttackSlot)
        {
            myPlayer.DrawTopCardFromMyDeck();
        }
    }


}

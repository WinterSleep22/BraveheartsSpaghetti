using UnityEngine;
using System.Collections;

public class DamageAnalysisLab : NormalBuildingCard
{
    public void OnBeingAttacked()
    {
        myPlayer.ActivateEffectOnStack(() => {
            if (myPlayer is SelfPlayer)
                LogTextUI.Log( title+": +1 Card");
            myPlayer.DrawTopCardFromMyDeck();
        
        });
        
    }

    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.myBoard.stackAttackSlot.onAttackedPlayerCallback += OnBeingAttacked;
        }
    }


    public override void OnExitCardSlot(CardSlot cardSlot)
    {
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.myBoard.stackAttackSlot.onAttackedPlayerCallback -= OnBeingAttacked;
        }
    }
   
}

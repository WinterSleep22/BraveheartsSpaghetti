using UnityEngine;
using System.Collections;

public class StrategyAnalysisLab : NormalBuildingCard
{
    public void OnOpponentBeingAttacked()
    {
        myPlayer.ActivateEffectOnStack(()=>{
             if(myPlayer is SelfPlayer)
                LogTextUI.Log(title+" +1 Card");
             myPlayer.DrawTopCardFromMyDeck();
        });
       
    }

    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.otherPlayer.myBoard.stackAttackSlot.onAttackedPlayerCallback += OnOpponentBeingAttacked;
        }
    }


    public override void OnExitCardSlot(CardSlot cardSlot)
    {
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.otherPlayer.myBoard.stackAttackSlot.onAttackedPlayerCallback -= OnOpponentBeingAttacked;
        }
    }

}

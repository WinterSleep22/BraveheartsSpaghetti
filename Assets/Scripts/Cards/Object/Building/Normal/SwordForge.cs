using UnityEngine;
using System.Collections;

public class SwordForge : NormalBuildingCard
{
    void OnAttackOpponent()
    {
        myPlayer.ActivateEffectOnStack(() =>
        {
            if (myPlayer is SelfPlayer)
                LogTextUI.Log(  title+": Draw +1 Card");
            myPlayer.DrawTopCardFromMyDeck();
        });
        
      
    }
    
    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        base.OnEnterCardSlot(cardSlot);
        if(cardSlot is NormalBuildingSlot){
            myPlayer.otherPlayer.myBoard.attackWaitListSlot.onAttackPlayerDirectly += OnAttackOpponent;
            myPlayer.otherPlayer.myBoard.attackWaitListSlot.onAttackBuilding += OnAttackOpponent;
        }
    }
    public override void OnExitCardSlot(CardSlot cardSlot)
    {
        base.OnExitCardSlot(cardSlot);
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.otherPlayer.myBoard.attackWaitListSlot.onAttackPlayerDirectly -= OnAttackOpponent;
            myPlayer.otherPlayer.myBoard.attackWaitListSlot.onAttackBuilding -= OnAttackOpponent;
        }
    }
}

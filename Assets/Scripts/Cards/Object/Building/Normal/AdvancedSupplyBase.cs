using UnityEngine;
using System.Collections;

public class AdvancedSupplyBase : NormalBuildingCard
{


    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        if(cardSlot is NormalBuildingSlot){
            GameMechanics.instance.onPlayerDrawCardDueStartTurn += OnPlayerDrawACardDueStartTurn;
        }
    }
    public override void OnExitCardSlot(CardSlot cardSlot)
    {
        if (cardSlot is NormalBuildingSlot)
        {
            GameMechanics.instance.onPlayerDrawCardDueStartTurn -= OnPlayerDrawACardDueStartTurn;
        }
    }

    void OnPlayerDrawACardDueStartTurn(Player player)
    {
        if(player == myPlayer){
            myPlayer.ActivateEffectOnStack(() =>
            {
                if (myPlayer is SelfPlayer)
                    LogTextUI.Log(title+": Draw +1 Card due draw on start turn");
                myPlayer.DrawTopCardFromMyDeck();
            });
        }
    }

  
 

}

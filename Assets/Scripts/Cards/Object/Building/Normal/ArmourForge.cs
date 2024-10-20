using UnityEngine;
using System.Collections;

public class ArmourForge : NormalBuildingCard
{

    public void OnPlayerSucessDefend(Player player)
    {
       
        myPlayer.ActivateEffectOnStack(() => {

            if (myPlayer is SelfPlayer)
                LogTextUI.Log(title+": Draw +1 Card");
            myPlayer.DrawTopCardFromMyDeck();
        });

       
    }
  

    void OnCardEnterFlippedSlot(CardSlot cardslot,Card card)
    {
        if(card is CounterAttack){
            CounterAttack counterAttackCard =  card as CounterAttack;
            counterAttackCard.onCounterAttackSucessfulyActivated += OnPlayerSucessDefend;
        }
    }
    void OnCardExitFlippedSlot(CardSlot cardslot, Card card)
    {
        if (card is CounterAttack)
        {
            CounterAttack counterAttackCard = card as CounterAttack;
            counterAttackCard.onCounterAttackSucessfulyActivated -= OnPlayerSucessDefend;
        }
    }
    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        base.OnEnterCardSlot(cardSlot);
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.myBoard.attackWaitListSlot.onPlayerSucessfullyDefend += OnPlayerSucessDefend;
            myPlayer.myBoard.flippedCardSlot.onCardEnterCallback += OnCardEnterFlippedSlot;
            myPlayer.myBoard.flippedCardSlot.onCardExitCallback += OnCardExitFlippedSlot;
        }
    }
    public override void OnExitCardSlot(CardSlot cardSlot)
    {
        base.OnExitCardSlot(cardSlot);
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.myBoard.attackWaitListSlot.onPlayerSucessfullyDefend -= OnPlayerSucessDefend;
            myPlayer.myBoard.flippedCardSlot.onCardEnterCallback -= OnCardEnterFlippedSlot;
            myPlayer.myBoard.flippedCardSlot.onCardExitCallback -= OnCardExitFlippedSlot;
        }
    }
   

}

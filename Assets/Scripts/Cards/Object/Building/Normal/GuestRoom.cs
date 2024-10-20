using UnityEngine;
using System.Collections;

/// <summary>
/// Draw one card on helper card is set on field
/// </summary>
public class GuestRoom : NormalBuildingCard
{


    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        base.OnEnterCardSlot(cardSlot);
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.myBoard.helperCardSlotLeft.onCardEnterCallback += OnPersonCardSetOnField;
            myPlayer.myBoard.helperCardSlotMiddle.onCardEnterCallback += OnPersonCardSetOnField;
            myPlayer.myBoard.helperCardSlotRight.onCardEnterCallback += OnPersonCardSetOnField;
        }
       
    }

    public override void OnExitCardSlot(CardSlot cardSlot)
    {
        base.OnExitCardSlot(cardSlot);
        if (cardSlot is NormalBuildingSlot)
        {
            myPlayer.myBoard.helperCardSlotLeft.onCardEnterCallback -= OnPersonCardSetOnField;
            myPlayer.myBoard.helperCardSlotMiddle.onCardEnterCallback -= OnPersonCardSetOnField;
            myPlayer.myBoard.helperCardSlotRight.onCardEnterCallback -= OnPersonCardSetOnField;
        }
    }

    public void OnPersonCardSetOnField(CardSlot cardSlot , Card card)
    {
        myPlayer.ActivateEffectOnStack(() => {
            if (myPlayer is SelfPlayer)
                LogTextUI.Log(title+" Effect: Draw +1 Card");
            myPlayer.DrawTopCardFromMyDeck();
        });
        
    }
}

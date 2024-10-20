using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// provides basic input function callbacks to select specific cardslots on field
/// </summary>
public class CardSlotSelectorForAttack : MonoBehaviour
{

   
    void OnEnable()
    {
        GameMechanics.instance.opponentPlayer.myBoard.specialBuildingCardSlotLeft.onClickCardSlotCallback += OnClickedCardSlot;
        GameMechanics.instance.opponentPlayer.myBoard.specialBuildingCardSlotRight.onClickCardSlotCallback += OnClickedCardSlot;
        FindObjectOfType<BackgroundBoard>().onClickedBackground += OnClickedOut;
        GameMechanics.instance.GetOpponentPlayer.SetBlockOpponentInput(false);
    }
    
    void OnDisable()
    {
        GameMechanics.instance.opponentPlayer.myBoard.specialBuildingCardSlotLeft.onClickCardSlotCallback -= OnClickedCardSlot;
        GameMechanics.instance.opponentPlayer.myBoard.specialBuildingCardSlotRight.onClickCardSlotCallback -= OnClickedCardSlot;
        GameMechanics.instance.turnManager.onEndTurn += (p) => { if (p is SelfPlayer) this.enabled = false; };
        FindObjectOfType<BackgroundBoard>().onClickedBackground -= OnClickedOut;
        GameMechanics.instance.GetOpponentPlayer.SetBlockOpponentInput(true);
    }

    void OnClickedCardSlot(CardSlot slot)
    {
        if(slot.cards.Count==0){
            Debug.Log("choosed empty slot");
            return;
        }
        if(!(slot.cards[0] is WatchTower)){
            Debug.Log("Not a WatchTower");
            return;
        }
        slot.ShowGraphics();
        SelectCard(slot.cards[0] as WatchTower);

    }

    void OnClickedOut()
    {
        
        Cancel();
    }

    void SelectCard(WatchTower choosedWatchTower)
    {
        if(choosedWatchTower==null){
            Debug.Log("Selected null watch tower to attack");
            return;
        }
        
        //Togle attack
        Gameplay.instance.selfPlayerNetwork.ToggleWachTowerToAttackNetwork( rightOrLeft : choosedWatchTower.cardSlot is SpecialBuildingSlot2);

    }

    public void Cancel()
    {
       // this.enabled = false;
    }
}

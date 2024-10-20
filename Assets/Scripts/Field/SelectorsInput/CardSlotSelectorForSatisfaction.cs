using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// provides basic input function callbacks to select specific cardslots on field
/// </summary>
public class CardSlotSelectorForSatisfaction : MonoBehaviour
{
    public Satisfaction satisfactionCard;
    public CardSlot[] cardslots;
    public void InitializeHelperCardSelection(Satisfaction _satisfactionCard, CardSlot[] _cardslots)
    {
        satisfactionCard = _satisfactionCard;
        cardslots = _cardslots;

        this.enabled = true;
        
      
    }

    void AssignCallbacks()
    {
        FindObjectOfType<BackgroundBoard>().onClickedBackground += OnClickedOut;
        foreach(var cardslot in cardslots){
            cardslot.onClickCardSlotCallback += OnClickedCardSlot;
        }
    }

    void DesAssignCallbacks()
    {
        FindObjectOfType<BackgroundBoard>().onClickedBackground -= OnClickedOut;
        foreach (var cardslot in cardslots)
        {
            cardslot.onClickCardSlotCallback -= OnClickedCardSlot;
        }
    }
    
    public void OnEnable()
    {
        AssignCallbacks();
        GetComponent<PlayerHandCardSelector>().enabled = false;

        GameMechanics.instance.GetOpponentPlayer.SetBlockOpponentInput(false);
        GameMechanics.instance.GetSelfPlayer.SetBlockAllInput(false);
        GameMechanics.instance.GetSelfPlayer.SetBlockSelfInput(true);
    }
    public void OnDisable()
    {
        DesAssignCallbacks();
        GetComponent<PlayerHandCardSelector>().enabled = true;

        GameMechanics.instance.GetOpponentPlayer.SetBlockOpponentInput(true);
    }

    void OnClickedCardSlot(CardSlot slot)
    {
        if (slot.myPlayer == satisfactionCard.myPlayer)
        {
            LogTextUI.Log("Choose an opponent Slot");
            return;
        }
        if(slot.cards.Count==0){
            LogTextUI.Log("choosed empty slot");
            return;
        }
        if(!(slot.cards[0] is HelperCard)){
            LogTextUI.Log("Choose a Helper Card");
            return;
        }
       
        SelectCard(slot.cards[0] as HelperCard);

    }

    void OnClickedOut()
    {
        
        Cancel();
    }

    void SelectCard(HelperCard choosedHelperCard)
    {

        Debug.Log("Selected " + choosedHelperCard.title);
        satisfactionCard.OnChooseHelperCard(choosedHelperCard);
        this.enabled = false;
        GetComponent<PlayerHandCardSelector>().enabled = true;
    }

    void Cancel()
    {
        Debug.Log("Cancel Satisfaction Choosing State, back to hand");
        satisfactionCard.CancelActivating();
        satisfactionCard.myDrag.ComeBackToDropZone();
        this.enabled = false;
       
    }
}

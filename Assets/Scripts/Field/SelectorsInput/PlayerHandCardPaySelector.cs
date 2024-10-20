using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerHandCardPaySelector : HandCardSelector
{
    public List<Card> selectedCards= new List<Card>();
 
    BuildingCard myBuildingCard;

    public void Initialize(BuildingCard card )
    {
        myBuildingCard = card;
        PlayerHandCardSelector normalCardSelection = GetComponent<PlayerHandCardSelector>();
        normalCardSelection.enabled = false;
        selectedCards.Clear();
        myBuildingCard.MoveToCenter();
    }


    public override void OnClickedOut()
    {
        Debug.Log("Canceling Pay Building");
        myBuildingCard.CancelActivating();
        myBuildingCard.myDrag.ComeBackToDropZone();
        foreach(var selectedCard in selectedCards){
            selectedCard.myDrag.ComeBackToDropZone();
        }
        selectedCards.Clear();
        enabled = false;
        PlayerHandCardSelector normalCardSelection = GetComponent<PlayerHandCardSelector>();
        normalCardSelection.enabled = true;
    }

    public override void OnCardClicked(Card card)
    {
        if (selectedCards.Contains(card))
        {
            RevertSelection(card);
        }
        else
        {
            SelectCard(card);
        }
    }

    void RevertSelection(Card card)
    {        
        card.myDrag.ComeBackToDropZone();
        selectedCards.Remove(card);
        Debug.Log("Card Deselected to pay " + card.title);
    }


    public void SelectCard(Card card)
    {

        if(card == myBuildingCard){
            Debug.Log("you can't pay the building card");
            return;
        }

        //highlight the card
      

        iTween.MoveTo(card.gameObject, card.transform.position + Vector3.up * card.myRectTransform.GetHeight() * 0.5f, 2f);
        iTween.RotateTo(card.gameObject, Vector3.zero, 1f);
        card.transform.SetAsLastSibling();
        card.myCardGraphics.FadeAlpha(1, 0.5f, 1f);

        selectedCards.Add(card);
        Debug.Log("Card Selected to pay " + card.title);

        if( selectedCards.Count == myBuildingCard.cost ){
            myBuildingCard.SelectCardsToPay(selectedCards.ToArray());
            //finish Selection
            enabled = false;
            PlayerHandCardSelector normalCardSelection = GetComponent<PlayerHandCardSelector>();
            normalCardSelection.enabled = true;
        }

    }
  

   
}

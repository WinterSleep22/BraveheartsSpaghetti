using UnityEngine;
using System.Collections;

public class DeckManager : MonoBehaviour {

    public Board selfBoard;
    public Board opponentBoard;
    public DeckGauge opponentUsedDeckGauge;
    public DeckGauge opponentDeckGauge;
    public DeckGauge selfDeckGauge;
    public DeckGauge selfUsedDeckGauge;
    void Awake()
    {
       
        opponentUsedDeckGauge.SetValue(0);
        opponentDeckGauge.SetValue(0);
        selfDeckGauge.SetValue(0);
        selfUsedDeckGauge.SetValue(0);
    }
    void Start()
    {
        selfBoard.myDeck.onChangeCardNumber += OnSelfDeckChange;
        selfBoard.usedCardsSlot.onChangeCardNumber += OnSelfUsedCardsChange;
        opponentBoard.usedCardsSlot.onChangeCardNumber += OnOpponentUsedCardsChange;
        opponentBoard.myDeck.onChangeCardNumber += OnOpponentDeckChange;
    }
    void OnSelfUsedCardsChange(CardSlot cardSlot)
    {
        float value = (float)cardSlot.cards.Count / (float)cardSlot.myDropZone.max;
        selfUsedDeckGauge.SetValue(value);
    }

    void OnOpponentUsedCardsChange(CardSlot cardSlot)
    {
        float value = (float)cardSlot.cards.Count / (float)cardSlot.myDropZone.max;
        opponentUsedDeckGauge.SetValue(value);
    }

    void OnSelfDeckChange(CardSlot cardSlot)
    {
        float value = (float)cardSlot.cards.Count / (float)cardSlot.myDropZone.max;
        selfDeckGauge.SetValue(value);
    }

    void OnOpponentDeckChange(CardSlot cardSlot)
    {

        float value = (float)cardSlot.cards.Count / (float)cardSlot.myDropZone.max;
        opponentDeckGauge.SetValue( value);
    }



}

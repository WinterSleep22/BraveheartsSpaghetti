using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class CardsManagerUI : MonoBehaviour
{

    public float cardSizeMultiplierWhileSelected = 1.1f;
    public GridLayoutGroup cardsUI;

    public GameObject prefabCardUI;

    public Image backgroundForSelection;

    [HideInInspector]
    public CardUI currentSelectedCard = null;

    [HideInInspector]
    public List<CardUI> listCards = new List<CardUI>();


    MyDeckMenu _myDeckMenu;
    public MyDeckMenu myDeckMenu
    {
        get
        {
            if (_myDeckMenu == null)
            {
                _myDeckMenu = FindObjectOfType<MyDeckMenu>();
            }
            return _myDeckMenu;
        }
    }

    void Start()
    {
        UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.callback.AddListener((data) => DeselectCurrentCard());
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
        backgroundForSelection.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry);
    }

    void OnDisable()
    {
        backgroundForSelection.enabled = false;
    }


    public void CreateCardsUI()
    {
        cardsUI.transform.DestroyAllChildren();
        listCards.Clear();
        var sortedListCards = FindObjectOfType<MyDeckMenu>().SortCards(CardManager.instance.cardTypes);
        sortedListCards.Reverse();
        foreach (var cardType in sortedListCards)
        {
            CardUI newCardUI = CreateCardUI(cardType.title);
            listCards.Add(newCardUI);
        }
    }

    CardUI CreateCardUI(string title)
    {
        var newCardUI = GameObject.Instantiate(prefabCardUI).GetComponentInChildren<CardUI>();
        newCardUI.transform.parent.SetParent(cardsUI.transform);
        newCardUI.transform.parent.localScale = Vector3.one;
        newCardUI.myCardTitle = title;
        return newCardUI;
    }

    void RemoveCardUI(string title)
    {
        var cardUI = listCards.Find(t => t.myCardTitle == title);
        if (cardUI == null)
        {
            Debug.Log("remove card failed title=" + title);
            return;
        }
        listCards.Remove(cardUI);
        Destroy(cardUI.transform.parent.gameObject);
    }

    public void UpdateCardUI(string title, float frequency)
    {

        var cardUI = listCards.Find(t => t.myCardTitle == title);


        var cardType = CardManager.GetCardPrefabByTitle(title);
        //Debug.Log ("UpdateCardUI cardType=" + cardType.title );
        cardUI.myCardTitle = cardType.title;
        cardUI.cardImage.sprite = cardType.front;
        //card is in the deck
        if (frequency != 0)
        {
            cardUI.counterText.text = frequency.ToString() + " / " + ((cardType.limitCopiesInUse == -1) ? "-" : PersistentCardGame.GetCopiesInTotal(cardType).ToString());
        }
        else
        {
            if (cardType.limitCopiesInUse != -1)
            {
                cardUI.counterText.text = "x" + PersistentCardGame.GetCopiesInTotal(cardType).ToString();
            }
            else
            {
                cardUI.counterText.text = "-";
            }
        }

    }

    public void UpdateCardsUI()
    {

        foreach (var cardType in CardManager.instance.cardTypes)
        {
            UpdateCardUI(cardType.title, MenuManager.instance.GetComponentInChildren<MyDeckMenu>().GetFrequency(cardType.title));

        }


    }

    public bool IsCardSelected { get { return currentSelectedCard != null; } }


    public void OnClickInsideCard(CardUI cardUI, bool opendedByDeck = false)
    {

        Debug.Log("Card clicked " + cardUI.myCardTitle);
        if (myDeckMenu.IsButton1Pressed)
        {
            if (opendedByDeck)
            {
                bool sucess = myDeckMenu.RemoveCardFromMyDeck(cardUI.myCardTitle);
                if (sucess)
                {
                    Debug.Log("Card removed from deck sucess title= " + cardUI.myCardTitle);
                    cardUI.transform.ExecuteSelectAnimation(multiplier: 1f / cardSizeMultiplierWhileSelected);
                    SoundManagerMenu.instance.BackSound();




                }
                else
                {
                    Debug.Log("Card removed from deck failed title= " + cardUI.myCardTitle);
                    SoundManagerMenu.instance.incorrectSound.Play();

                }
            }
            else
            {
                bool sucess = myDeckMenu.AddCardToMyDeck(cardUI.myCardTitle);
                if (sucess)
                {
                    cardUI.transform.ExecuteSelectAnimation(cardSizeMultiplierWhileSelected);
                    SoundManagerMenu.instance.GoSound();

                }
                else
                {
                    SoundManagerMenu.instance.incorrectSound.Play();
                }

            }
        }
        else
        {
            if (IsCardSelected)
            {
                if (opendedByDeck)
                {
                    bool sucess = myDeckMenu.RemoveCardFromMyDeck(cardUI.myCardTitle);
                    if (sucess)
                    {

                        SoundManager.instance.cardDeselect.Play();

                        if (myDeckMenu.deck.FindAll(c => c.title == cardUI.myCardTitle).Count == 0)
                        {

                            DeselectCurrentCard();
                        }
                        else
                        {
                            cardUI.transform.ExecuteSelectAnimation(multiplier: 1f / cardSizeMultiplierWhileSelected);
                        }

                    }
                    else
                    {
                        SoundManagerMenu.instance.incorrectSound.Play();
                    }

                }
                else
                {
                    bool sucess = myDeckMenu.AddCardToMyDeck(cardUI.myCardTitle);
                    if (sucess)
                    {
                        cardUI.transform.ExecuteSelectAnimation(cardSizeMultiplierWhileSelected);
                        SoundManagerMenu.instance.GoSound();
                        /*
						if( listCards.FindAll(c=>c.myCardTitle == cardUI.myCardTitle).Count== CardManager.GetCardPrefabByTitle(cardUI.myCardTitle).limitCopiesPerDeck){
							DeselectCurrentCard ();
						}
						*/
                    }
                    else
                    {
                        SoundManagerMenu.instance.incorrectSound.Play();
                    }

                }

            }
            else
            {
                //no card current selected

                if (!opendedByDeck && myDeckMenu.deck.FindAll(c => c.title == cardUI.myCardTitle).Count == PersistentCardGame.GetCopiesInTotal(CardManager.GetCardPrefabByTitle(cardUI.myCardTitle)))
                {
                    Debug.Log("Can not select this card to be added, already got max copies per deck");
                    SoundManagerMenu.instance.incorrectSound.Play();

                }
                else
                {
                    Debug.Log("Card zoom in title= " + cardUI.myCardTitle);
                    cardUI.ZoomIn();
                    SoundManager.instance.cardSelect.Play();
                }


            }
        }

    }

    public void DeselectCurrentCard()
    {
        if (currentSelectedCard != null)
            currentSelectedCard.ZoomOut();
        currentSelectedCard = null;
        backgroundForSelection.enabled = false;
        SoundManager.instance.cardDeselect.Play();
    }
}

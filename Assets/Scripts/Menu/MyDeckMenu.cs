using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class DeckAux
{

    public List<string> deck = new List<string>();
    public DeckAux(List<string> d)
    {
        deck = d;
    }
}

public class MyDeckMenu : Menu
{


    static MyDeckMenu _instance;
    public static MyDeckMenu instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MyDeckMenu>();
            }
            return _instance;
        }
    }

    public ThumbsManagerUI thumbsManagerUI;
    public CardsManagerUI cardsManagerUI;



    //HideInInspector]
    public List<Card> deck = new List<Card>();


    public GameObject soundButton;

    public Button button1;
    public Button okButton;

    public Color button1PressedColor = new Color(148f / 256f, 148f / 256f, 148f / 256f);

    bool button1IsPressed = false;

    public bool IsButton1Pressed
    {
        get
        {
            return button1IsPressed;
        }
    }

    void Start()
    {
        okButton.onClick.AddListener(Ok);
        button1.onClick.AddListener(OnClickButton1);
    }

    void OnEnable()
    {

        soundButton.gameObject.SetActive(false);

        deck = CardManager.LoadDeck();
        deck = SortCards(deck);
        RemadeAllUI();
    }


    void OnDisable()
    {

        soundButton.gameObject.SetActive(true);

    }


    public void Ok()
    {

        CardManager.SaveDeckToPlayerPrefs(deck);

        //TODO: show feedback that the save went well: show something for some seconds
        Hide();
        transform.parent.GetComponentInChildren<MyInfoPanelMenu>(true).Show();

    }

    public void OnClickButton1()
    {
        button1IsPressed = !button1IsPressed;

        button1.image.color = (button1IsPressed) ? button1PressedColor : Color.white;

        iTween.ScaleTo(button1.gameObject, Vector3.one * ((button1IsPressed) ? 1.2f : 1f), 0.2f);

        //Debug.Log ("OnClickButton1 color=" + button1.image.color);
        //Hide();
        //transform.parent.GetComponentInChildren<MyInfoPanelMenu>(true).Show();
    }


    public void RemadeAllUI()
    {
        thumbsManagerUI.CreateDeckUI();
        cardsManagerUI.CreateCardsUI();

        thumbsManagerUI.UpdateDeckUI();
        cardsManagerUI.UpdateCardsUI();
    }

    public List<Card> SortCards(List<Card> cards)
    {
        List<Card> newList = new List<Card>();

        newList.AddRange(cards.FindAll(card => card is EffectCard));
        newList.AddRange(cards.FindAll(card => card is DefenseCard));
        newList.AddRange(cards.FindAll(card => card is AttackCard));
        newList.AddRange(cards.FindAll(card => card is SpecialBuildingCard));
        newList.AddRange(cards.FindAll(card => card is NormalBuildingCard));
        newList.AddRange(cards.FindAll(card => card is HelperCard));
        newList.AddRange(cards.FindAll(card => card is HeroCard));


        if (newList.Count != cards.Count)
        {
            Debug.Log("___ sort wrong count newlist=" + newList.Count + " cards=" + cards.Count);
        }

        return newList;
    }
    
    public bool AddCardToMyDeck(string title)
    {
        var prefabCard = CardManager.GetCardPrefabByTitle(title);
        if (prefabCard == null)
        {
            Debug.Log("___ prefab card is nul, can't remove it title=" + title);
            return false;
        }

        if (deck.Count >= 41)
        {
            Debug.Log("Deck is full");
            return false;
        }
        int frequency = GetFrequency(title);
        if (frequency == prefabCard.limitCopiesInUse)
        {
            if (prefabCard.title == "General" || prefabCard.title == "Attack" || prefabCard.title == "Defense")
            {

            }
            else
            {
                Debug.Log("limitCopiesPerDeck");
                return false;
            }
        }
        if (frequency == PersistentCardGame.GetCopiesInTotal(prefabCard))
        {
            Debug.Log("Got max limit per deck=" + PersistentCardGame.GetCopiesInTotal(prefabCard) + " title=" + title);
            return false;
        }

        Debug.Log("AddCardToMyDeck card=" + prefabCard.title + " to deck");

        deck.Add(prefabCard);
        thumbsManagerUI.UpdateThumbUI(title, GetFrequency(title));
        cardsManagerUI.UpdateCardUI(title, GetFrequency(title));
        return true;
    }

    public int GetFrequency(string title)
    {
        var dict = GetFrequencyCardDeck();
        return (dict.ContainsKey(title)) ? dict[title] : 0;
    }

    public bool RemoveCardFromMyDeck(string title)
    {
        var cardToBeRemoved = deck.Find(card => card.title == title);

        if (cardToBeRemoved == null)
        {
            Debug.Log("Cant remove it, not in the deck: card=" + title + " ");
            return false;
        }

        if (cardToBeRemoved is General)
        {
            Debug.Log("Can't remove General card");
            return false;
        }
        Debug.Log("Removing card=" + cardToBeRemoved.title + " from deck");

        bool removed = deck.Remove(cardToBeRemoved);

        if (removed)
        {


            thumbsManagerUI.UpdateThumbUI(title, GetFrequency(title));
            cardsManagerUI.UpdateCardUI(title, GetFrequency(title));
            return true;
        }
        else
        {
            Debug.Log("failed to remove title=" + title);
            return false;
        }
    }


    public Dictionary<string, int> GetFrequencyCardDeck()
    {

        //update card UI according to modifications
        Dictionary<string, int> frequencyCardDeck = new Dictionary<string, int>();

        foreach (var card in deck)
        {
            if (frequencyCardDeck.ContainsKey(card.title))
            {
                frequencyCardDeck[card.title]++;
            }
            else
            {
                frequencyCardDeck.Add(card.title, 1);
            }
        }
        return frequencyCardDeck;
    }


}

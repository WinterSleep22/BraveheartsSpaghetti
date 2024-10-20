using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Reflection;
public class CardManager : MonoBehaviour {
    private static CardManager _instance;
    public static CardManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CardManager>();
            }
            return _instance;
        }
    }

    [HideInInspector]
    public List<Card> cardsGenerated = new List<Card>();

    public static T GetCardOfType<T>() where T : Card
    {
        return instance.cardsGenerated.Find(c => c is T) as T;
    }

    GameObject[] _cardsPrefab;
    GameObject[] cardsPrefab
    {
        get
        {
            if (_cardsPrefab == null)
            {
                _cardsPrefab = Resources.LoadAll<GameObject>("Prefabs/Cards/Resources");

            }
            return _cardsPrefab;
        }
    }

    List<Card> _cardTypes = null;
    public List<Card> cardTypes
    {
        get
        {
            if (_cardTypes == null)
            {
                List<Card> cards = new List<Card>();
                foreach (var cardPrefab in cardsPrefab)
                {

                    Card newCard = cardPrefab.GetComponent<Card>();
                    cards.Add(newCard);
                }
                _cardTypes = cards;
            }
            return _cardTypes;
        }
    }

    public static T GetCardPrefabByType<T>() where T : Card
    {
        return instance.cardTypes.Find(c => c is T) as T;
    }
    public static Card GetCardPrefabByTitle(string title)
    {
        return instance.cardTypes.Find(c => c.title == title);
    }

    static private Sprite _back;
    public static Sprite back
    {
        get
        {
            if (_back == null)
            {
                _back = Resources.Load<Sprite>("back");
            }
            return _back;
        }
    }

    void InstantiateCard(string titleCardToGenerate)
    {
        if (WifiNetworkManager.instance == null)
        {
            PhotonNetwork.Instantiate(GetCardPrefabByTitle(titleCardToGenerate).gameObject.name, Vector3.zero, Quaternion.identity, 0);
        }
        else
        {

            Gameplay.instance.selfPlayerNetwork.CmdSpawnCard(titleCardToGenerate);
        }

    }

    /// <summary>
    /// randomly calls network instantiation for generate cards
    /// </summary>
	public System.Collections.IEnumerator GeneratingCards(bool loadDefault = false)
    {
        List<string> cardsToGenerate = new List<string>();
        List<Card> myDeck;
        if (loadDefault)
        {
            myDeck = LoadDefaultDeck();
            Debug.Log("Load Default Deck for local Opponent");
        }
        else
        {
            myDeck = LoadDeck();
        }
        foreach (var card in myDeck)
        {
            cardsToGenerate.Add(card.title);
        }

        yield return StartCoroutine(InstantiatingCards(cardsToGenerate));



    }

    System.Collections.IEnumerator InstantiatingCards(List<string> cardsToGenerate)
    {
        // object pooling would do good here
        foreach (var cardType in cardsToGenerate)
        {
            InstantiateCard(cardType);
            yield return new WaitForSeconds(0.1f);
        }

    }

    public System.Collections.IEnumerator WaitingPlayerGet41Cards(Player player, float timing = 0.5f)
    {
        Card[] cards;
        while (true)
        {
            yield return new WaitForSeconds(timing);

            cards = CardManager.GetMyCards(player);

            if (cards == null)
                continue;

            Debug.Log("AdjustCardsToDeckPlayer: Waiting Cards to be instantiated for player=" + player.gameObject.name + " mycards=" + cards.Length);
            if (cards.Length == 41)
            {
                break;
            }

        }
    }

    public System.Collections.IEnumerator AdjustingCardsToDeckPlayer(Player player, int[] cardsID)
    {
        Debug.Log("GAmeMechanics AdjustCardsToDeckPlayer player=" + player.gameObject.name);

        yield return StartCoroutine(WaitingPlayerGet41Cards(player));

        Card[] cards = GetCardsById(cardsID);

        foreach (var card in cards)
        {
            card.transform.SetParent(player.myBoard.myDeck.transform);
            card.transform.localPosition = Vector3.zero;
        }



        player.myBoard.myDeck.myDropZone.DetectDragChilds();//add to drop logic and then slot logics
    }


    public static int[] GetIdByCards(Card[] cards)
    {
        if (cards == null || cards.Length == 0)
        {
            return new int[0];
        }
        int[] result = new int[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            result[i] = cards[i].instance_id;

        }

        return result;
    }

    public static Card[] GetMyCards(Player player)
    {
        List<Card> result = new List<Card>();
        instance.cardsGenerated.ForEach(c =>
        {
            if (c.myPlayer == player)
            {
                result.Add(c);
            }
        }
        );
        return result.ToArray();
    }


    public static Card[] GetCardsById(int[] cards)
    {
        Card[] result = new Card[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            result[i] = GetCardById(cards[i]);

        }

        return result;
    }

    public static string GetNameCardById(int id)
    {
        Card card = GetCardById(id);
        if (card == null)
        {
            return "null card";
        }
        return card.title;
    }
    public static string GetNameCardsByIds(int[] ids)
    {
        string result = "";
        if (ids == null)
        {
            return result;
        }

        for (int i = 0; i < ids.Length; i++)
        {
            result += GetNameCardById(ids[i]) + ((ids.Length == i + 1) ? " " : ", ");
        }
        return result;
    }

    public static Card GetCardById(int id)
    {
        if (id < 0)
        {
            return null;
        }
        if (!instance.cardsGenerated.Exists(c => c.instance_id == id))
        {
            Debug.Log("__________CardLoader Id=" + id + " card not found");
            return null;
        }

        return instance.cardsGenerated.Find(c => c.instance_id == id);
    }


    public static List<Card> LoadDeck()
    {
        List<Card> deck = LoadDeckToPlayerPrefs();

        if (deck.Count == 0)
        {
            Debug.LogWarning("Problem loading deck; Getting default deck...");
            deck = LoadDefaultDeck();
        }
        return deck;
    }


    //fill deck list from player prefs
    static List<Card> LoadDeckToPlayerPrefs()
    {
        List<Card> deck = new List<Card>();
        if (PlayerPrefs.HasKey("deck"))
        {
            //parse
            string jsonDeck = PlayerPrefs.GetString("deck");
            List<string> cards = JsonUtility.FromJson<DeckAux>(jsonDeck).deck;
            Debug.Log("card detected from json=" + cards.Count);
            foreach (var titleCard in cards)
            {
                Card cardPrefab = CardManager.GetCardPrefabByTitle(titleCard);
                if (cardPrefab == null)
                {
                    Debug.Log("CardType null trying to load fro player prefs");
                }
                else
                {
                    deck.Add(cardPrefab);
                    Debug.Log("Loading title=" + titleCard + " from player card");
                }
            }
            Debug.Log("jsonDeck Loading=" + jsonDeck);
        }
        return deck;
    }
    
    //use copiesInDeck property in each card as default
    public static List<Card> LoadDefaultDeck()
    {
        List<Card> deck = new List<Card>();
        foreach (var cardType in CardManager.instance.cardTypes)
        {
            for (int i = 0; i < cardType.defaultCopiesInUse; i++)
                deck.Add(cardType);
        }
        Debug.Log("loading default deck=" + deck.Count);
        return deck;
    }

    //save in string[] in player prefs deck list
    public static void SaveDeckToPlayerPrefs(List<Card> deck)
    {
        if (deck.Count != 41)
        {
            Debug.Log("__ failed to save, deck has less than 41 cards!");
            return;
        }

        List<string> cards = new List<string>();
        foreach (var cardType in deck)
            cards.Add(cardType.title);
        
        string jsonDeck = JsonUtility.ToJson(new DeckAux(cards), true);
        PlayerPrefs.SetString("deck", jsonDeck);
        Debug.Log("jsonDeck Saving=" + jsonDeck);
    }
}

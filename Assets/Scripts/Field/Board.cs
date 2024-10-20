using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
/// <summary>
/// Holds reference for elements of gameplay player
/// </summary>
public class Board : MonoBehaviour {

 
    public Player myPlayer;

  

    [HideInInspector] public DeckSlot myDeck;
    [HideInInspector] public Hand myHand;


    [HideInInspector] public HeroCardSlot heroCardSlot;
    [HideInInspector] public HelperCardSlot1 helperCardSlotLeft;
    [HideInInspector] public HelperCardSlot2 helperCardSlotMiddle;
    [HideInInspector] public HelperCardSlot3 helperCardSlotRight;
    [HideInInspector] public FlippedCardSlot flippedCardSlot;
    [HideInInspector] public NormalBuildingSlot1 normalBuildingCardSlotLeft;
    [HideInInspector] public NormalBuildingSlot2 normalBuildingCardSlotMiddle;
    [HideInInspector] public NormalBuildingSlot3 normalBuildingCardSlotRight;
    [HideInInspector] public SpecialBuildingSlot1 specialBuildingCardSlotLeft;
    [HideInInspector] public SpecialBuildingSlot2 specialBuildingCardSlotRight;
    [HideInInspector] public UsedCardsSlot usedCardsSlot;
    [HideInInspector] public StackAttackSlot stackAttackSlot;
    [HideInInspector] public AttackWaitListSlot attackWaitListSlot;

    BarrierLineMiddleGame _barrierLine;
    public BarrierLineMiddleGame barrierLine
    {
        get
        {
            if (_barrierLine == null)
            {
                _barrierLine = GetComponentInChildren<BarrierLineMiddleGame>(true);
            }
            return _barrierLine;
        }
    }
    
    FieldArea _fieldArea;
    public FieldArea fieldArea
    {
        get
        {
            if (_fieldArea == null)
            {
                _fieldArea = GetComponentInChildren<FieldArea>(true);
            }
            return _fieldArea;
        }
    }


    Timer _timer;
    public Timer timer
    {
        get
        {
            if (_timer == null)
            {
                _timer = GetComponentInChildren<Timer>(true);
            }
            return _timer;
        }
    }
    
    void Awake()
    {

        //set player reference on card slots
        GetComponentsInChildren<CardSlot>().ToList().ForEach( cardSlot => cardSlot.myPlayer = myPlayer);
        //set reference plater on cards
       
        


        myDeck = GetComponentInChildren<DeckSlot>();
        myHand = GetComponentInChildren<Hand>();
        flippedCardSlot = GetComponentInChildren<FlippedCardSlot>();
        usedCardsSlot = GetComponentInChildren<UsedCardsSlot>();

        heroCardSlot = GetComponentInChildren<HeroCardSlot>();
        helperCardSlotLeft = GetComponentInChildren<HelperCardSlot1>();
        helperCardSlotMiddle = GetComponentInChildren<HelperCardSlot2>();
        helperCardSlotRight = GetComponentInChildren<HelperCardSlot3>();
        normalBuildingCardSlotLeft = GetComponentInChildren<NormalBuildingSlot1>();
        normalBuildingCardSlotMiddle = GetComponentInChildren<NormalBuildingSlot2>();
        normalBuildingCardSlotRight = GetComponentInChildren<NormalBuildingSlot3>();
        specialBuildingCardSlotLeft = GetComponentInChildren<SpecialBuildingSlot1>();
        specialBuildingCardSlotRight = GetComponentInChildren<SpecialBuildingSlot2>();
        stackAttackSlot = GetComponentInChildren<StackAttackSlot>();
        attackWaitListSlot = GetComponentInChildren<AttackWaitListSlot>();

    }

    /// <summary>
    /// ignores Used,Deck,Hand,AttackWaitList,Stackattack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T TryGetCardOnField<T>() where T : Card
    {

        // the order of childs is important here!
        List<CardSlot> cardSlots =  GetComponentsInChildren<CardSlot>().ToList();
        cardSlots.RemoveAll(slot=> slot is UsedCardsSlot || slot is DeckSlot || slot is Hand || slot is AttackWaitListSlot || slot is StackAttackSlot );
        foreach (var slot in cardSlots)
        {
            T result = slot.cards.Find(card => card is T) as T;
            if(result!=null){
                return result;
            }
        }

        return default(T);
    }

    /// <summary>
    /// ignores Usedcards,deck,hand,attack wait list and stack attack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<T> TryGetAllCardsOnField<T>() where T : Card
    {
        // the order of childs is important here!
        List<T> results = new List<T>();
        List<CardSlot> cardSlots = GetComponentsInChildren<CardSlot>().ToList();
        cardSlots.RemoveAll(slot => slot is UsedCardsSlot || slot is DeckSlot || slot is Hand || slot is AttackWaitListSlot || slot is StackAttackSlot);
        foreach (var slot in cardSlots)
        {
            T result = slot.cards.Find(card => card is T) as T;
            if (result != null)
            {
                results.Add(result);
            }
        }

        return results;
    }

    
    /// <summary>
    /// ignores UsedCardsSlot, DeckSlot, Hand, FlippedCardSlot, AttackWaitListSlot, StackAttackSlot
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public CardSlot GetFreeCardSlot(Card card)
    {

        List<CardSlot> cardSlots = GetComponentsInChildren<CardSlot>().ToList();
        cardSlots.RemoveAll(slot => slot is UsedCardsSlot || slot is DeckSlot || slot is Hand || slot is FlippedCardSlot || slot is AttackWaitListSlot || slot is StackAttackSlot);
        foreach (var slot in cardSlots)
        {
            if ( slot.OnAllowCardAdd(card) && slot.cards.Count==0 ){
                return slot;
            }
        }

        Debug.Log("Board.ChooseAppropriateSlot: no slots found for cardtype=" + card.GetType());
        return null;
    }


    
}

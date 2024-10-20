using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Player : MonoBehaviour {

    [HideInInspector]
    public Card currentDraggingCard;
    public Board myBoard;

    public PlayerStatus status;
    public Player otherPlayer {
        get
        {
            if (this is SelfPlayer)
            {
                return GameMechanics.instance.opponentPlayer;
            }
            else
            {
                return GameMechanics.instance.selfPlayer;
            }
        }
    }

    public delegate void OnPlayerExecutedAction(Action action);
    public event OnPlayerExecutedAction onPlayerExecutedAction;
    //keep a record of past actions
    public List<ActionsTurn> actionsTaken = new List<ActionsTurn>();

    void Start()
    {
        GameMechanics.instance.turnManager.onBeginTurn += (player) => { if(player == this)  OnMyTurnBegin(); };

        StartCoroutine(ExecutingStackEffects());
    }

    void OnMyTurnBegin()
    {
        actionsTaken.Add(new ActionsTurn());
    }

    public void OnActionExecuted(Action action)
    {
        
        actionsTaken[actionsTaken.Count - 1].actionsTaken.Add(action);
     
        if(onPlayerExecutedAction!=null)
            onPlayerExecutedAction(action);
        if(this is SelfPlayer){
            Gameplay.instance.GetPlayerNetwork(this).ExecuteActionNetwork(action.card_instance,action.UseFlipped,action.targetCards,action.payCards);
        }
    }


    //used by button
    public void EndMyTurn()
    {
        
        Gameplay.instance.GetPlayerNetwork(this).EndMyTurnNetwork();
    }



    Stack<System.Action> stackEffects = new Stack<System.Action>();
    public void ActivateEffectOnStack(System.Action action)
    {
        if(action==null){
            Debug.Log("Null effect");
            return;
        }
        stackEffects.Push(action);
    }
    IEnumerator ExecutingStackEffects()
    {
        for (; ; )
        {
            if(stackEffects.Count>0){
                stackEffects.Pop()();
            }

            yield return new WaitForSeconds(1f);
        }
    }

   


    /// <summary>
    /// move card from deck to my hand
    /// </summary>
    /// <param name="timeTransitionToHand"></param>
    public Card DrawTopCardFromMyDeck(float timeTransitionToHand=1f)
    {

        if( myBoard.myHand.cards.Count >= myBoard.myHand.myDropZone.max ){
            Debug.Log(" hand  reached max of 11 cards ");
            if(this is SelfPlayer){
                LogTextUI.Log("There’re too many cards in the hand");
            }

            return null;
        }
        if(myBoard.myDeck.cards.Count==0){
            Debug.Log("_______ no cards to be draw!");
            return null;
        }
        Card card = null;
        
        card = myBoard.myDeck.cards[myBoard.myDeck.cards.Count - 1];
        //Debug.Log("test draw card");
        //card = DrawCardFromMyDeck<BasicAttack>();
        card.myDrag.MoveToNewDropZone(myBoard.myHand.myDropZone, timeTransitionToHand);
        SoundManager.instance.cardDrawFromDeck.Play();
        return card;

    }

    public T DrawCardFromMyDeck<T>(float timeTransitionToHand = 1f) where T : Card
    {
        if (myBoard.myHand.cards.Count >= myBoard.myHand.myDropZone.max)
        {
            Debug.Log("Player.DrawOneCardFromMyDeck hand has too many cards");
            return null;
        }
        T card = myBoard.myDeck.FindCardSpecefic<T>();
        if(card==null){
            Debug.Log("Failed to Draw one specific Card from deck " + typeof(T));
            return null;
        }
        card.myDrag.MoveToNewDropZone(myBoard.myHand.myDropZone, timeTransitionToHand);

        return card;
    }
    

    /// <summary>
    /// set allow drag cards on hand
    /// </summary>
    public void SetAllowDrag(bool _allowDrag)
    {
        myBoard.myHand.cards.ForEach(c => c.myDrag.allowDrag = _allowDrag);
    }


    /// <summary>
    /// moves from hand to use card slot
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void PayOneCard<T>() where T : Card
    {
        T card = myBoard.myHand.FindCardSpecefic<T>();
       
        if (card != null)
        {
            card.myDrag.MoveToNewDropZone(myBoard.usedCardsSlot.myDropZone);
        }
        else
        {
            Debug.Log("Player.UseCard: didnt find card in hands " + typeof(T));
        }
    }

    

}

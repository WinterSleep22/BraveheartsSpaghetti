using UnityEngine;
using System.Collections.Generic;
using IEnumerator = System.Collections.IEnumerator;
using UnityEngine.UI;
using System.Linq;

public class CardAI : MonoBehaviour {


    public bool TryToBuilding = false;
    public bool AlwaysAttackFirst = false;
    public Player myPlayer
    {
        get
        {
            return GameMechanics.instance.opponentPlayer;
        }
    }
    void Start()
    {

        GameMechanics.instance.turnManager.onBeginTurn += (p) => { if (p == myPlayer) OnMyTurnStarted(); };
        GameMechanics.instance.turnManager.onEndTurn += (p) => { if (p == myPlayer) OnMyTurnEnd(); };
        myPlayer.onPlayerExecutedAction += OnIExecutedAction;
    }





    void OnMyTurnStarted()
    {
        if (!enabled)
        {
            return;
        }
        StartCoroutine(Playing());
    }

    void OnMyTurnEnd()
    {
        StopAllCoroutines();
    }

    void OnIExecutedAction(Action action)
    {

    }



    IEnumerator Playing()
    {
        List<System.Action> avaiableActions = PlayerInputControl.instance.GetAvaiableActions();
        for (; ; )
        {
            if (GameMechanics.instance.turnManager.currentPlayer != myPlayer)
            {
                Debug.Log("AI: not my Turn cancelling AI");
                break;
            }
            if (GameMechanics.instance.turnManager.currentSecondsTurnLeft < 7)
            {
                Debug.Log("AI: less than 7 seconds, cancelling AI");
                break;
            }
            if (avaiableActions.Count <= 0)
            {
                Debug.Log("No actions to perform, cancelling AI (" + avaiableActions.Count + ")");
                break;
            }

            Debug.Log("AI: processing with avaiableActions=" + avaiableActions.Count);

            while (TurnManager.isGamePaused)
            {
                Debug.Log("Game is paused; waiting for unpause to continue AI");
                yield return null;
            }

            //choose
            System.Action actionChoosed = avaiableActions.ElementAt(Random.Range(0, avaiableActions.Count - 1));
            if (actionChoosed == null)
            {
                Debug.LogWarning("Error choosing AI action", this);
                break; // Error handling
            }

            while (TurnManager.isGamePaused)
            {
                Debug.Log("Game is paused; waiting for unpause to continue AI");
                yield return null;
            }
            //execute
            Debug.Log("Enemy action: " + actionChoosed.Method.Name);

            actionChoosed();

            avaiableActions.Remove(actionChoosed);

            /*
            //second way
            Card cardToUse = myPlayer.myBoard.myHand.cards[Random.Range(0,myPlayer.myBoard.myHand.cards.Count-1)];

            Action howTouse = HowToUse(cardToUse);//behaviour tree or state machine of something

            PlayerInputControl.instance.UseCard(howTouse);
            */
            yield return new WaitWhile(() =>
            {
                bool wait = Gameplay.instance.IsThereACardBeingActivated(myPlayer) != null;
                //if (wait)
                //  Debug.Log("ai waiting action being executed");
                return wait;
            });

            yield return new WaitForSeconds(2f);
        }
        PlayerInputControl.instance.EndTurn();
    }

    Action HowToUse(Card card)
    {
        //behaviour tree here
        Action newAction = new Action(card.instance_id);

        if (card is BuildingCard)
        {
            newAction.payCards = CardManager.GetIdByCards(SelectCardsFromHandToPayForBuilding((card as BuildingCard).cost, card.myPlayer).ToArray());
        }

        if (card is AttackCard)
        {
            //Target watch tower
        }

        if (card is DefenseCard)
        {
            newAction.UseFlipped = true;
        }

        if (card is Raid)
        {
            if (myPlayer.otherPlayer.myBoard.flippedCardSlot.cards.Count == 1)
            {
                newAction.UseFlipped = true;
            }
        }



        //TODO

        //etc
        return newAction;
    }

    public static List<Card> SelectCardsFromHandToPayForBuilding(int amount, Player player)
    {
        List<Card> cardsToPay = new List<Card>();

        //try pay with cards in this order
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<BasicAttack>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<BasicDefense>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<Insight>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<Fighter>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<CounterAttack>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<Vanguard>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<Raid>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<AdvancedSupplyBase>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<SwordForge>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<Barrier>());
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.FindCards<ArmourForge>());

        //easy way
        if (cardsToPay.Count < amount)
            cardsToPay.AddRange(player.myBoard.myHand.cards);

        //remove any extra cards
        while (cardsToPay.Count > amount)
        {
            cardsToPay.RemoveAt(cardsToPay.Count - 1);
        }
        Debug.Log("Player input control trying to pay " + cardsToPay.ToStringValues());
        return cardsToPay;
    }


}

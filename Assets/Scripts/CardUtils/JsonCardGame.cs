using UnityEngine;
using System.Collections.Generic;

using System.Linq;

[System.Serializable]
public class GameState
{

    public BoardStatus selfBoard;
    public BoardStatus opponentBoard;


    public bool CompareOpponentGameState(GameState opponentGame)
    {
        if(!selfBoard.CompareBoard(opponentGame.opponentBoard)){
            Debug.Log("my selfBoard Differs from his opponentBoard");
            return false;
        }
        if(!opponentBoard.CompareBoard(opponentGame.selfBoard)){
            Debug.Log("my opponentBoard Differs from his selfBoard");
            return false;
        }
        return true;
    }

    public static GameState GetCurrentGameState()
    {
        GameState gameState = new GameState();
        gameState.selfBoard = new BoardStatus(GameMechanics.instance.selfPlayer.myBoard);
        gameState.opponentBoard = new BoardStatus(GameMechanics.instance.opponentPlayer.myBoard);
        return gameState;
    }

   
    public static GameState FromJson(string json)
    {
        return JsonUtility.FromJson<GameState>(json);
    }
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
    public string ToString(bool pretty=false)
    {
        return JsonUtility.ToJson(this, pretty);
    }
}


[System.Serializable]
public class BoardStatus
{
    public int flippedSlot;

    public int normalBuildingLeft;
    public int normalBuildingMiddle;
    public int normalBuildingRight;

    public int specialBuildingLeft;
    public int specialBuildingRight;

    public int helperLeft;
    public int helperMiddle;
    public int helperRight;

    public int[] deck;
    public int[] usedCards;
    public int[] stackAttack;
    public int[] hand;


    public BoardStatus(Board board)
    {
        flippedSlot = GetIdFromSlot(board.flippedCardSlot);
        normalBuildingLeft = GetIdFromSlot(board.normalBuildingCardSlotLeft);
        normalBuildingMiddle = GetIdFromSlot(board.normalBuildingCardSlotMiddle);
        normalBuildingRight = GetIdFromSlot(board.normalBuildingCardSlotRight);
        specialBuildingLeft = GetIdFromSlot(board.specialBuildingCardSlotLeft);
        specialBuildingRight = GetIdFromSlot(board.specialBuildingCardSlotRight);
        helperLeft = GetIdFromSlot(board.helperCardSlotLeft);
        helperMiddle = GetIdFromSlot(board.helperCardSlotMiddle);
        helperRight = GetIdFromSlot(board.helperCardSlotRight);

        deck = GetAllIdFromSlot(board.myDeck);
        usedCards = GetAllIdFromSlot(board.usedCardsSlot);
        stackAttack = GetAllIdFromSlot(board.stackAttackSlot);
        hand = GetAllIdFromSlot(board.myHand);
    }
    
    public bool CompareBoard(BoardStatus boardStatus)
    {
        if (flippedSlot != boardStatus.flippedSlot)
        {
            Debug.Log("flippedSlot differs " + CardManager.GetNameCardById(flippedSlot) + " from " + CardManager.GetNameCardById(boardStatus.flippedSlot));
            return false;
        }
        if (normalBuildingLeft != boardStatus.normalBuildingLeft)
        {
            Debug.Log("normalBuildingLeft differs " + CardManager.GetNameCardById(normalBuildingLeft) + " from " + CardManager.GetNameCardById(boardStatus.normalBuildingLeft));
            return false;
        }
        if (normalBuildingMiddle != boardStatus.normalBuildingMiddle)
        {
            Debug.Log("normalBuildingMiddle differs " + CardManager.GetNameCardById(normalBuildingMiddle) + " from " + CardManager.GetNameCardById(boardStatus.normalBuildingMiddle));
            return false; 
        }
        if (normalBuildingRight != boardStatus.normalBuildingRight)
        {
            Debug.Log("normalBuildingRight differs " + CardManager.GetNameCardById(normalBuildingRight) + " from " + CardManager.GetNameCardById(boardStatus.normalBuildingRight));
            return false;
        }
        if (specialBuildingLeft != boardStatus.specialBuildingLeft)
        {
            Debug.Log("specialBuildingLeft differs " + CardManager.GetNameCardById(specialBuildingLeft) + " from " + CardManager.GetNameCardById(boardStatus.specialBuildingLeft));
            return false;
        }
        if (specialBuildingRight != boardStatus.specialBuildingRight)
        {
            Debug.Log("specialBuildingRight differs " + CardManager.GetNameCardById(specialBuildingRight) + " from " + CardManager.GetNameCardById(boardStatus.specialBuildingRight));
            return false;
        }
        if (helperMiddle != boardStatus.helperMiddle)
        {
            Debug.Log("helperMiddle differs " + CardManager.GetNameCardById(helperMiddle) + " from " + CardManager.GetNameCardById(boardStatus.helperMiddle));
            return false;
        }
        if (helperRight != boardStatus.helperRight)
        {
            Debug.Log("helperRight differs " + CardManager.GetNameCardById(helperRight) + " from " + CardManager.GetNameCardById(boardStatus.helperRight));
            return false; 
        }
        
        if(!CompareArrays(deck,boardStatus.deck,"deck")){
            
            return false;
        }
        if (!CompareArrays(usedCards,boardStatus.usedCards,"usedCards"))
        {
         
            return false;
        }
        if (!CompareArrays(stackAttack,boardStatus.stackAttack,"stackAttack"))
        {
          
            return false;
        }
        if (!CompareArrays(hand,boardStatus.hand,"hand"))
        {
        
            return false;
        }
        return true;
    }
    bool CompareArrays(int[] a,int[] b,string arrayName)
    {
        List<int> array1 = new List<int>(a);
        List<int> array2 = new List<int>(b);
        array1.Sort();
        array2.Sort();
        if(array2.Count != array1.Count){
             Debug.Log(  arrayName+ " differs at size from " + array1.Count  + " to " + array2.Count);
             return false;
        }
        for (int i = 0; i < array1.Count; i++ )
        {
            if(array1[i] != array2[i]){
               
                Debug.Log(  arrayName+ " differs at position=" + i+" from " + CardManager.GetNameCardById(array1[i])+ "  to "+ CardManager.GetCardById(array2[i]).title  );
              //  DebugCardIds(a, arrayName );
               // DebugCardIds(b, arrayName );
                return false;
            }
        }
        return true;
    }
    void DebugCardIds(int[] ids,string title)
    {
        string result = "";
        foreach (var id in ids)
        {
            var card = CardManager.GetCardById(id);
            result += card.title + ",";
        }
        Debug.Log(title +" "+ result);

    }
    int GetIdFromSlot(CardSlot cardSlot)
    {
        if (cardSlot.cards.Count == 0)
        {
            if(cardSlot.GetComponentInChildren<Card>()!=null){
                return cardSlot.GetComponentInChildren<Card>().instance_id;
            }

            return -1;
        }
        else
        {
            return cardSlot.cards[0].instance_id;
        }
    }

    int[] GetAllIdFromSlot(CardSlot cardSlot)
    {
  

        int[] result = new int[cardSlot.cards.Count];
        for (int i = 0; i < cardSlot.cards.Count; i++)
        {

            result[i] = cardSlot.cards[i].instance_id;
        }
        return result;
    }

    public BoardStatusReadable GetReadableData()
    {
        return new BoardStatusReadable(this);
    }

    public static BoardStatus FromJson(string json)
    {
        return JsonUtility.FromJson<BoardStatus>(json);
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}


[System.Serializable]
public class BoardStatusReadable
{
    public string flippedSlot;

    public string normalBuildingLeft;
    public string normalBuildingMiddle;
    public string normalBuildingRight;

    public string specialBuildingLeft;
    public string specialBuildingRight;

    public string helperLeft;
    public string helperMiddle;
    public string helperRight;

    public string[] deck;
    public string[] usedCards;
    public string[] stackAttack;
    public string[] hand;


    public BoardStatusReadable(BoardStatus boardStatus)
    {
        flippedSlot = GetTitle(boardStatus.flippedSlot);
        normalBuildingLeft = GetTitle(boardStatus.normalBuildingLeft);
        normalBuildingMiddle = GetTitle(boardStatus.normalBuildingMiddle);
        normalBuildingRight = GetTitle(boardStatus.normalBuildingRight);
        specialBuildingLeft = GetTitle(boardStatus.specialBuildingLeft);
        specialBuildingRight = GetTitle(boardStatus.specialBuildingRight);
        helperLeft = GetTitle(boardStatus.helperLeft);
        helperMiddle = GetTitle(boardStatus.helperMiddle);
        helperRight = GetTitle(boardStatus.helperRight);

        deck = new string[boardStatus.deck.Length];
        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = GetTitle(boardStatus.deck[i]);
        }

        usedCards = new string[boardStatus.usedCards.Length];
        for (int i = 0; i < usedCards.Length; i++)
        {
            usedCards[i] = GetTitle(boardStatus.usedCards[i]);
        }

        stackAttack = new string[boardStatus.stackAttack.Length];
        for (int i = 0; i < stackAttack.Length; i++)
        {
            stackAttack[i] = GetTitle(boardStatus.stackAttack[i]);
        }

        hand = new string[boardStatus.hand.Length];
        for (int i = 0; i < hand.Length; i++)
        {
            hand[i] = GetTitle(boardStatus.hand[i]);
        }

    }
    string GetTitle(int id)
    {
        Card card = CardManager.GetCardById(id);
        if (card == null)
        {
            return "NULL";
        }
        return card.title;
    }

    public static BoardStatusReadable FromJson(string json)
    {
        return JsonUtility.FromJson<BoardStatusReadable>(json);
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}




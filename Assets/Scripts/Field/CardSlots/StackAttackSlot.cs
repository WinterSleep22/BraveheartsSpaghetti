using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class StackAttackSlot : CardSlot
{
    [ContextMenuItem("OrganizeCards", "OrganizeCards")]
    public Vector2 cardSize = new Vector2(0.4f, 0.2f);

    public float spaceX = 1;
    public float offsetX = 1;
    public float offsetY = 1;
    public float spaceY = 1;

    #region AttackStack logic
    /*

     private GridLayoutGroup _stackGrid;
     public GridLayoutGroup StackGrid
     {
         get
         {
             if (_stackGrid ==null)
             {
                 _stackGrid = GetComponent<GridLayoutGroup>();
             }
             return _stackGrid;
         }
     }

     */

    private int attackNumberInThisturn = 0;
    public int GetNumberOfAttackInThisTurn
    {
        get { return attackNumberInThisturn; }
    }



    public delegate void OnAttackedPlayer();
    public OnAttackedPlayer onAttackedPlayerCallback;

    public delegate void OnReachMaxCallback();
    public OnReachMaxCallback onReachMaxCallback;



    public void AddAttack()
    {
        if (myPlayer is SelfPlayer)
        {
            SoundManager.instance.selfSufferAttack.Play();
        }
        else
        {
            SoundManager.instance.opponentSufferAttack.Play();
        }

        attackNumberInThisturn++;
        if (onAttackedPlayerCallback != null)
        {
            onAttackedPlayerCallback();
        }

        if (cards.Count >= 10)
        {
            if (onReachMaxCallback != null)
            {
                onReachMaxCallback();
            }
        }

    }


    #endregion

    #region Slot logic

    public override void Awake()
    {

        base.Awake();
        myDropZone.max = 10;
        myDropZone.allowDrop = false; //player cant use input for stack here
        ShowGraphics();
    }

    ///just to  override the default behaviour of card slot to hide graphics
    public override void Start()
    {
        GameMechanics.instance.turnManager.onBeginTurn += OnPlayerBeginTurn;
    }

    void OnPlayerBeginTurn(Player player)
    {
        if (player == myPlayer)
        {
            attackNumberInThisturn = 0;
        }
    }

    public override bool OnAllowCardAdd(Card _card)
    {
        if (!(_card is AttackCard))
        {
            Debug.Log("________StackAttackSlot:card isnt the type of attack card=" + _card.title);

        }
        return true;
    }

    public override void OnCardBeginComeBackDrag(Card card)
    {
        AdjustCardSize(card);
    }

    public override void OnCardEndComeBackDrag(Card card)
    {
        card.myCardGraphics.ShowBack();

        OrganizeCards();

    }
    void AdjustCardSize(Card card)
    {


        iTween.ScaleTo(card.gameObject, iTween.Hash(
           "scale", new Vector3(cardSize.x, cardSize.y, 1),
           "time", 2f
           ));
    }
    void AdjustCardPosition(Card card)
    {
        iTween.MoveTo(card.gameObject, iTween.Hash(
           "position", transform.position + GetCurrentPosition(cards.IndexOf(card)),
           "time", 1f

           ));
    }
    public void OrganizeCards()
    {
        foreach (var card in cards)
        {
            AdjustCardPosition(card);
            AdjustCardSize(card);
        }
    }



    public override void OnCardEnter(Card _card)
    {

        AddAttack();

    }
    public Vector3 GetCurrentPosition(int myIndice)
    {

        // Debug.Log("StackAttackSlot: GetMyPosition myIndice=" + myIndice);
        float y = offsetY - myIndice % 5 * (spaceY) + myRectTransform.GetHeight() / 2f;
        float x = offsetX - myRectTransform.GetWidth() / 2f;

        if (myIndice >= 5)
        {
            return new Vector3(x + spaceX, y, 0);
        }
        else
        {
            return new Vector3(x, y, 0);
        }
    }
    public override Vector3 GetMyPosition(Card card)
    {
        int myIndice = cards.IndexOf(card);
        return GetCurrentPosition(myIndice);

        //return new Vector3(StackGrid.padding.right + StackGrid.cellSize.x/2f- myRectTransform.GetWidth()/2f , myRectTransform.GetHeight() / 2f - StackGrid.cellSize.y, 0);
    }


    #endregion

}

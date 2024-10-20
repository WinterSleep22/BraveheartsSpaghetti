using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Specialize drag and drop to cards
/// </summary>
[RequireComponent(typeof(DropZone))]
public abstract class CardSlot : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector]
    public Player myPlayer;
    // [HideInInspector]
    public List<Card> cards = new List<Card>();
    public OnChangeCardNumber onChangeCardNumber;
    public delegate void OnChangeCardNumber(CardSlot me);

    //TODO callbacks to remove function and connection to the class Game
    public delegate void OnCardEnterCallBack(CardSlot me, Card newCard);
    public delegate void OnCardExitCallBack(CardSlot me, Card oldCard);
    public OnCardEnterCallBack onCardEnterCallback;
    public OnCardExitCallBack onCardExitCallback;


    private DropZone _myDropZone;
    public DropZone myDropZone
    {
        get
        {
            if (_myDropZone == null)
            {
                _myDropZone = GetComponent<DropZone>();
            }
            return _myDropZone;
        }
    }

    private RectTransform _myRectTransform;
    public RectTransform myRectTransform
    {
        get
        {
            if (_myRectTransform == null)
            {
                _myRectTransform = GetComponent<RectTransform>();
            }
            return _myRectTransform;
        }
    }


    private Image _myImage;
    public Image myImage
    {
        get
        {
            if (_myImage == null)
            {
                _myImage = GetComponent<Image>();
            }
            return _myImage;
        }
    }
    /// <summary>
    /// assign callbacks(IMPORTANT!) use base.Awake() max 1 card default
    /// </summary>
    public virtual void Awake()
    {
        //  Debug.Log("initializing card slot");
        myDropZone.max = 1;
        myDropZone.onAllowCardEnter += OnAllowDragAdd;
        myDropZone.onEnterDropCallback += OnCardEnter;
        myDropZone.onExitDropCallback += OnCardExit;
        myDropZone.onCardComeBackDragCallback += OnCardEndComeBackDrag;
        myDropZone.onCardBeginComeBack += OnCardBeginComeBackDrag;
        myDropZone.getDragPosition += GetMyPosition;
        myDropZone.getDragRotation += GetMyRotation;
        myDropZone.onBeingDropped += OnDragBeingDropped;
        myDropZone.onAllowDragToDrop += OnDragAllowDrop;
        cards.Clear();
    }
    public virtual void Start()
    {
        HideGraphics();
    }




    public T FindCardSpecefic<T>() where T : Card
    {
        return cards.Find(c => c.GetType() == typeof(T)) as T;
    }

    public T FindCardOfType<T>() where T : Card
    {
        return cards.Find(c => c is T) as T;
    }
    public List<Card> FindCards<T>() where T : Card
    {
        return cards.FindRange(c => c is T);
    }


    private Vector3 GetMyPosition(Draggable drag)
    {
        return GetMyPosition(drag.GetComponent<Card>());
    }
    public virtual Vector3 GetMyRotation(Card card)
    {
        return Vector3.zero;
    }

    private Vector3 GetMyRotation(Draggable drag)
    {
        return GetMyRotation(drag.GetComponent<Card>());
    }
    public virtual Vector3 GetMyPosition(Card card)
    {
        return Vector3.zero;
    }


    private void OnCardBeginComeBackDrag(Draggable drag)
    {
        OnCardBeginComeBackDrag(drag.GetComponent<Card>());
    }

    private bool OnDragAllowDrop(Draggable drag)
    {
        return OnAllowCardBeDropped(drag.GetComponent<Card>());
    }
    private void OnDragBeingDropped(Draggable drag)
    {
        OnCardBeingDropped(drag.GetComponent<Card>());
    }

    private void OnCardEndComeBackDrag(Draggable drag)
    {
        OnCardEndComeBackDrag(drag.GetComponent<Card>());
    }

    private bool OnAllowDragAdd(Draggable drag)
    {
        return OnAllowCardAdd(drag.GetComponent<Card>());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drag"></param>
    private void OnCardExit(Draggable drag)
    {
        Card cardToBeRemoved = drag.GetComponent<Card>();
        cards.Remove(cardToBeRemoved);
        cardToBeRemoved.OnExitCardSlot(this);
        OnCardExit(cardToBeRemoved);
        if (onCardExitCallback != null)
        {
            onCardExitCallback(this, cardToBeRemoved);
        }
        if (cardToBeRemoved is ObjectCard)
        {
            cardToBeRemoved.transform.localScale = cardToBeRemoved.transform.localScale / 0.8f;
        }
        if (onChangeCardNumber != null)
            onChangeCardNumber(this);
    }

    private void OnCardEnter(Draggable drag)
    {
        Card newCard = drag.GetComponent<Card>();
        if (!cards.Contains(newCard))
        {
            cards.Add(newCard);
        }
        else
        {
            //Debug.Log("card already exist " + gameObject.name + " card=" + newCard.title);
        }

        newCard.OnEnterCardSlot(this);
        OnCardEnter(newCard);
        if (onCardEnterCallback != null)
        {
            onCardEnterCallback(this, newCard);
        }

        if (onChangeCardNumber != null)
            onChangeCardNumber(this);


    }

    public virtual void OnCardBeginComeBackDrag(Card card)
    {

    }

    /// <summary>
    /// useful call back to activate card on drop in field
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnCardBeingDropped(Card card)
    {

    }

    public virtual bool OnAllowCardBeDropped(Card card)
    {
        return false;
    }

    /// <summary>
    /// default behaviour is trying to show front on field, if fail, show front, scale card to 1
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnCardEndComeBackDrag(Card _card)
    {
        iTween.ScaleTo(_card.gameObject, Vector3.one, 1f);
        _card.myCardGraphics.ShowFrontOnField();
        HideGraphics();

    }


    /// <summary>
    /// default behaviour is return if player of card is the same as this card slot
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public virtual bool OnAllowCardAdd(Card _card)
    {

        if (_card.myPlayer != myPlayer)
        {
            Debug.Log("OnAllowCardAdd: card player differ from cardSloplayer ");
        }
        if (cards.Count == myDropZone.max)
        {
            return false;
        }
        return _card.myPlayer == myPlayer;
    }

    public virtual void OnCardExit(Card _card)
    {
    }

    /// <summary>
    /// Default behaviour is set rotation and position to zero and try and highlight slot graphics
    /// </summary>
    /// <param name="_card"></param>
    public virtual void OnCardEnter(Card _card)
    {
        ShowGraphics();
        _card.myRectTransform.localRotation = Quaternion.identity;
        _card.myRectTransform.localPosition = new Vector3(0, 0, 0);
        _card.myRectTransform.localScale = Vector3.one;
        HideGraphics();
    }







    public void HideGraphics()
    {

        myImage.CrossFadeAlpha(0.1f, 0, true);
    }
    public void ShowGraphics()
    {
        myImage.CrossFadeAlpha(1, 0, true);
    }



    public delegate void OnClickCardSlotCallback(CardSlot me);
    public event OnClickCardSlotCallback onClickCardSlotCallback;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (onClickCardSlotCallback != null)
        {
            onClickCardSlotCallback(this);
        }
        Debug.Log("Card slot clicked = " + gameObject.name);
    }
}

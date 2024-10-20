using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {


    private Card _myCard;
    public Card myCard
    {
        get
        {
            if (_myCard == null)
            {
                _myCard = GetComponent<Card>();
            }
            return _myCard;
        }
    }

    Canvas _myCanvas;
    public Canvas myCanvas
    {
        get
        {
            if (_myCanvas == null)
            {
                _myCanvas = FindObjectOfType<Canvas>();
            }
            return _myCanvas;
        }

    }


    private RectTransform _myRecTranform;
    public RectTransform myRectTransform
    {
        get
        {
            if (_myRecTranform == null)
                _myRecTranform = GetComponent<RectTransform>();
            return _myRecTranform;
        }
    }


    private DropZone _myDropZone;
    public DropZone myDropZone { get { return _myDropZone; } }
    public void SetDropZone(DropZone newZone)
    {
        if (newZone != _myDropZone)
        {
            _myDropZone = newZone;
            if (onChangeDropZone != null)
                onChangeDropZone(this);
        }
    }

    private CanvasGroup _myCanvasGroup;
    public CanvasGroup myCanvasGroup
    {
        get
        {
            if (_myCanvasGroup == null)
                _myCanvasGroup = this.GetOrAddComponent<CanvasGroup>();
            return _myCanvasGroup;
        }
    }


    public delegate void OnBeginComeBack(Draggable drag);
    public OnBeginComeBack onBeginBackToDropZone;

    public delegate void OnEndComeBack(Draggable drag);
    public OnEndComeBack onEndBackToDropZone;

    public delegate void OnChangeDropZone(Draggable one);
    public OnChangeDropZone onChangeDropZone;



    public bool allowDrop = true;
    public bool allowDrag = true;
    private bool dragging = false;
    public bool IsDragging { get { return dragging; } }
    [HideInInspector]
    public bool isComingBackToDropZone = false;


    void Start()
    {
        if (myCard.myPlayer is OpponentPlayer)
        {
            allowDrag = false;
            allowDrop = false;

        }

    }


    /// <summary>
    /// logics to execute on card 
    /// </summary>
    public void OnCardBeginDrag()
    {
        iTween.RotateTo(gameObject, Vector3.one, 1f);
        myCard.myCardGraphics.ShowFront();
        myCard.myPlayer.currentDraggingCard = this.myCard;

    }

    /// <summary>
    /// logics to execute on card 
    /// </summary>
    public void OnCardEndDrag()
    {
        myCard.myCardGraphics.FadeAlpha(0.5f, 1, 0.5f);
        myCard.myPlayer.currentDraggingCard = null;

    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!allowDrag)
            return;



        myCard.myPlayer.myBoard.myHand.GetComponent<PlayerHandCardSelector>().currentSelectedCard = this.myCard;
        dragging = true;
        transform.rotation = Quaternion.identity;
        myCanvasGroup.blocksRaycasts = false;
        this.transform.SetParent(myCanvas.transform);
        transform.SetAsLastSibling();//card dont stay behind some background
        myCard.myPlayer.myBoard.fieldArea.gameObject.SetActive(true);
        myCard.myPlayer.myBoard.fieldArea.myImage.CrossFadeAlpha(0.5f, 0.5f, true);

        iTween.ScaleTo(this.gameObject, Vector3.one, 1f);
        Debug.Log("OnBeginDrag");

        if (myCard.isFlippable)
        {
            this.GetOrAddComponent<AutoFlip>();
        }


        OnCardBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (!allowDrag)
            return;
        // this is due the world mode canvas render
        Vector2 viewPort = new Vector2(eventData.position.x / Screen.width, eventData.position.y / Screen.height);
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(viewPort);
        worldPoint.z = transform.position.z;
        //Debug.Log("worldPoint=" + worldPoint);
        this.transform.position = worldPoint;


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        myCard.myPlayer.myBoard.fieldArea.myImage.CrossFadeAlpha(0f, 0.5f, true);//TODO: make a reference decent to this
        myCard.myPlayer.myBoard.fieldArea.SetActiveIn(0.5f, false, myCard.myPlayer.myBoard.fieldArea.gameObject);
        myCard.myPlayer.myBoard.myHand.GetComponent<PlayerHandCardSelector>().currentSelectedCard = null;
        Destroy(this.GetComponent<AutoFlip>());
        myCanvasGroup.blocksRaycasts = true;

        dragging = false;

        OnCardEndDrag();
        Debug.Log("OnEndDrag");

        if (DetectField())
        {
            Debug.Log("Drop on field");
            // Gameplay.instance.selfPlayerNetwork.ExecuteActionNetwork(myCard.instance_id, myCard.myCardGraphics.isFlipped);
            myCard.Activate();

        }
        else
        {
            if (transform.parent == myCanvas.transform)
            {
                if (myCard != null)
                {
                    ComeBackToDropZone(0.5f);
                }
            }
        }

    }
    bool DetectField()
    {
        //Create the PointerEventData with null for the EventSystem
        PointerEventData ped = new PointerEventData(null);
        //Set required parameters, in this case, mouse position
        ped.position = Input.mousePosition;
        //Create list to receive all results
        List<RaycastResult> results = new List<RaycastResult>();
        //Raycast it
        GameMechanics.instance.grCanvas.Raycast(ped, results);
        return results.Exists(r => r.gameObject.GetComponent<FieldArea>() && r.gameObject.GetComponent<FieldArea>().myPlayer == myCard.myPlayer);
    }




    public void MoveToNewDropZone(DropZone newDropZone, float transitionTime = 1f)
    {
        //TODO: better this to more free use
        Vector3 oldPosition = transform.position;
        SetDropZone(newDropZone);
        newDropZone.AddDraggable(this);//normally this method set the position of dragglable

        transform.position = oldPosition;//show as if the movement was fluid

        ComeBackToDropZone(transitionTime);
    }
    public void ComeBackToDropZone(float time = 1f)
    {
        //Debug.Log("ComeBackToDropZone " + myCard.title);

        StartCoroutine(ComingBackToDropZone(time));
    }

    IEnumerator ComingBackToDropZone(float time)
    {
        isComingBackToDropZone = true;

        transform.SetParent(myCanvas.transform);

        if (onBeginBackToDropZone != null)
            onBeginBackToDropZone(this);

        iTween.MoveTo(gameObject, iTween.Hash(
               "position", myDropZone.transform.position + myDropZone.GetMyPosition(this),
               "time", time,
               "easeType", iTween.EaseType.easeOutQuad
           ));

        iTween.RotateTo(gameObject, iTween.Hash(
               "rotation", myDropZone.transform.rotation.eulerAngles + myDropZone.GetMyRotation(this),
               "time", time,
               "easeType", iTween.EaseType.easeOutQuad
           ));



        transform.SetAsLastSibling();
        if (time > 0)
        {
            yield return new WaitForSeconds(time);
        }
        transform.SetParent(myDropZone.transform);
        isComingBackToDropZone = false;
        if (onEndBackToDropZone != null)
            onEndBackToDropZone(this);
    }


}

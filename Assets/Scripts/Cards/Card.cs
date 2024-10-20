using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnityEngine.Networking;


[RequireComponent(typeof(PhotonView))]
public class Card : NetworkBehaviour {
    public int limitCopiesInUse = 2;
    [Space]
    public int defaultCopiesInUse = 1;
    public int defaultCopiesInTotal = 2;
    [Space]
    public string title;
    public string description;
    public Sprite front;
    public Sprite thumb;
    public bool isFlippable;

    NetworkIdentity _networkIdentity;

    public NetworkIdentity networkIdentity
    {
        get
        {
            if (_networkIdentity == null)
            {
                _networkIdentity = GetComponent<NetworkIdentity>();
            }
            return _networkIdentity;
        }
    }


    PhotonView _photonView;

    public PhotonView photonView
    {
        get
        {
            if (_photonView == null)
            {
                _photonView = GetComponent<PhotonView>();
            }
            return _photonView;
        }
    }

    public int instance_id
    {
        get
        {
            if (WifiNetworkManager.instance == null)
            {
                return photonView.instantiationId;
            }
            else
            {
                return (int)networkIdentity.netId.Value;
            }
        }
    }


    void Awake()
    {


        if (!CardManager.instance.cardsGenerated.Contains(this))
        {
            CardManager.instance.cardsGenerated.Add(this);
        }
    }

    private Player _myPlayer;

    public Player myPlayer
    {
        get
        {
            if (_myPlayer == null)
            {
                if (WifiNetworkManager.instance == null)
                {
                    if (PhotonNetwork.offlineMode)
                    {
                        //this only work because in GamePlay generate self cards then later opponents cards and this list is never moififed
                        //need to do this as fast way to resolve wifi unet feature change problems
                        _myPlayer = (CardManager.instance.cardsGenerated.IndexOf(this) < 41) ? GameMechanics.instance.selfPlayer : GameMechanics.instance.opponentPlayer;
                    }
                    else
                    {
                        _myPlayer = (photonView.isMine) ? GameMechanics.instance.selfPlayer : GameMechanics.instance.opponentPlayer;
                    }
                }
                else
                {
                    _myPlayer = (hasAuthority) ? GameMechanics.instance.selfPlayer : GameMechanics.instance.opponentPlayer;

                }

            }
            return _myPlayer;
        }
    }

    private CardGraphics _myCardGraphics;
    public CardGraphics myCardGraphics
    {
        get
        {

            if (_myCardGraphics == null)
            {
                _myCardGraphics = this.GetOrAddComponent<CardGraphics>();
            }
            return _myCardGraphics;
        }
    }

    private Selectable _mySelectable;
    public Selectable mySelectable
    {
        get
        {

            if (_mySelectable == null)
            {
                _mySelectable = this.GetOrAddComponent<Selectable>();
            }
            return _mySelectable;
        }
    }

    private Draggable _myDrag;
    public Draggable myDrag
    {
        get
        {

            if (_myDrag == null)
            {
                _myDrag = this.GetOrAddComponent<Draggable>();
            }
            return _myDrag;
        }

    }

    public CardSlot cardSlot
    {
        get
        {
            if (myDrag.myDropZone == null)
            {
                return null;
            }
            return myDrag.myDropZone.GetComponent<CardSlot>();
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


    /// <summary>
    /// normally used to actiavate effect
    /// </summary>
    public virtual IEnumerator OnCardRevealedStartTurnPreAttack()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public virtual IEnumerator OnCardRevealedStartTurnPosAttack()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public override string ToString()
    {
        return title + " id=" + instance_id;
    }


    /// <summary>
    /// useful callback to activate card on placed on slot
    /// </summary>
    /// <param name="cardSlot"></param>
    public virtual void OnEnterCardSlot(CardSlot cardSlot)
    {

    }

    public virtual void OnExitCardSlot(CardSlot cardSlot)
    {

    }

    protected IEnumerator activatingReference;

    //TODO: find better way to do this
    public bool isActivating
    {
        get
        {
            if (activatingReference != null)
            {
                return activatingReference.Current != null;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// called when activiting coroutine ends
    /// </summary>
    /// <param name="action">null if activiting failed </param>
    public void OnActivatingEnd(Action action = null)
    {
        if (action != null)
        {
            Debug.Log("Action.UseCard action=" + action.ToString());
            myPlayer.OnActionExecuted(action);
        }
        else
        {
            Debug.Log("activating end=" + title + " without action to send");
            myDrag.ComeBackToDropZone();
        }
        activatingReference = null;
    }
    public void CancelActivating()
    {

        Debug.Log("cancel activating " + title);
        if (activatingReference != null)
            StopCoroutine(activatingReference);
        activatingReference = null;
        OnActivatingForcedCancel();
        myDrag.ComeBackToDropZone();


    }


    public virtual void OnCardSelectedOnHand()
    {
        SoundManager.instance.cardSelect.Play();
    }

    public virtual void OnCardDeselectedOnHand()
    {
        SoundManager.instance.cardDeselect.Play();
    }

    /// <summary>
    /// this is called whenever player wants use card from hand
    /// </summary>
    public void Activate()
    {
        //Find better way to avoid multiple cards being activate
        //if(myPlayer.myBoard.myHand.cards.Exists(c=>c.isActivating)){
        //    Debug.Log("Already exits a card being activating");
        //     return;
        // }
        SoundManager.instance.cardActivated.Play();
        activatingReference = Activating();

        StartCoroutine(activatingReference);

    }

    /// <summary>
    /// default behaviour is come back to drop zone if card slot is hand
    /// </summary>
    public virtual void OnActivatingForcedCancel()
    {
        if (myDrag.myDropZone.GetComponent<CardSlot>() is Hand)
        {
            myDrag.ComeBackToDropZone();
        }

    }

    protected virtual IEnumerator Activating()
    {
        Debug.Log("__________Activating not implemented Card " + title);
        yield return new WaitForSeconds(1f);

        yield return null;
    }



    /// <summary>
    /// default behaviour is to check if flippable if flipped card slot is avaiable
    /// </summary>
    /// <param name="printLog"></param>
    /// <returns></returns>
    public virtual bool IsAllowedToActivate()
    {
        if (isFlippable && myCardGraphics.isShowingBack)
        {
            if (myPlayer.myBoard.flippedCardSlot.cards.Count == 1)
            {
                if (myPlayer is SelfPlayer)
                {
                    LogTextUI.Log("Cant use " + title + ", flipped slot is already filled");
                }
                return false;
            }
        }
        return true;

    }


    /// <summary>
    /// scale to normal size, move,rotate card to center, set parent as canvas and show front
    /// </summary>
    public void MoveToCenter(float transitionTime = 1f, float delaytime = 0.5f, bool showFront = true)
    {
        transform.SetParent(myDrag.myCanvas.transform);
        if (showFront)
            myCardGraphics.ShowFront();
        iTween.MoveTo(gameObject, iTween.Hash("position", myDrag.myCanvas.transform.position, "time", transitionTime, "delay", delaytime));
        iTween.RotateTo(gameObject, iTween.Hash("rotation", Vector3.zero, "time", transitionTime, "delay", delaytime));
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "time", transitionTime, "delay", delaytime));
    }
}










using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Holding : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
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

    bool pointerDown = false;
    float timeHold = 1f;
    public bool holding = false;
    float timerholding = 0;
  
    void OnBeginHold()
    {
        myCard.myCardGraphics.FadeAlpha(1, 0.5f, 0.5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameMechanics.instance.turnManager.currentPlayer is OpponentPlayer)
        {
            return;
        }
        Debug.Log("Draggable: Clicked inside card");
        holding = false;
        pointerDown = true;
        timerholding = 0;

    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameMechanics.instance.turnManager.currentPlayer is OpponentPlayer)
        {
            return;
        }
        Debug.Log("Draggable: End Holding");
        holding = false;
        pointerDown = false;
        timerholding = 0;
    }

    void Update()
    {
        UpdateHoldLogic();

    }

    void UpdateHoldLogic()
    {
        if (pointerDown)
        {
            timerholding += Time.deltaTime;
        }
        if (timerholding > timeHold)
        {
            if (!holding)
            {
                OnBeginHold();

            }
            holding = true;

        }

    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackWaitListSlot : CardSlot {

    bool _isTargetWatchTowerRight = false;
    public bool isTargetToWatchTowerRight
    {
        get
        {
            return _isTargetWatchTowerRight;
        }
        set
        {
            if (value != _isTargetWatchTowerRight)
            {
                _isTargetWatchTowerRight = value;
                if (myPlayer.myBoard.specialBuildingCardSlotRight.cards.Count == 1)
                {
                    WatchTower watchTower = myPlayer.myBoard.specialBuildingCardSlotRight.cards[0] as WatchTower;
                    if (watchTower != null)
                    {
                        watchTower.SetTargetEffect(_isTargetWatchTowerRight);
                    }
                }
            }
        }
    }

    bool _isTargetToWatchTowerLeft = false;
    public bool isTargetToWatchTowerLeft
    {
        get
        {
            return _isTargetToWatchTowerLeft;
        }
        set
        {
            if (value != _isTargetToWatchTowerLeft)
            {
                _isTargetToWatchTowerLeft = value;
                if (myPlayer.myBoard.specialBuildingCardSlotLeft.cards.Count == 1)
                {
                    WatchTower watchTower = myPlayer.myBoard.specialBuildingCardSlotLeft.cards[0] as WatchTower;
                    if (watchTower != null)
                    {
                        watchTower.SetTargetEffect(_isTargetToWatchTowerLeft);
                    }
                }
            }
        }
    }

    public float offset = 200;
    public float space = 50;

    public delegate void OnPlayerSucessfullyDefend(Player player);
    public event OnPlayerSucessfullyDefend onPlayerSucessfullyDefend;

    public event OnProcessAttack onAttackPlayerDirectly;
    public delegate void OnProcessAttack();

    public event OnAttackBuilding onAttackBuilding;
    public delegate void OnAttackBuilding();

    public override void Awake()
    {
        base.Awake();
        myDropZone.max = 10;
        myDropZone.allowDrop = false; //player cant use input for stack here
        ShowGraphics();
    }

    #region CardSlot logics

    public override bool OnAllowCardAdd(Card _card)
    {
        if (!(_card is AttackCard))
        {

            return false;
        }
        return true;
    }
    public override void OnCardEndComeBackDrag(Card _card)
    {
        Debug.Log("attack wait list end come back");
        //_card.transform.SetParent(_card.myDrag.myCanvas.transform);
        iTween.ScaleTo(_card.gameObject, iTween.Hash("scale", Vector3.one * 0.5f, "time", 1f, "delay", 0.5f));
    }

    public override Vector3 GetMyPosition(Card card)
    {
        int num = 0;
        for (int i = 0; i < myDropZone.draggables.Count; i++)
            if (myDropZone.draggables[i].GetComponent<Card>().instance_id == card.instance_id)
                num = i;
        return new Vector3(xposes()[num], 0, 0);
    }

    float[] xposes()
    {
        float[] _xposes = new float[myDropZone.draggables.Count];
        switch (_xposes.Length)
        {
            case 0:
                break;
            case 1:
                _xposes[0] = 0;
                break;
            case 2:
                _xposes[0] = -0.5f * space;
                _xposes[1] = 0.5f * space;
                break;
            case 3:
                _xposes[0] = -space;
                _xposes[1] = 0;
                _xposes[2] = space;
                break;
            case 4:
                _xposes[0] = -1.5f * space;
                _xposes[1] = -0.5f * space;
                _xposes[2] = 0.5f * space;
                _xposes[3] = 1.5f * space;
                break;
            case 5:
                _xposes[0] = -2 * space;
                _xposes[1] = -space;
                _xposes[2] = 0;
                _xposes[3] = space;
                _xposes[4] = 2 * space;
                break;
            case 6:
                _xposes[0] = -2.5f * space;
                _xposes[1] = -1.5f * space;
                _xposes[2] = -0.5f * space;
                _xposes[3] = 0.5f * space;
                _xposes[4] = 1.5f * space;
                _xposes[5] = 2.5f * space;
                break;
            case 7:
                _xposes[0] = -3 * space;
                _xposes[1] = -2 * space;
                _xposes[2] = -space;
                _xposes[3] = 0;
                _xposes[4] = space;
                _xposes[5] = 2 * space;
                _xposes[6] = 3 * space;
                break;
            case 8:
                _xposes[0] = -3.5f * space;
                _xposes[1] = -2.5f * space;
                _xposes[2] = -1.5f * space;
                _xposes[3] = -0.5f * space;
                _xposes[4] = 0.5f * space;
                _xposes[5] = 1.5f * space;
                _xposes[6] = 2.5f * space;
                _xposes[7] = 3.5f * space;
                break;
            case 9:
                _xposes[0] = -4 * space;
                _xposes[1] = -3 * space;
                _xposes[2] = -2 * space;
                _xposes[3] = space;
                _xposes[4] = 0;
                _xposes[5] = space;
                _xposes[6] = 2 * space;
                _xposes[7] = 3 * space;
                _xposes[8] = 4 * space;
                break;
            case 10:
                _xposes[0] = -4.5f * space;
                _xposes[1] = -3.5f * space;
                _xposes[2] = -2.5f * space;
                _xposes[3] = -1.5f * space;
                _xposes[4] = -0.5f * space;
                _xposes[5] = 0.5f * space;
                _xposes[6] = 1.5f * space;
                _xposes[7] = 2.5f * space;
                _xposes[8] = 3.5f * space;
                _xposes[9] = 4.5f * space;
                break;

        }
        return _xposes;
    }

    public override Vector3 GetMyRotation(Card card)
    {
        return new Vector3(0, 0, 0);
    }

    public override void OnCardEnter(Card _card)
    {
        PlaceCardOnTop(_card);
        StartCoroutine(Organize());
    }

    IEnumerator Organize()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < myDropZone.draggables.Count; i++)
            myDropZone.draggables[i].ComeBackToDropZone();
    }

    void PlaceCardOnTop(Card _card)
    {
        iTween.ScaleTo(_card.gameObject, iTween.Hash("scale", Vector3.one * 0.5f, "time", 1f, "delay", 0.5f));
        _card.transform.SetParent(_card.myDrag.myCanvas.transform);

        //Considering clock be on top
        if (GameMechanics.instance.clock.gameObject.activeSelf)
        {
            _card.transform.SetSiblingIndex(_card.myDrag.myCanvas.transform.childCount - 2);
        }
        else if (GameMechanics.instance.GetSelfPlayer.myBoard.myHand.GetComponent<PlayerHandCardSelector>().currentSelectedCard != null)
        {
            _card.transform.SetSiblingIndex(_card.myDrag.myCanvas.transform.childCount - 2);
        }
        else
        {
            _card.transform.SetAsLastSibling();
        }

    }


    public override void OnCardExit(Card _card)
    {
        Debug.Log("attack wait list card exit");
        iTween.ScaleTo(_card.gameObject, iTween.Hash("scale", Vector3.one, "time", 1f, "delay", 0f));
    }


    #endregion   


    #region AuX Checks

    bool isThereNone(Player targetPlayer)
    {
        return !isThereABarrier(myPlayer) && !isThereABasicDefense(myPlayer) && !isThereAWatchTower(myPlayer);
    }
    bool BasicDefenseAndWatchTower(Player targetPlayer)
    {
        return !isThereABarrier(targetPlayer) && isThereABasicDefense(targetPlayer) && isThereAWatchTower(targetPlayer);
    }
    bool BarrierAndBasicDefense(Player targetPlayer)
    {
        return isThereABarrier(targetPlayer) && isThereABasicDefense(targetPlayer) && !isThereAWatchTower(targetPlayer);
    }
    bool BarrierAndWatchTower(Player targetPlayer)
    {
        return isThereABarrier(targetPlayer) && !isThereABasicDefense(targetPlayer) && isThereAWatchTower(targetPlayer);
    }
    bool isThereBarrierDefenseWatchTower(Player targetPlayer)
    {
        return isThereABarrier(targetPlayer) && isThereABasicDefense(targetPlayer) && isThereAWatchTower(targetPlayer);
    }
    bool isThereOnlyBarrierAndDefense(Player targetPlayer)
    {
        return isThereABarrier(targetPlayer) && isThereABasicDefense(targetPlayer) && !isThereAWatchTower(targetPlayer);
    }
    bool isThereOnlyBasicDefense(Player targetPlayer)
    {
        return !isThereABarrier(targetPlayer) && isThereABasicDefense(targetPlayer) && !isThereAWatchTower(targetPlayer);
    }
    bool isThereOnlyBarrier(Player targetPlayer)
    {
        return isThereABarrier(targetPlayer) && !isThereABasicDefense(targetPlayer) && !isThereAWatchTower(targetPlayer);
    }
    bool isThereOnlyWatchTower(Player targetPlayer)
    {
        return !isThereABarrier(targetPlayer) && !isThereABasicDefense(targetPlayer) && isThereAWatchTower(targetPlayer);
    }
    bool isThereABasicDefense(Player targetPlayer)
    {
        return targetPlayer.myBoard.TryGetCardOnField<BasicDefense>() != null;
    }

    bool isThereAWatchTower(Player targetPlayer)
    {
        return targetPlayer.myBoard.TryGetCardOnField<WatchTower>() != null;
    }

    bool isThereABarrier(Player targetPlayer)
    {
        return targetPlayer.myBoard.TryGetCardOnField<Barrier>() != null;
    }

    bool isThereABash(Player targetPlayer)
    {
        return targetPlayer.myBoard.TryGetCardOnField<Bash>() != null;
    }

    #endregion

    #region DEFENSE


    public IEnumerator ProcessDefense()
    {
        if (cards.Count > 0)
        {
            Debug.Log(cards.Count + " Attack cards to Process Defense");

            yield return new WaitForSeconds(0.5f);
            //collect attack cards
            BasicAttack[] basicAttackCards = new BasicAttack[cards.Count];
            int i = 0;
            foreach (var card in cards)
            {
                basicAttackCards[i] = card as BasicAttack;
                i++;
            }
            yield return StartCoroutine(ProcessAttack(basicAttackCards));
        }
    }

    //TODO: this code should be at defense part of Game Class
    IEnumerator ProcessAttack(BasicAttack[] basicAttackCards)
    {
        //move card to middle
        Debug.Log("Processing basic Attack on " + myPlayer.gameObject.name);
        // iTween.MoveTo(basicAttackCard.gameObject, new Vector3(Screen.width/2f,Screen.height/2f,0),1f);
        //    iTween.ScaleTo(basicAttackCard.gameObject, iTween.Hash("scale", Vector3.one, "time", 1f, "delay", 0.5f));
        yield return new WaitForSeconds(0.5f);


        //3 defense options
        if (isThereBarrierDefenseWatchTower(myPlayer))
        {
            if (isTargetToWatchTowerRight || isTargetToWatchTowerLeft)
            {
                TryBlockAttackWithWatchTower(myPlayer, basicAttackCards);
            }
            else
            {
                TryBlockAttackWithBasicDefense(myPlayer, basicAttackCards);
            }
        }
        //2 defense option
        else if (BarrierAndWatchTower(myPlayer))
        {
            TryBlockAttackWithBarrier(myPlayer, basicAttackCards);
        }
        else if (BarrierAndBasicDefense(myPlayer))
        {
            TryBlockAttackWithBasicDefense(myPlayer, basicAttackCards);
        }
        else if (BasicDefenseAndWatchTower(myPlayer))
        {
            if (isTargetToWatchTowerRight || isTargetToWatchTowerLeft)
            {
                TryBlockAttackWithWatchTower(myPlayer, basicAttackCards);
            }
            else
            {
                TryBlockAttackWithBasicDefense(myPlayer, basicAttackCards);
            }

        }
        //1 defense option
        else if (isThereOnlyBarrier(myPlayer))
        {
            TryBlockAttackWithBarrier(myPlayer, basicAttackCards);
        }
        else if (isThereOnlyWatchTower(myPlayer))
        {
            if (isTargetToWatchTowerRight || isTargetToWatchTowerLeft)
            {
                TryBlockAttackWithWatchTower(myPlayer, basicAttackCards);
            }
            else
            {
                AttackPlayerDirectly(myPlayer, basicAttackCards);
            }
        }
        else if (isThereOnlyBasicDefense(myPlayer))
        {
            TryBlockAttackWithBasicDefense(myPlayer, basicAttackCards);
        }
        else if (isThereABash(myPlayer))
        {
            TryBlockAttackWithBash(myPlayer, basicAttackCards);
        }

        //0 options
        else if (isThereNone(myPlayer))
        {
            AttackPlayerDirectly(myPlayer, basicAttackCards);
        }
        else
        {
            Debug.Log("__Error case invalid, not found");
        }

        isTargetToWatchTowerRight = false;
        isTargetToWatchTowerLeft = false;

    }


    void AttackPlayerDirectly(Player targetPlayer, BasicAttack[] basicAttackCards)
    {
        Debug.Log("Attack Directly " + myPlayer.name);
        foreach (var basicAttackCard in basicAttackCards)
            basicAttackCard.myDrag.MoveToNewDropZone(targetPlayer.myBoard.stackAttackSlot.myDropZone, 2f);

        if (onAttackPlayerDirectly != null)
        {
            onAttackPlayerDirectly();
        }
    }

    bool TryBlockAttackWithBarrier(Player targetPlayer, BasicAttack[] basicAttackCards)
    {
        Barrier barrier = targetPlayer.myBoard.TryGetCardOnField<Barrier>();
        if (barrier == null)
        {
            Debug.Log("_______ Barrier not found");
            return false;
        }
        Debug.Log("Attack Blocked By Barrier");
        barrier.myCardGraphics.ShowFront();
        barrier.myDrag.MoveToNewDropZone(barrier.myPlayer.myBoard.usedCardsSlot.myDropZone, 2f);
        foreach (var basicAttackCard in basicAttackCards)
            basicAttackCard.myDrag.MoveToNewDropZone(basicAttackCard.myPlayer.myBoard.usedCardsSlot.myDropZone, 2f);

        if (onAttackBuilding != null)
        {
            onAttackBuilding();
        }
        //   if (onPlayerSucessfullyDefend != null)
        //       onPlayerSucessfullyDefend(targetPlayer);
        return true;
    }

    bool TryBlockAttackWithBasicDefense(Player targetPlayer, BasicAttack[] basicAttackCards)
    {
        BasicDefense basicDefense = targetPlayer.myBoard.TryGetCardOnField<BasicDefense>();
        if (basicDefense == null)
        {
            Debug.Log("basicDefense not found");
            return false;
        }
        Debug.Log("Attack Blocked By Basic Defense");

        basicDefense.myDrag.MoveToNewDropZone(basicDefense.myPlayer.myBoard.usedCardsSlot.myDropZone, 2f);
        foreach (var basicAttackCard in basicAttackCards)
            basicAttackCard.myDrag.MoveToNewDropZone(basicAttackCard.myPlayer.myBoard.usedCardsSlot.myDropZone, 2f);
        if (onPlayerSucessfullyDefend != null)
            onPlayerSucessfullyDefend(targetPlayer);

        return true;
    }

    bool TryBlockAttackWithBash(Player targetPlayer, BasicAttack[] basicAttackCards)
    {
        Bash bash = targetPlayer.myBoard.TryGetCardOnField<Bash>();
        if (bash == null)
        {
            Debug.Log("bash not found");
            return false;
        }
        Debug.Log("Attack Blocked By Bash");

        bash.myDrag.MoveToNewDropZone(bash.myPlayer.myBoard.usedCardsSlot.myDropZone, 2f);
        foreach (var basicAttackCard in basicAttackCards)
            basicAttackCard.myDrag.MoveToNewDropZone(basicAttackCard.myPlayer.myBoard.usedCardsSlot.myDropZone, 2f);
        if (onPlayerSucessfullyDefend != null)
            onPlayerSucessfullyDefend(targetPlayer);

        return true;
    }

    bool TryBlockAttackWithWatchTower(Player targetPlayer, BasicAttack[] basicAttackCards)
    {
        WatchTower watchTowerCard = null;
        if (isTargetToWatchTowerLeft)
        {
            watchTowerCard = targetPlayer.myBoard.specialBuildingCardSlotLeft.FindCardSpecefic<WatchTower>();
        }
        else if (isTargetToWatchTowerRight)
        {
            watchTowerCard = targetPlayer.myBoard.specialBuildingCardSlotRight.FindCardSpecefic<WatchTower>();
        }
        if (watchTowerCard == null)
        {
            Debug.Log("Attack wait list:  watchTowerCard not found to defend");
            return false;
        }
        Debug.Log("Attack Blocked By Watch Tower");
        watchTowerCard.stackAttack += basicAttackCards.Length;
        if (watchTowerCard.cardSlot != null)
        {
            if (watchTowerCard.cardSlot is SpecialBuildingSlot1)
            {
                isTargetToWatchTowerLeft = false;
            }
            else if (watchTowerCard.cardSlot is SpecialBuildingSlot2)
            {
                isTargetToWatchTowerRight = false;
            }
        }

        //throw away card? TODO: check this
        foreach (var basicAttackCard in basicAttackCards)
            basicAttackCard.myDrag.MoveToNewDropZone(basicAttackCard.myPlayer.myBoard.usedCardsSlot.myDropZone, 2f);

        if (onAttackBuilding != null)
        {
            onAttackBuilding();
        }
        //   if(onPlayerSucessfullyDefend!=null)
        //      onPlayerSucessfullyDefend(targetPlayer);
        return true;
    }

    #endregion

}

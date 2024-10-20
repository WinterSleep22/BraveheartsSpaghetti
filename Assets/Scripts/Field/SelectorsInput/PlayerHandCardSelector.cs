using UnityEngine;
using System.Collections;

public class PlayerHandCardSelector : HandCardSelector {
    public float offsetHeightMultiplierWhileSelected = 1.6f;
    public float zoomMultiplierWhileSelected = 2.5f;
    public Card currentSelectedCard;
    void Start()
    {
        GameMechanics.instance.turnManager.onEndTurn += OnEndTurn;
        StartCoroutine(CorrectingCards());
    }


    IEnumerator CorrectingCards()
    {
        for (; ; )
        {
            var cardWithWrongSize = myHand.cards.Find(c =>
                !c.isActivating &&
                myHand.myPlayer.currentDraggingCard != c &&
                c.transform.localScale != Vector3.one &&
                c != currentSelectedCard

                );

            if (cardWithWrongSize != null)
            {
                Debug.Log("Automatically change card size one");
                iTween.ScaleTo(cardWithWrongSize.gameObject, Vector3.one, 0.5f);
            }
            if (currentSelectedCard != null)
            {
                if (!TurnManager.isGamePaused)
                {
                    if (currentSelectedCard.transform.parent != GameMechanics.instance.myCanvas.transform)
                        currentSelectedCard.transform.SetParent(GameMechanics.instance.myCanvas.transform);
                    currentSelectedCard.transform.SetAsLastSibling();
                    Debug.Log("Making sure current Selected card is over all cards: " + currentSelectedCard);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void OnEndTurn(Player player)
    {
        if (player is SelfPlayer)
        {
            Debug.Log("SelfPlayer end turn: PlayerHandCardSelector canceling");
            CancelSelection();
            this.enabled = false;
        }
    }

    public override void OnClickedOut()
    {
        Debug.Log("PlayerHandCardSelector: Clicked outside cardhand");
        CancelSelection();

    }

    public override void OnCardClicked(Card card)
    {
        Debug.Log("PlayerHandCardSelector: Clicked inside a hand card=" + card.title);

        SelectCard(card);


    }

    void SelectCard(Card card)
    {
        if (card == currentSelectedCard)
        {
            Debug.Log("PlayerHandCardSelector: Select Card=" + card.title);
            //RevertChanges(currentSelectedCard, false);
            card.Activate();
            //call rpc

            currentSelectedCard = null;

            return;
        }

        if (currentSelectedCard != null)
        {
            RevertChanges(currentSelectedCard);
        }

        card.OnCardSelectedOnHand();
        currentSelectedCard = card;
        ApplyChanges(currentSelectedCard);

    }

    void CancelSelection()
    {
        if (currentSelectedCard != null)
        {
            RevertChanges(currentSelectedCard);
        }
        currentSelectedCard = null;
        myHand.myPlayer.myBoard.fieldArea.gameObject.SetActive(false);
        myHand.OrganizeCards();


    }


    void RevertChanges(Card card)
    {
        card.OnCardDeselectedOnHand();
        Debug.Log("PlayerHandCardSelector: Revertchanges card=" + card.title);


        StartCoroutine(myHand.OrganizeCardsIn(0.6f));
        card.myDrag.ComeBackToDropZone(0.5f);

        iTween.ScaleTo(card.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.5f));
        //  card.myCardGraphics.FadeAlpha(0.5f, 1, 1f);


        if (currentSelectedCard is AttackCard)
        {
            //if it is attack card, so open selection mode to target watchtower if desired
            GetComponent<CardSlotSelectorForAttack>().Cancel();
        }
    }

    void ApplyChanges(Card card)
    {
        Debug.Log("PlayerHandCardSelector:  Highlight Selection card=" + card.title);
        Vector3 cardPos = transform.position;
        cardPos += Vector3.up * card.myRectTransform.GetHeight() * offsetHeightMultiplierWhileSelected;
        //cardPos.x = Screen.width / 2f;
        iTween.MoveTo(card.gameObject, cardPos, 1f);
        iTween.RotateTo(card.gameObject, Vector3.zero, 1f);
        iTween.ScaleTo(card.gameObject, iTween.Hash("scale", Vector3.one * zoomMultiplierWhileSelected, "time", 0.5f));
        card.transform.SetParent(GameMechanics.instance.myCanvas.transform);
        card.transform.SetAsLastSibling();
        // card.myCardGraphics.FadeAlpha(1, 0.5f, 1f);
        StartCoroutine(TryMakingItChildOfCanvas(card));
    }

    IEnumerator TryMakingItChildOfCanvas(Card c)
    {
        while (true)
        {
            Debug.Log("makinit child: " + c.transform.name);
            c.transform.SetParent(GameMechanics.instance.myCanvas.transform);
            yield return null;
            if (c.transform.parent == GameMechanics.instance.myCanvas.transform)
                break;
        }
    }



}

using UnityEngine;
using System.Collections;

public class FlippedCardSlot : CardSlot {


    public override void Awake()
    {
        base.Awake();
    }

    public override void OnCardBeginComeBackDrag(Card card)
    {

    }

    public override void OnCardEndComeBackDrag(Card _card)
    {
        AdjustCardPosition(_card);
        AdjustCardRotation(_card);
        AdjustCardSize(_card);
        HideCard();
    }

    public override bool OnAllowCardBeDropped(Card card)
    {

        return true;
    }

    public override bool OnAllowCardAdd(Card card)
    {

        return card.isFlippable;
    }

    public override void OnCardExit(Card _card)
    {
        base.OnCardExit(_card);
    }

    public override void OnCardEnter(Card _card)
    {
        AdjustCardPosition(_card);
        AdjustCardRotation(_card);
        AdjustCardSize(_card);
        HideCard();

    }
    void AdjustCardSize(Card card)
    {
        iTween.ScaleTo(card.gameObject, iTween.Hash(
            "scale", Vector3.one,
            "time", 0.5f
            ));
    }
    void AdjustCardPosition(Card card)
    {
        iTween.MoveTo(card.gameObject, iTween.Hash(
            "position", Vector3.zero,
            "islocal", true,
            "time", 0.5f
            ));
    }
    void AdjustCardRotation(Card card)
    {
        iTween.RotateTo(card.gameObject, iTween.Hash(
            "rotation", Vector3.zero,
            "islocal", true,
            "time", 0.5f
            ));
    }



    public void HideCard()
    {
        if (cards.Count == 0)
        {
            Debug.Log("FlippedCardSlot.RevealCard: no cards to reveal");
            return;
        }
        if (myPlayer.otherPlayer.myBoard.TryGetCardOnField<WatchTower>() != null)
        {
            RevealCard();
            Debug.Log("Cant hide card, there is a WatchTower on other player's field");
            return;
        }
        if (cards[0].myCardGraphics.isFlipped)
        {
            Debug.Log("FlippedCardSlot.RevealCard: card was already hide");
            return;
        }
        cards[0].myCardGraphics.ShowBack();
    }




    public void RevealCard()
    {
        if (cards.Count == 0)
        {
            Debug.Log("FlippedCardSlot.RevealCard: no cards to reveal");
            return;
        }

        cards[0].myCardGraphics.ShowFrontOnField();
    }
    public IEnumerator ProcessRevealCardStartTurnPreAttack()
    {

        if (cards.Count == 1)
        {
            Card card = cards[0];
            if (card.title != "Bash") //manual turning from Bash.cs
                card.myCardGraphics.ShowFront();
            card.transform.SetParent(GameMechanics.instance.myCanvas.transform);
            card.transform.SetAsLastSibling();

            yield return StartCoroutine(card.OnCardRevealedStartTurnPreAttack());


        }
    }
    public IEnumerator ProcessRevealCardStartTurnPosAttack()
    {

        if (cards.Count == 1)
        {
            Card card = cards[0];
            if (card.title != "Bash") //manual turning from Bash.cs
                card.myCardGraphics.ShowFront();
            card.transform.SetParent(GameMechanics.instance.myCanvas.transform);
            card.transform.SetAsLastSibling();

            yield return StartCoroutine(card.OnCardRevealedStartTurnPosAttack());


        }
    }

}

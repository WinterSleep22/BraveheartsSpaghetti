using UnityEngine;
using System.Collections;

public class Satisfaction : EffectCard {

    [HideInInspector]
    public HelperCard alreadyChoosedCardToPay;
    private HelperCard choosedHelperCardOpponent;

    public void OnChooseHelperCard(HelperCard helperCard)
    {
        choosedHelperCardOpponent = helperCard;
    }

    public override IEnumerator OnCardRevealedStartTurnPosAttack()
    {
        Fighter fighter = myPlayer.myBoard.TryGetCardOnField<Fighter>();
        bool isThereAHelperCardOnOpponentField = myPlayer.otherPlayer.myBoard.TryGetCardOnField<HelperCard>() != null;
        if (isThereAHelperCardOnOpponentField)
        {
            if (fighter != null)
            {
                if (myCardGraphics.isShowingBack)
                {
                    myCardGraphics.ShowFront();
                    yield return new WaitForSeconds(1f);
                }
                yield return StartCoroutine(ActivitingSatisfaction());
            }
            else
            {
                Debug.Log(" Satisfaction effect on card reveadled:  fighter not found on my board!");
                myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
            }
        }
        else
        {
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("There’s no person card to choose");
            Debug.Log(" Satisfaction effect on card reveadled:  helper not found on opponent board!");
            myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
        }

    }

    bool IsChoosedHelperCardAllowed(HelperCard helperCard)
    {
        if (helperCard == null)
            return false;

        if (helperCard.myPlayer == this.myPlayer)
        {
            Debug.Log("opponent helper card is not allowed, own card");
            return false;
        }
        if (helperCard.GetComponentInParent<CardSlot>() is HelperCardSlot)
            return true;

        Debug.Log("choosed helper card isnt in helper card slot");
        return false;
    }


    public override bool IsAllowedToActivate()
    {
        bool isThereAHelperCardOnOpponentField = myPlayer.otherPlayer.myBoard.TryGetCardOnField<HelperCard>() != null;
        if (myCardGraphics.isShowingFront && !isThereAHelperCardOnOpponentField)
        {
            Debug.Log("Cant use " + title + ", no Helper Cards on opponent's field");
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("There’s no person card to choose");
            return false;
        }

        Fighter myfighter = myPlayer.myBoard.TryGetCardOnField<Fighter>();
        bool isThereAFighterOnMyField = myfighter != null;
        if (!isThereAFighterOnMyField)
        {

            Debug.Log("Cant use " + title + ", needs Fighter placed on field");
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("Need a " + CardManager.GetCardOfType<Fighter>().title + " on the field");

            return false;
        }


        if (myCardGraphics.isShowingFront && myfighter.myCardGraphics.isShowingBack)
        {
            Debug.Log("Cant use " + title + " flipped, Fighter needs to be revealed");
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("Need a revealed " + CardManager.GetCardOfType<Fighter>().title + " card");
            return false;
        }
        return base.IsAllowedToActivate();
    }


    protected override IEnumerator Activating()
    {
        if (IsAllowedToActivate())
            yield return StartCoroutine(ActivitingSatisfaction());
        else OnActivatingEnd();
    }

    IEnumerator ChoosingOpponentHelperCard()
    {
        Debug.Log("Satisfaction: Choose Opponent Helper Card");
        LogTextUI.Log("Choose a helper card");
        MoveToCenter();

        if (myPlayer is SelfPlayer)
        {
            Debug.Log("Turn on CArd Slot Selector for Satisfaction");
            CardSlotSelectorForSatisfaction selector = myPlayer.myBoard.myHand.GetOrAddComponent<CardSlotSelectorForSatisfaction>();
            selector.InitializeHelperCardSelection(this, new CardSlot[] { myPlayer.otherPlayer.myBoard.helperCardSlotLeft, myPlayer.otherPlayer.myBoard.helperCardSlotMiddle, myPlayer.otherPlayer.myBoard.helperCardSlotRight });
        }

        float maxTime = 20;
        float currentTime = 0;
        while (!IsChoosedHelperCardAllowed(choosedHelperCardOpponent))
        {
            currentTime += 1f;
            if (currentTime > maxTime)
            {
                Debug.Log("SAtisfaction: canceling selection helper card choosed");
                break;
            }

            Debug.Log("Satisfaction :Wait player choose helper card of his opponent");
            yield return new WaitForSeconds(1f);
        }

    }
    IEnumerator ActivitingSatisfaction()
    {
        Fighter myFighter = myPlayer.myBoard.TryGetCardOnField<Fighter>();

        if (myCardGraphics.isShowingFront)
        {
            Debug.Log("Satisfaction: front showing using");
            yield return StartCoroutine(ChoosingOpponentHelperCard());
            yield return StartCoroutine(ActivitingSatisfactionEffect(myFighter, choosedHelperCardOpponent));
        }
        else
        {
            Debug.Log("Satisfaction: back showing, use as flipped card");
            myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
            OnActivatingEnd(new Action(instance_id, true));
        }
    }

    public IEnumerator ActivitingSatisfactionEffect(Fighter myFighter, HelperCard opponentHelperCard)
    {
        Debug.Log("Satisfaction Effect");

        MoveToCenter();

        //move cards to be choosed to middle 

        myFighter.MoveToCenter(1, 0.5f, false);

        opponentHelperCard.MoveToCenter(1, 0.5f, false);

        Debug.Log("Flipping a Coin");
        //TODO: show coin graphics

        yield return new WaitForSeconds(1f);


        PersonCard personCardChoosed;
        PersonCard personCardNotChoosed;


        if (alreadyChoosedCardToPay == null) //choose randomly 
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                personCardChoosed = myFighter;
                personCardNotChoosed = choosedHelperCardOpponent;
            }
            else
            {
                personCardChoosed = choosedHelperCardOpponent;
                personCardNotChoosed = myFighter;
            }
        }
        else// or the choose is already made up, remotly networked
        {
            Debug.Log("Satisfaction Choosed remotely card to be discarded");
            personCardChoosed = alreadyChoosedCardToPay;
            personCardNotChoosed = (myFighter != alreadyChoosedCardToPay) ? myFighter : choosedHelperCardOpponent;
        }

        if (personCardChoosed.myPlayer is SelfPlayer)
            LogTextUI.Log("The opponent wins the " + CardManager.GetCardOfType<Satisfaction>().title);
        else LogTextUI.Log("You win the " + CardManager.GetCardOfType<Satisfaction>().title);

        Debug.Log("Card to be removed is " + personCardChoosed.title + " " + ((personCardChoosed.myPlayer is SelfPlayer) ? ":(" : ":)"));

        personCardChoosed.myDrag.MoveToNewDropZone(personCardChoosed.myPlayer.myBoard.usedCardsSlot.myDropZone);
        Debug.Log("personCardNotChoosed=" + personCardNotChoosed.title + " come back to drop zone=" + personCardNotChoosed.myDrag.myDropZone);
        personCardNotChoosed.myDrag.ComeBackToDropZone();

        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);

        //target cards are myfighter and choosed helper opponent card
        //pay card is the card choosen by this effect
        OnActivatingEnd(new Action(instance_id, false, new int[] { myFighter.instance_id, choosedHelperCardOpponent.instance_id }, new int[] { personCardChoosed.instance_id }));

        alreadyChoosedCardToPay = null;
        choosedHelperCardOpponent = null;

        Debug.Log("Satisfaction to Used Deck");
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
        yield return new WaitForSeconds(2f);
    }
}

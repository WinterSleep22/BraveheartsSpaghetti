
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;



/// <summary>
/// mostly for visual mechanics, aux functions and keep reference of some objects
/// </summary>
public class GameMechanics : MonoBehaviour {

    private static GameMechanics _instance;
    /// <summary>
    /// Singleton
    /// </summary>
    public static GameMechanics instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameMechanics>();
            }
            return _instance;
        }
    }






    ResultWinMenu _winMenu;
    public ResultWinMenu winMenu
    {
        get
        {
            if (_winMenu == null)
            {
                _winMenu = transform.parent.GetComponentInChildren<ResultWinMenu>(true);
            }
            return _winMenu;
        }
    }


    ResultDefeatedMenu _loseMenu;
    public ResultDefeatedMenu loseMenu
    {
        get
        {
            if (_loseMenu == null)
            {
                _loseMenu = transform.parent.GetComponentInChildren<ResultDefeatedMenu>(true);
            }
            return _loseMenu;
        }
    }

    private Player _selfPlayer;
    public Player selfPlayer
    {
        get
        {
            if (_selfPlayer == null)
            {
                _selfPlayer = FindObjectOfType<SelfPlayer>();
            }
            return _selfPlayer;
        }
    }
    public SelfPlayer GetSelfPlayer
    {
        get
        {
            if (_selfPlayer == null)
            {
                _selfPlayer = FindObjectOfType<SelfPlayer>();
            }
            return _selfPlayer as SelfPlayer;
        }
    }

    private Player _opponentPlayer;
    public Player opponentPlayer
    {
        get
        {
            if (_opponentPlayer == null)
            {
                _opponentPlayer = FindObjectOfType<OpponentPlayer>();
            }
            return _opponentPlayer;
        }
    }
    public OpponentPlayer GetOpponentPlayer
    {
        get
        {
            if (_opponentPlayer == null)
            {
                _opponentPlayer = FindObjectOfType<OpponentPlayer>();
            }
            return _opponentPlayer as OpponentPlayer;
        }
    }

    public Player masterClientPlayer
    {
        get
        {
            if (Gameplay.instance.IsServerGame)
            {
                return selfPlayer;

            }
            else
            {
                return opponentPlayer;
            }
        }
    }

    Clock _clock;
    public Clock clock
    {
        get
        {
            if (_clock == null)
            {
                _clock = GetComponentInChildren<Clock>(true);
            }
            return _clock;
        }
    }

    Canvas _mycanvas;
    public Canvas myCanvas
    {
        get
        {
            if (_mycanvas == null)
            {
                _mycanvas = transform.parent.GetComponent<Canvas>();
            }
            return _mycanvas;
        }
    }


    GraphicRaycaster _grCanvas;
    public GraphicRaycaster grCanvas
    {
        get
        {
            if (_grCanvas == null)
            {
                _grCanvas = transform.parent.GetComponent<GraphicRaycaster>();
            }
            return _grCanvas;
        }
    }



    private TurnManager _turnManager;
    public TurnManager turnManager
    {
        get
        {
            if (_turnManager == null)
            {
                _turnManager = GetComponent<TurnManager>();
            }
            return _turnManager;
        }
    }


    private LoadingScreen _loadingScreen;
    public LoadingScreen loadingScreen
    {
        get
        {
            if (_loadingScreen == null)
            {
                _loadingScreen = FindObjectOfType<LoadingScreen>();
            }
            return _loadingScreen;
        }
    }

    PingText _pingText;
    public PingText pingText
    {
        get
        {
            if (_pingText == null)
            {
                _pingText = FindObjectOfType<PingText>();
            }
            return _pingText;
        }
    }

    private CardManager _cardLoader;
    public CardManager cardLoader
    {
        get
        {
            if (_cardLoader == null)
            {
                _cardLoader = FindObjectOfType<CardManager>();
            }
            return _cardLoader;
        }
    }

    public delegate void OnPlayerDrawCardDueStartTurn(Player player);
    public event OnPlayerDrawCardDueStartTurn onPlayerDrawCardDueStartTurn;

    public delegate IEnumerator OnPlayerRevealingFlippedCardDueStartTurn(Player player);
    public event OnPlayerRevealingFlippedCardDueStartTurn onPlayerRevealingFlippedCardDueStartTurn;

    public delegate void OnPlayerRevealFlippedCardDueStartTurn(Player player);
    public event OnPlayerRevealFlippedCardDueStartTurn onPlayerRevealFlippedCardDueStartTurn;

    void Start()
    {
        AllowPlayerInput(false);
        turnManager.onBeginLastSeconds += OnBeginLastSeconds;
        turnManager.onEndLastSeconds += OnEndLastSeconds;
        turnManager.onBeginTurn += OnBeginTurn;
        turnManager.onEndTurn += OnEndTurn;

    }




    void OnBeginTurn(Player player)
    {
        if (player is SelfPlayer)
        {
            player.myBoard.flippedCardSlot.HideCard();
            AllowPlayerInput(true);
        }
        else
        {
            AllowPlayerInput(false);
        }
    }

    void OnEndTurn(Player player)
    {
        if (player is SelfPlayer)
        {
            player.myBoard.flippedCardSlot.HideCard();
            AllowPlayerInput(false);
            clock.gameObject.SetActive(false);
        }
        else
        {
            //AllowPlayerInput(true);
        }
    }

    void OnBeginLastSeconds(float lastSeconds)
    {
        if (turnManager.currentPlayer is SelfPlayer)
        {
            clock.gameObject.SetActive(true);
        }
    }

    void OnEndLastSeconds(float seconds)
    {
        if (turnManager.currentPlayer is SelfPlayer)
        {
            clock.gameObject.SetActive(false);

        }
    }

    public void ShowResultMenu(bool isWinner)
    {
        if (isWinner)
        {
            winMenu.Show();
        }
        else
        {
            loseMenu.Show();
        }
    }



    public void AllowPlayerInput(bool allow)
    {
        (selfPlayer as SelfPlayer).SetPlayerInput(allow);
    }


    public IEnumerator PreparingTurn(Player player)
    {
        yield return StartCoroutine(PreProcessPlayerTurn(player));
        yield return StartCoroutine(BeginingTurnIn(player, 1f));

    }
    public IEnumerator BeginingTurnIn(Player player, float time)
    {
        yield return new WaitForSeconds(time);
        turnManager.BeginTurn(player);
    }

    public IEnumerator PreProcessPlayerTurn(Player player)
    {
        //   Debug.Log("Pre-Process Player " + player.gameObject.name);

        yield return StartCoroutine(RevealHelperCards(player));
        //TODO: BASH (not activating on beginning of round), PROVISION (should be affected only by Attack card)


        //pre attack
        yield return StartCoroutine(player.myBoard.flippedCardSlot.ProcessRevealCardStartTurnPreAttack());

        if (onPlayerRevealFlippedCardDueStartTurn != null)
        {
            onPlayerRevealFlippedCardDueStartTurn(player);
        }

        if (onPlayerRevealingFlippedCardDueStartTurn != null)
        {
            yield return StartCoroutine(onPlayerRevealingFlippedCardDueStartTurn(player));
        }


        //process attack and defense
        yield return StartCoroutine(player.myBoard.attackWaitListSlot.ProcessDefense());


        // pos attack
        yield return StartCoroutine(player.myBoard.flippedCardSlot.ProcessRevealCardStartTurnPosAttack());

        //flipped card to used deck
        if (player.myBoard.flippedCardSlot.cards.Count > 0)
        {

            //  Debug.Log("Move flipped card to used cards");
            player.myBoard.flippedCardSlot.cards[0].myDrag.MoveToNewDropZone(player.myBoard.usedCardsSlot.myDropZone);
            yield return new WaitForSeconds(1f);

        }

        //normal draw card
        if (player.myBoard.myHand.cards.Count < 11)
        {
            //  Debug.Log(player.gameObject.name + " Draw +1 Card due his turn begins");
            player.DrawTopCardFromMyDeck();
        }
        else
        {
            Debug.Log(player.gameObject.name + " Cant Draw: max of 11 cards reached");
        }
        if (onPlayerDrawCardDueStartTurn != null)
            onPlayerDrawCardDueStartTurn(player);



    }



    IEnumerator RevealHelperCards(Player player)
    {
        player.myBoard.helperCardSlotLeft.RevealCard();
        player.myBoard.helperCardSlotMiddle.RevealCard();
        player.myBoard.helperCardSlotRight.RevealCard();
        yield return new WaitForSeconds(1f);


    }



    public IEnumerator DrawingInitialCards()
    {

        int cardsToDraw = 6;//6
        float timeEachCard = 0.5f;
        Debug.Log("Drawing Cards");

        //General card draw and place on field
        AdjustHeroCardFromDeck(selfPlayer, 0.01f);
        AdjustHeroCardFromDeck(opponentPlayer, 0.01f);



        yield return new WaitForSeconds(timeEachCard);

        for (int i = 0; i < cardsToDraw; i++)
        {

            if (drawTestCards)
                SwitchForTests(i, timeEachCard);
            else
                DrawFromDeckBothPlayers(timeEachCard);

            yield return new WaitForSeconds(timeEachCard);
        }

    }

    public bool drawTestCards = true;
    void AdjustHeroCardFromDeck(Player player, float timeTransition)
    {
        HeroCard card = player.myBoard.myDeck.cards.Find(c => c is HeroCard) as HeroCard;
        if (card == null)
        {
            Debug.Log("_____ Failed to Draw one specific Card from deck");
        }

        card.myDrag.MoveToNewDropZone(player.myBoard.heroCardSlot.myDropZone, timeTransition);
    }
    void DrawFromDeckBothPlayers<T>(float timeTransition) where T : Card
    {
        selfPlayer.DrawCardFromMyDeck<T>(timeTransition);
        opponentPlayer.DrawCardFromMyDeck<T>(timeTransition);
    }

    void DrawFromDeckBothPlayers(float timeTransition)
    {
        selfPlayer.DrawTopCardFromMyDeck(timeTransition);
        opponentPlayer.DrawTopCardFromMyDeck(timeTransition);
    }

    void SwitchForTests(int i, float timeEachCard)
    {
        //switch for tests
        switch (i)
        {
            case 0: DrawFromDeckBothPlayers<Bash>(timeEachCard); break;
            case 1: DrawFromDeckBothPlayers<Vanguard>(timeEachCard); break;
            case 2: DrawFromDeckBothPlayers<Supplier>(timeEachCard); break;
            case 3: DrawFromDeckBothPlayers<Provision>(timeEachCard); break;
            case 4: DrawFromDeckBothPlayers<Meal>(timeEachCard); break;
            case 5: DrawFromDeckBothPlayers<Charge>(timeEachCard); break;
            case 6: DrawFromDeckBothPlayers<Quarrying>(timeEachCard); break;
            case 7: DrawFromDeckBothPlayers<BasicAttack>(timeEachCard); break;
            case 8: DrawFromDeckBothPlayers<Barrier>(timeEachCard); break;
            case 9: DrawFromDeckBothPlayers(timeEachCard); break;
            case 10: DrawFromDeckBothPlayers(timeEachCard); break;
            default: DrawFromDeckBothPlayers(timeEachCard); break;

        }
    }


}

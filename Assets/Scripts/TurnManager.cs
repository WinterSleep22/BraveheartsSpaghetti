using UnityEngine;
using System.Collections;

public enum PenaltyLateTurn {
    Level0 = 0,
    Level1,
    Level2,
    Level3,
    Level4,
}

/// <summary>
/// Offers basic turn management for Game class
/// </summary>
public class TurnManager : MonoBehaviour {

    public bool isTurnRunning = false;
    public bool gotMaxTimeturn = false;
    public Player lastPlayer;

    public float currentSecondsTurn = 0;
    public float currentSecondsTurnLeft
    {
        get
        {
            if (currentPlayer == null)
                return 0;
            float leftSeconds = currentPlayer.status.maxSecsTurnTime - currentSecondsTurn;
            if (gotMaxTimeturn)
            {
                return leftSeconds + 7;
            }
            return leftSeconds;
        }
    }

    public int turnCount = 0;

    public Player currentPlayer;

    private IEnumerator coroutineLastSeconds;

    private Gameplay _myGame;
    public Gameplay myGame
    {
        get
        {
            if (_myGame == null)
            {
                _myGame = GetComponent<Gameplay>();
            }
            return _myGame;
        }
    }

    public delegate void OnBeginTurn(Player player);
    public delegate void OnEndTurn(Player player);
    public delegate void OnBeginLastSeconds(float seconds);
    public delegate void OnEndLastSeconds(float seconds);
    public OnEndLastSeconds onEndLastSeconds;
    public OnBeginLastSeconds onBeginLastSeconds;
    public OnBeginTurn onBeginTurn;
    public OnEndTurn onEndTurn;


    void Update()
    {
        paused = isGamePaused;
        if (!isGamePaused)
            if (isTurnRunning)
            {
                currentSecondsTurn += Time.deltaTime;
                if (currentSecondsTurn > currentPlayer.status.maxSecsTurnTime)
                {
                    if (!gotMaxTimeturn)
                    {
                        coroutineLastSeconds = CountTheLastSeconds();
                        StartCoroutine(coroutineLastSeconds);
                    }
                    gotMaxTimeturn = true;

                }
                else
                {
                    gotMaxTimeturn = false;
                }

            }
    }

    void Start()
    {
        StartCoroutine(CheckForBugInPausing());
    }

    IEnumerator CheckForBugInPausing()
    {
        while (true)
        {
            if (isGamePaused)
            {
                print("game is paused");
                yield return new WaitForSecondsRealtime(0.6f);
                if (!mdg.isShown)
                {
                    print("unpausing");
                    yield return new WaitForSecondsRealtime(0.6f);
                    Unpause();
                }
            }
            yield return null;
        }
    }

    IEnumerator CountTheLastSeconds()
    {
        float lastSecondsAmount = currentPlayer.status.maxLastSecsTurnTime;
        if (onBeginLastSeconds != null)
        {
            onBeginLastSeconds(lastSecondsAmount);
        }
        yield return new WaitForSeconds(lastSecondsAmount);

        //turn could be ended in these 7 secdonds
        if (isTurnRunning)
        {
            if (onEndLastSeconds != null)
            {
                onEndLastSeconds(lastSecondsAmount);
            }
            EndTurn(currentPlayer);
        }

    }




    void OnTurnBegin(Player player)
    {

        if (onBeginTurn != null)
            onBeginTurn(player);
        lastPlayer = player;
    }
    void OnTurnEnd(Player player)
    {
        if (onEndTurn != null)
            onEndTurn(player);
        lastPlayer = player;
    }


    public void EndTurn(Player player)
    {
        if (currentPlayer == null)
        {
            Debug.Log("TurnManager: player cant end turn, no turn running ");
            return;
        }
        if (player != currentPlayer)
        {
            Debug.Log("TurnManager: player cant end turn, its turn is = " + currentPlayer.gameObject.name);
            return;
        }
        if (coroutineLastSeconds != null)
        {
            Debug.Log("interrupting countdown");
            StopCoroutine(coroutineLastSeconds);

        }
        isTurnRunning = false;
        currentPlayer = null;
        currentSecondsTurn = 0;
        OnTurnEnd(player);
        turnCount++;

    }

    public void BeginTurn(Player player)
    {
        if (currentPlayer != null || isTurnRunning)
        {
            Debug.Log("TurnManager: player cant begin turn, a turn is already running ");
            return;
        }
        isTurnRunning = true;
        currentPlayer = player;
        currentSecondsTurn = 0;

        OnTurnBegin(currentPlayer);
    }

    Player myPlayer
    {
        get
        {
            return GameMechanics.instance.selfPlayer;
        }
    }

    public MenuDuringGame mdg;
    public static bool isGamePaused { get; private set; }
    [SerializeField] bool paused;

    public void Pause()
    {
        if (isGamePaused)
            return;
        if (!Gameplay.IsSinglePlayerMode)
        {
            Debug.Log("Can't pause; not in singleplayer mode!");
            if (mdg == null)
                mdg = GameObject.FindObjectOfType<MenuDuringGame>();
            mdg._Show();
            return;
        }

        if (mdg == null)
            mdg = GameObject.FindObjectOfType<MenuDuringGame>();
        StartCoroutine(PauseGame());
    }

    public void Unpause()
    {
        if (!isGamePaused)
            return;
        if (mdg == null)
            mdg = GameObject.FindObjectOfType<MenuDuringGame>();
        StartCoroutine(UnpauseGame());
    }


    IEnumerator PauseGame()
    {
        Debug.Log("!Pausing");
        Time.timeScale = 1;
        (myPlayer as SelfPlayer).exitGameButton.interactable = false;

        yield return mdg._Show();
        while (currentPlayer == null)
            yield return null;
        isGamePaused = true;
        isTurnRunning = false;
        Time.timeScale = 0;

        if (GameMechanics.instance.turnManager.currentPlayer is SelfPlayer)
        {
            Debug.Log("!Disabling input");
            (myPlayer as SelfPlayer).SetBlockAllInput(true);
            (myPlayer as SelfPlayer).SetBlockSelfInput(true);
            (myPlayer as SelfPlayer).SetAllowDrag(false);
            (myPlayer as SelfPlayer).endTurnButton.interactable = false;
            myPlayer.myBoard.myHand.GetComponent<PlayerHandCardSelector>().enabled = false;
            myPlayer.myBoard.myHand.GetComponent<PlayerHandCardPaySelector>().enabled = false;
            myPlayer.myBoard.myHand.GetComponent<CardSlotSelectorForAttack>().enabled = false;
            myPlayer.myBoard.myHand.GetComponent<CardSlotSelectorForSatisfaction>().enabled = false;
            myPlayer.myBoard.myHand.GetComponent<PlayerHandCardEffectPaySelector>().enabled = false;
        }

        while (mdg.isShown)
        {
            mdg.transform.SetAsLastSibling();
            yield return null;
        }
    }

    IEnumerator UnpauseGame()
    {
        Debug.Log("!Unpausing");

        Time.timeScale = 1;
        if (currentPlayer != null)
            isTurnRunning = true;
        while (currentPlayer == null)
        {
            Debug.Log("null player, waiting to unpause");
            yield return null;
        }

        isGamePaused = false;
        isTurnRunning = true;
        (myPlayer as SelfPlayer).exitGameButton.interactable = true;

        if (currentPlayer is SelfPlayer)
        {
            Debug.Log("!Enabling input");
            (myPlayer as SelfPlayer).endTurnButton.interactable = true;
            (myPlayer as SelfPlayer).SetBlockAllInput(false);
            (myPlayer as SelfPlayer).SetBlockSelfInput(false);
            (myPlayer as SelfPlayer).SetAllowDrag(true);

            myPlayer.myBoard.myHand.GetComponent<PlayerHandCardSelector>().enabled = true;
        }
    }

    public void OnApplicationFocus(bool focus)
    {
        if (!focus)
            mdg.Show();
    }
}

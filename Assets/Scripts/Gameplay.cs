using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.SceneManagement;
public enum GameplayMode
{
    Wifi,
    Multiplayer,
    SinglePlayer
}



/// <summary>
/// manages the game, network logics, references,gameplay callbacks
/// </summary>
public class Gameplay : MonoBehaviour {


    
  
    
    private static Gameplay _instance;
    public static Gameplay instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Gameplay>();
            }
            return _instance;
        }
    }

	public PhotonLogLevel photonLogLevel = PhotonLogLevel.Informational;

    public static bool IsWifiMode
    {
        get
        {
            return WifiNetworkManager.instance != null && PhotonNetwork.offlineMode;
        }
    }
    public static bool IsMultiplayerMode
    {
        get
        {
            return (PhotonNetwork.connected && WifiNetworkManager.instance==null && !PhotonNetwork.offlineMode);
        }
    }
    
    public static bool IsSinglePlayerMode
    {
        get
        {
            return PhotonNetwork.offlineMode && WifiNetworkManager.instance == null;
        }
    }

    public bool IsServerGame
    {
        get
        {
            if (WifiNetworkManager.instance == null)
            {
                return PhotonNetwork.isMasterClient;
            }
            else
            {
                return NetworkServer.active;
            }
        }
    }


    public static bool HasGameFinished { get { return Gameplay.instance.winner != null; } }

    public static GameplayMode initialGameplayMode;

    public static GameplayMode currentGameplayMode
    {
        get
        {
            if(IsSinglePlayerMode){
                return GameplayMode.SinglePlayer;
            }
            else if(IsMultiplayerMode)
            {
                return GameplayMode.Multiplayer; 
            }
            else if(IsWifiMode){
                return GameplayMode.Wifi;
            }
            Debug.Log("__________________Error mode undetected! ");
            return GameplayMode.SinglePlayer;
        }
    }

    PlayerNetwork _selfPlayerNetwork;
    public PlayerNetwork selfPlayerNetwork
    {
        get
        {
            if (_selfPlayerNetwork==null)
            {
                _selfPlayerNetwork = FindObjectsOfType<PlayerNetwork>().ToList().Find(p=>p.myPlayer is SelfPlayer);
                
            }
            return _selfPlayerNetwork;
            
        }
    }
   
    
    PlayerNetwork _opponentPlayerNetwork;
    public PlayerNetwork opponentPlayerNetwork
    {
        get
        {
            if (_opponentPlayerNetwork == null)
            {
                _opponentPlayerNetwork = FindObjectsOfType<PlayerNetwork>().ToList().Find(p => p.myPlayer is OpponentPlayer);
            }
            return _opponentPlayerNetwork;

        }
    }
    

    public PlayerNetwork GetPlayerNetwork(Player player)
    {
        if(player is SelfPlayer){
            return selfPlayerNetwork;
        }
        else
        {
            return opponentPlayerNetwork;
        }
    }

    public delegate void OnBothDeckIitialized();
    public event OnBothDeckIitialized onBothDeckIitialized; 


    #region Mono

    void Awake()
    {
       PhotonNetwork.logLevel = photonLogLevel;
       //GameMechanics.instance.pingText.gameObject.SetActive(PhotonNetwork.connected);
       if(PhotonNetwork.inRoom==false){
           Debug.Log("Photon not connected: entering offline mode ");
           PhotonNetwork.offlineMode = true;
           PhotonNetwork.CreateRoom("MyRoom");
           
       }
       
       if(IsServerGame){
           Debug.Log("Server running");
       }
       else
       {
           Debug.Log("Client running");
       }
      
        
    }



    void Start()
    {
        if (WifiNetworkManager.instance == null)
        {
            Debug.Log("photon players count=" + PhotonNetwork.playerList.Length);
        }

        AssignCallbacks();

     
        initialGameplayMode = currentGameplayMode;

        Debug.Log("######### read initial gameplay mode as " + initialGameplayMode);
        
        StartCoroutine(StartingGame());


    }

    


    #endregion

  
  
    
    void CreateNetworkPlayer(bool opponentLocal=false)
    {
        Debug.Log("Gameplay.CreateNetworkPlayer");
        GameObject playerNetworkPrefab = Resources.Load<GameObject>("Prefabs/Gameplay/Resources/PlayerNetwork");
        GameObject newPlayerGameObject = null;
        if(WifiNetworkManager.instance==null){
            newPlayerGameObject = PhotonNetwork.Instantiate(playerNetworkPrefab.gameObject.name, Vector3.zero, Quaternion.identity, 0);
            if(opponentLocal){
                newPlayerGameObject.GetComponent<PlayerNetwork>().OpponentLocal = true;
            }
        }
        else
        {
            //CmdSpawnPlayerNetwork();
        }
    }

   
  

    #region Callbacks GAmeplay

    void AssignCallbacks()
    {
     
        GameMechanics.instance.selfPlayer.myBoard.stackAttackSlot.onReachMaxCallback += OnSelfPlayerStackReachedMax;
        GameMechanics.instance.opponentPlayer.myBoard.stackAttackSlot.onReachMaxCallback += OnOpponentStackRechedMax;
        GameMechanics.instance.turnManager.onBeginTurn += OnStartTurnCallback;
        GameMechanics.instance.turnManager.onEndTurn += OnEndTurnCallback;
        GameMechanics.instance.turnManager.onEndLastSeconds += OnEndLastSeconds;

        // building slot callback
        CardSlot[] cardSlots =  Object.FindObjectsOfType<CardSlot>();
        foreach(var cardSlot in cardSlots){
            if (cardSlot is BuildingCardSlot)
            {
                cardSlot.onCardEnterCallback += OnCardEnterBuildingSlot;
            }
            if( cardSlot is HelperCardSlot || cardSlot is HeroCardSlot ){
                cardSlot.onCardEnterCallback += OnCardEnterPersonSlot;
            }
        }
    }

    void OnCardEnterBuildingSlot(CardSlot cardSlot,Card card)
    {
        if(card is NormalBuildingCard){
            cardSlot.myPlayer.status.normalBuildingCardSetInThisTurn = true;    
        }
        if(card is SpecialBuildingCard){
            cardSlot.myPlayer.status.specialBuildingCardSetInThisturn = true;
        }
    }

    void OnCardEnterPersonSlot(CardSlot cardSlot, Card card)
    {
       cardSlot.myPlayer.status.personCardSetInThisTurn = true;
    }

  

    #endregion 



    #region GameStart
    
    public delegate void OnGameStart();
    public OnGameStart onGameStart;


    IEnumerator GeneratingPlayers()
    {
        if (selfPlayerNetwork == null)
        {
            Debug.Log("Create my SelfplayerNetwork");
            CreateNetworkPlayer();
        }
        else
        {
            Debug.Log("Didnt create player network, already exist =" + selfPlayerNetwork.gameObject.name);
        }

        float maxTimeWaiting = 30;
        float currentWaitingTime = 0;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            currentWaitingTime += 1;
            
            if(currentWaitingTime > maxTimeWaiting){
                Debug.Log("waiting time reached in waiiting oponent, loading main menu");
                SceneManager.LoadScene("MainMenu");
            }

            
            
            if (opponentPlayerNetwork ==null)
                Debug.Log("Gameplay.PreparingStartGame: waiting opponentPlayerNetwork");

            if (selfPlayerNetwork == null)
                Debug.Log("Gameplay.PreparingStartGame: waiting selfPlayerNetwork");
         
            //single player
            if (PhotonNetwork.offlineMode && WifiNetworkManager.instance==null)
            {
                Debug.Log("Gameplay.PreparingStartGame: offline mode, opponentPlayerNetwork instantiated locally");
                CreateNetworkPlayer(true);
                if(!PlayerInputControl.instance.showPanelOnStartTurn){
                    GameMechanics.instance.opponentPlayer.GetOrAddComponent<CardAI>().enabled = true;
                }
                
  
                break;
            
            }
           

            if (opponentPlayerNetwork != null && selfPlayerNetwork!=null)
            {
                Debug.Log("Gameplay.PreparingStartGame: opponentPlayerNetwork instantiated");
                break;
            }
        }
        yield return new WaitForSeconds(1f);

    }   

    IEnumerator StartingGame()
    {
        yield return StartCoroutine(GeneratingPlayers());

        Debug.Log("Start Game");

        winner = null;

        yield return StartCoroutine(InitializingDecks());

        if (onBothDeckIitialized!=null)
        {
            onBothDeckIitialized();
        }
        yield return StartCoroutine(GameMechanics.instance.DrawingInitialCards());

        if (IsServerGame)
        {
            yield return StartCoroutine(ChoosingFirstPlayer());
        }
        else
        {
            Debug.Log("Wait MasterClient choose first player");
        }
    }

    IEnumerator InitializingDecks()
    {
        Debug.Log("Initialize Decks");

        GameMechanics.instance.loadingScreen.currentMessage = "Generating Cards";

        yield return GeneratingTheCards(Gameplay.instance.selfPlayerNetwork);
        

        if (PhotonNetwork.offlineMode && WifiNetworkManager.instance == null)
        {
            GameMechanics.instance.loadingScreen.currentMessage = "Generating Opponent";
            Debug.Log("Gameplay.StartingGame: singleplayer mode, initialize deck locally");
            yield return GeneratingTheCards(Gameplay.instance.opponentPlayerNetwork);
        }

        GameMechanics.instance.loadingScreen.currentMessage = "Waiting Opponent";

        float maxTimeWaiting = 30;
        float currentTimeWaiting = 0;
        //wait remote player initialize his deck(could be a little latency here)
        while (true)
        {
            currentTimeWaiting += 1f;
            if ( currentTimeWaiting >  maxTimeWaiting)
            {
                Debug.Log("Max time waiting reached in initializing deck");
                SceneManager.LoadScene("MainMenu");
            }

            Debug.Log("waiting bothplayers initializedeck self=" + GameMechanics.instance.selfPlayer.myBoard.myDeck.cards.Count + " opponent=" + GameMechanics.instance.opponentPlayer.myBoard.myDeck.cards.Count);
           // Debug.Log("Waiting opponent initialize");
            if (GameMechanics.instance.opponentPlayer.myBoard.myDeck.cards.Count == 41 && GameMechanics.instance.selfPlayer.myBoard.myDeck.cards.Count == 41)
            {
                break;
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator GeneratingTheCards(PlayerNetwork playerNetwork)
    {
        Debug.Log("Generate Cards " + playerNetwork.gameObject.name);
       
		yield return StartCoroutine(CardManager.instance.GeneratingCards(playerNetwork.OpponentLocal));
        
        int[] myCardsID = CardManager.GetIdByCards( CardManager.GetMyCards(playerNetwork.myPlayer) ); 
       
        myCardsID.Shuffle();

        playerNetwork.InitializeDeckNetwork(myCardsID);
    }


    public IEnumerator ChoosingFirstPlayer()
    {
        Debug.Log("Choosing Random First Player");
        Player player = (Random.Range(0f, 1f) > 0.5f) ? GameMechanics.instance.selfPlayer : GameMechanics.instance.opponentPlayer;
       // Player player = GameMechanics.instance.selfPlayer; 
      //  Debug.Log("TEST: always using self as first");
        yield return new WaitForSeconds(1f);
        GetPlayerNetwork(player).PrepareTurnNetwork();
        
    }

    #endregion



    #region Win Game Logics
  
    public Player winner = null;
    
    public void WinGame(Player winnerPlayer)
    {
        if(winner!=null){
            Debug.Log("________________ Gameplay Wingame: already exist winner");
            Debug.Log("Winn Game: Do nothing!");
            return;
        }
        
        winner = winnerPlayer;
        Debug.Log("WIN GAME winner=" + winnerPlayer.gameObject.name);
        Debug.Log(winnerPlayer.gameObject.name + " Wins ");
        GameMechanics.instance.turnManager.EndTurn(GameMechanics.instance.turnManager.currentPlayer);
        GameMechanics.instance.turnManager.enabled = false;
        PlayerInputControl.instance.myPanel.gameObject.SetActive(false);

        GameMechanics.instance.ShowResultMenu(winnerPlayer is SelfPlayer);

        //add data to be persistent
        if (winnerPlayer is SelfPlayer)
        {
            SoundManager.instance.win.Play();
            PersistentCardGame.AddWin();
            
        }
        else
        {
            SoundManager.instance.lose.Play();
            PersistentCardGame.AddLose();
        }
      
        switch (initialGameplayMode)
        {
            case GameplayMode.Wifi:
                Debug.Log("End Game, destroying wifi");
                Destroy(WifiNetworkManager.instance);
                break;
            case GameplayMode.Multiplayer:
                Debug.Log("End game, disconecting from Photon");
                //PhotonNetwork.Disconnect();
                break;
            case GameplayMode.SinglePlayer:
                break;
            default:
                break;
        }
     
        if (selfPlayerNetwork!=null)
            Destroy(selfPlayerNetwork.gameObject);
        if(opponentPlayerNetwork!=null)
            Destroy(opponentPlayerNetwork.gameObject);

    }


    void OnOpponentStackRechedMax()
    {
        if(IsServerGame){
            selfPlayerNetwork.IWinGameNetwork();
           
        }
    }


    void OnSelfPlayerStackReachedMax()
    {
        if (IsServerGame)
        {
            opponentPlayerNetwork.IWinGameNetwork();
        }
        
    }


    #endregion



    #region Turn Logics

    void OnStartTurnCallback(Player player)
    {
     
        Debug.Log(player.gameObject.name + " Turn Begins");
        player.status.Reset();

    }

    void OnEndTurnCallback(Player player)
    {
       
        player.status.isFirstTurn = false;
        SoundManager.instance.endturn.Play();
        StartCoroutine(WaitingToPrepareTurn(player));   


    }


    public Card IsThereACardBeingActivated(Player player)
    {
        //TODO: better this
        return CardManager.instance.cardsGenerated.Find(c => { return c.myPlayer == player && c.isActivating; });
    }

  

    IEnumerator WaitingToPrepareTurn(Player player)
    {
        
        yield return StartCoroutine(WaitingAllCardsFinishActivation(player));
      
       
       
        Debug.Log(player.gameObject.name + " Turn Ends");

        if (!PhotonNetwork.offlineMode || WifiNetworkManager.instance != null)
            CheckGameState();

        yield return new WaitForSeconds(1f);


        if (winner == null)
        {
            if (IsServerGame)
            {
                GetPlayerNetwork(player.otherPlayer).PrepareTurnNetwork();
            }
            else
            {
                Debug.Log("Wait Master Client initiates turn");
            }
        }
    }

    IEnumerator WaitingAllCardsFinishActivation(Player player)
    {
        Card cardBeingActivated = IsThereACardBeingActivated(player);
        while (cardBeingActivated != null)
        {
            cardBeingActivated = IsThereACardBeingActivated(player);
            if (cardBeingActivated == null)
            {
                break;
            }
            Debug.Log("Wait cards finish activation to end turn");
            yield return new WaitForSeconds(3f);
            cardBeingActivated.CancelActivating();
        }
    }
    
    public void CheckGameState()
    {
        GameState myGameState = GameState.GetCurrentGameState();
        // Debug.Log("gameState=\n" + myGameState.ToString(true));
        selfPlayerNetwork.CheckGameStateNetwork(myGameState.ToString());
      
    }

    void OnEndLastSeconds(float time)
    {
        
        GameMechanics.instance.turnManager.currentPlayer.status.currentPenalty++;
        if(GameMechanics.instance.turnManager.currentPlayer is SelfPlayer)
            LogTextUI.Log("Times up! ");
       // Debug.Log("player " + GameMechanics.instance.turnManager.currentPlayer.gameObject.name + " penalty=" + GameMechanics.instance.turnManager.currentPlayer.status.currentPenalty);
        ChoosePlayerPenaltyTurn(GameMechanics.instance.turnManager.currentPlayer);
    }
   
    void ChoosePlayerPenaltyTurn(Player player)
    {
        switch (player.status.currentPenalty)
        {
            case PenaltyLateTurn.Level0:
                break;
            case PenaltyLateTurn.Level1:
                Debug.Log(player.gameObject.name + " Max seconds changed to 40");
                player.status.maxSecsTurnTime = 40;
                player.status.maxLastSecsTurnTime = 7;
                break;
            case PenaltyLateTurn.Level2:
                Debug.Log(player.gameObject.name + " Max seconds changed to 20");
                player.status.maxSecsTurnTime = 20;
                player.status.maxLastSecsTurnTime = 7;
                break;
            case PenaltyLateTurn.Level3:
                Debug.Log(player.gameObject.name + " the last seconds changed to 0");
                player.status.maxLastSecsTurnTime = 0;
                break;
            case PenaltyLateTurn.Level4:
                Debug.Log(player.gameObject.name + " loses due time penalty");
                if(IsServerGame){
                    GetPlayerNetwork(player.otherPlayer).IWinGameNetwork();
                }

                break;
            default:
                break;
        }
    }

    #endregion


  
}

using UnityEngine;
using System.Collections;
using Photon;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(PhotonView))]
public class PlayerNetwork : NetworkBehaviour {

    [HideInInspector]
    public bool OpponentLocal = false;


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
            if(_photonView ==null){
                _photonView = GetComponent<PhotonView>();
            }
            return _photonView;
        }
    }

   
    private Player _myplayer;
    public Player myPlayer
    {
        get
        {
            if (_myplayer == null)
            {
                if (OpponentLocal)
                {
                   
                    _myplayer = GameMechanics.instance.opponentPlayer;
                    gameObject.name = "OpponentPlayerNetwork";
                }
                else
                {
                    if(WifiNetworkManager.instance==null){
                        if (photonView.isMine)
                        {
                            _myplayer= GameMechanics.instance.selfPlayer;
                            gameObject.name = "SelfPlayerNetwork";
                        }
                        else
                        {
                            _myplayer= GameMechanics.instance.opponentPlayer;
                            gameObject.name = "OpponentPlayerNetwork";
                        }
                    }
                    else
                    {
                        if (isLocalPlayer)
                        {
                            _myplayer= GameMechanics.instance.selfPlayer;
                            gameObject.name = "SelfPlayerNetwork";
                        }
                        else
                        {
                            _myplayer= GameMechanics.instance.opponentPlayer;
                            gameObject.name = "OpponentPlayerNetwork";
                        }
                    }
                }
                
               
            }
            return _myplayer;
        }
    }

    public PlayerNetwork otherPlayerNetwork
    {
        get
        {
          if(myPlayer is SelfPlayer){
              return Gameplay.instance.opponentPlayerNetwork;
          }
          else
          {
              return Gameplay.instance.selfPlayerNetwork;
          }

        }
    }


    
    #region Mono

    void Awake()
    {

        Debug.Log("playerNetwork DontDestroyOnLoad");
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnLevelChanged;
    }

    void OnLevelChanged(Scene scene,LoadSceneMode mode)
    {

        if (scene.name == "MainMenu" && this !=null && gameObject!=null)
        {

            Debug.Log("Destroying " + gameObject.name + " due enter on main menu scene");
            Destroy(this.gameObject);
        }
        
    }
   

    void OnDisable()
    {

      
        Debug.Log("OnDisable PlayerNetwork="+ gameObject.name );
      //  Debug.Log("LoadScene MainMenu due OnDisable of " + gameObject.name);
        /*
        if(GameMechanics.instance!=null)
        if (myPlayer is SelfPlayer)
            Gameplay.instance.WinGame(GameMechanics.instance.selfPlayer);
        else
            Gameplay.instance.WinGame(GameMechanics.instance.selfPlayer);
         */

    }

    #endregion

    #region Public Functions

    public void CheckGameStateNetwork(string gameStateJson)
    {
        if (WifiNetworkManager.instance != null)
            CmdrpcCheckGameState(gameStateJson);
        else
        {
            photonView.RPC("RpcCheckGameState", PhotonTargets.AllBuffered, new object[] { gameStateJson });
        }
    }

    public void PrepareTurnNetwork()
    {
        if (WifiNetworkManager.instance != null)
             CmdrpcReceiveTurn();
        else
        {
            photonView.RPC("RpcPrepareTurn", PhotonTargets.All, new object[] { });
        }
    }

    public void EndMyTurnNetwork()
    {
        if (WifiNetworkManager.instance != null)
             CmdrpcEndMyturn();
        else
        {
            photonView.RPC("RpcEndMyturn", PhotonTargets.All, new object[] { });
        }
    }


    public void InitializeDeckNetwork(int[] cardsID)
    {
        if (WifiNetworkManager.instance != null)
        {
             CmdrpcInitializeDeck(cardsID);
        }
        else
        {
            photonView.RPC("RpcInitializeDeck", PhotonTargets.AllBuffered, new object[] { cardsID });
        }
    }


    public void ToggleWachTowerToAttackNetwork(bool rightOrLeft)
    {
        if (WifiNetworkManager.instance != null)
        {
            CmdToggleWachTowerToAttackNetwork(rightOrLeft);
        }
        else
        {
            photonView.RPC("RpcToggleWachTowerToAttackNetwork", PhotonTargets.All, new object[] { rightOrLeft });
        }
    }

    public void ExecuteActionNetwork(int id_instance, bool useFlipped = false, int[] targetCards = null, int[] cardsToPay = null)
    {
        if (WifiNetworkManager.instance != null)
        {
             CmdrpcExecuteAction(id_instance,useFlipped,targetCards,cardsToPay);
        }
        else
        {
            photonView.RPC("RpcExecuteAction", PhotonTargets.Others, new object[] { id_instance, useFlipped, targetCards, cardsToPay });
        }
    }

    public void IWinGameNetwork()
    {
        if (WifiNetworkManager.instance != null)
        {
            CmdrpcIWinGame();
        }
        else
        {
            photonView.RPC("RpcIWinGame", PhotonTargets.All, new object[] { });
        }
        
    }

    public void IConcedeNetwork()
    {
        if (WifiNetworkManager.instance != null)
        {
            CmdIConcede();
        }
        else
        {
            photonView.RPC("RpcIConcede", PhotonTargets.All, new object[] { });
        }
    }



    #endregion


    #region Photon Callbacks


  
    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("OnPhotonPlayerDisconnected otherPlayer");
        if(Gameplay.instance.winner==null){
            LogTextUI.Log("opponenet disconnected, you win the game!");
            Gameplay.instance.selfPlayerNetwork.IWinGameNetwork();
        }

    }
    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        //Debug.Log("Load MainMenu due OnLeftPhoton");
       // SceneManager.LoadScene("MainMenu");
    }

    public void OnPhotonJoinRoomFailed()
    {
        Debug.Log("OnPhotonJoinRoomFailed");
        Debug.Log("Load MainMenu");
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPhotonCreateRoomFailed()
    {
        Debug.Log("Load MainMenu due OnPhotonCreateRoomFailed");
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    

    #region RPCs

    [ClientRpc]
    [PunRPC]
    public void RpcToggleWachTowerToAttackNetwork(bool rightOrLeft)
    {
        if(rightOrLeft){
            myPlayer.otherPlayer.myBoard.attackWaitListSlot.isTargetToWatchTowerRight ^= true;//toggle
            if(myPlayer is SelfPlayer)
            if (myPlayer.otherPlayer.myBoard.attackWaitListSlot.isTargetToWatchTowerRight)
            {
                LogTextUI.Log("Selected "+CardManager.GetCardOfType<WatchTower>().title+" to Attack");

            }
            else
            {
                LogTextUI.Log("Deselected "+CardManager.GetCardOfType<WatchTower>().title+" to Attack");
            }
        }
        else
        {
            myPlayer.otherPlayer.myBoard.attackWaitListSlot.isTargetToWatchTowerLeft ^= true;//toggle
            if (myPlayer is SelfPlayer)
            if (myPlayer.otherPlayer.myBoard.attackWaitListSlot.isTargetToWatchTowerLeft)
            {

                LogTextUI.Log("Selected "+CardManager.GetCardOfType<WatchTower>().title+" to Attack");
            }
            else
            {
                LogTextUI.Log("Deselected "+CardManager.GetCardOfType<WatchTower>().title+" to Attack");
                
            }
        }
        
    }

    [ClientRpc]
    [PunRPC]
    public void RpcIConcede()
    {
        Gameplay.instance.WinGame(myPlayer.otherPlayer);
    }
   

    [ClientRpc]
    [PunRPC]
    public void RpcCheckGameState(string remoteGameStateJson)
    {
        if(myPlayer is SelfPlayer){
            return;
        }

        GameState myGameState = GameState.GetCurrentGameState() ;
        GameState remoteGameState = GameState.FromJson(remoteGameStateJson);

       if (!myGameState.CompareOpponentGameState( remoteGameState))
       {
           Debug.Log("________Game State differs!");      
       }
    }

    [ClientRpc]
    [PunRPC]
    public void RpcPrepareTurn()
    {
       // Debug.Log("rpcPrepareTurn " + myPlayer.gameObject.name);
        StartCoroutine(PrepareIn(1f));
    }
    IEnumerator PrepareIn(float secs)
    {
        yield return new WaitForSeconds(secs);
        StartCoroutine(GameMechanics.instance.PreparingTurn(myPlayer));
    }

    [ClientRpc]
    [PunRPC]
    public void RpcEndMyturn()
    {
        //Debug.Log("rpcEndMyturn " + myPlayer.gameObject.name );
        GameMechanics.instance.turnManager.EndTurn(myPlayer);
    }

   
   
    [ClientRpc]
    [PunRPC]
    public void RpcInitializeDeck( int[] cardsID)
    {
        
        //Debug.Log("PLayerNEtwork: rpcInitializeDeck player=" + myPlayer.gameObject.name);
        StartCoroutine( CardManager.instance.AdjustingCardsToDeckPlayer(myPlayer,cardsID) );
     
    }


    

    [ClientRpc]
    [PunRPC]
    public void RpcExecuteAction(int id_instance, bool useFlipped , int[] targetCards , int[] cardsToPay )
    {
        Action actionToExecute = new Action(id_instance,useFlipped,targetCards,cardsToPay);
        Card card = CardManager.GetCardById(id_instance);
       
        if (card != null)
        {
            if(card.myPlayer is SelfPlayer){
                return;
            }
            Debug.Log("rpcActivateCard player=" + myPlayer.gameObject.name + " action= "+actionToExecute);
           
            if (card.myPlayer == myPlayer)
            {
                Action.UseCard(actionToExecute);
            }
            else
            {
                Debug.Log("_________Error: rpcActivateCard id=" + id_instance + " isnt mine player=" + myPlayer.gameObject.name );
            }
        }
        else
        {
            Debug.Log("_________Error: rpcActivateCard id=" + id_instance + " not found!");
        }
    }

    [ClientRpc]
    [PunRPC]
    public void RpcIWinGame()
    {
        Gameplay.instance.WinGame(myPlayer);
    }


 


    #endregion


    #region UNET commands

    [Command]
    public void CmdToggleWachTowerToAttackNetwork(bool rightOrLeft)
    {
        RpcToggleWachTowerToAttackNetwork(rightOrLeft);
    }

    [Command]
    public void CmdIConcede()
    {
        RpcIConcede();
    }

    [Command]
    public void CmdSpawnCard(string titleCard)
    {
		GameObject cardPrefab = CardManager.GetCardPrefabByTitle(titleCard).gameObject;
        GameObject newCard = GameObject.Instantiate(cardPrefab);



        bool sucess = NetworkServer.SpawnWithClientAuthority(newCard, networkIdentity.clientAuthorityOwner);

        if (!sucess)
        {
            Debug.Log("___failed to spawn with client authority  for connection id=" + networkIdentity.clientAuthorityOwner.connectionId);
        } 
      

    }
   
   

    [Command]
    public void CmdrpcCheckGameState(string gameState)
    {
        RpcCheckGameState(gameState);
    }


    [Command]
    public void CmdrpcReceiveTurn()
    {
        RpcPrepareTurn();
    }

    [Command]
    public void CmdrpcEndMyturn()
    {
        RpcEndMyturn();

    }

    [Command]
    public void CmdrpcInitializeDeck(int[] cardsID)
    {
      
        RpcInitializeDeck(cardsID);
    }


    [Command]
    public void CmdrpcExecuteAction(int id_instance, bool useFlipped, int[] targetCards, int[] cardsToPay)
    {
        RpcExecuteAction(id_instance, useFlipped, targetCards, cardsToPay);
    }


    [Command]
    public void CmdrpcIWinGame()
    {
        RpcIWinGame();
    }
   


    #endregion

}

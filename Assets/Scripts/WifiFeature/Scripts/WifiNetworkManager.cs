using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WifiNetworkManager : NetworkManager {



  //  int maxTentativesBroadcastWifi = 10;
  //  private int currentTentativesBroadcastWifi = 0;
  
    
    static WifiNetworkManager _instance;
    public static WifiNetworkManager instance
    {
        get
        {
            if(_instance ==null){
                _instance = FindObjectOfType<WifiNetworkManager>();
            }
            return _instance;
        }
    }

    NetworkDiscoveryOverriden _networkDiscovery;
    public NetworkDiscoveryOverriden networkDiscovery
    {
        get
        {
            if (_networkDiscovery==null)
            {
                _networkDiscovery = GetComponent<NetworkDiscoveryOverriden>();
            }
            return _networkDiscovery;
        }
    }



    public delegate void OnServerAddAllPlayers();
    public event OnServerAddAllPlayers onServerAddAllPlayers;


   // public delegate void OnGotMaxTentativesBroadcastWifi();
    //public event OnGotMaxTentativesBroadcastWifi onGotMaxTentativesBroadcastWifi;

	// Use this for initialization
	void Start () {
        networkDiscovery.onFoundServerListening += OnFound;
        networkDiscovery.onListeningTimeOut += OnEndListeining;
        networkDiscovery.onBroadcastingTimeOut += OnEndSending;
     
      
        this.maxConnections = 2;
        StartCoroutine(CheckingState());
        SceneManager.sceneLoaded += OnNewSceneLoad;
       
	}

    void OnNewSceneLoad(Scene newScene, LoadSceneMode arg1)
    {
        if (newScene.name == "MainMenu")
        {
            Debug.Log("Destroy WifiManager due back to MainMenu");
            if (WifiNetworkManager.instance != null)
                Destroy(WifiNetworkManager.instance.gameObject);
        }
    }
    IEnumerator CheckingState()
    {
        for (; ; )
        {
            if(NetworkServer.connections.Count==1 && SceneManager.GetActiveScene().name=="Game"){
                Debug.Log("finishing game due only one player in game scene");
                Gameplay.instance.selfPlayerNetwork.IWinGameNetwork();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public bool foundWifiPlayer = false;

    void OnFound(string address, string data)
    {
        if (foundWifiPlayer)
        {
           // Debug.Log("already found Received Broadcast adress=" + address + " data=" + data);
            return;
        }
       // if (address == Network.player.ipAddress)
       // {
        //    Debug.Log("same address as mine =" + address);
//}

        Debug.Log("Received Broadcast adress=" + address + " data=" + data);
        foundWifiPlayer = true;
   
        ConnectWifiAsClient(address);

    }

    void OnEndListeining()
    {
       

    }

    void OnEndSending()
    {
        if (NetworkServer.connections.Count < 2)
        {
            Debug.Log("nobody joined end host");
            StopHost();

        }
    }
  
    void OnDisable()
    {

        Debug.Log("OnDisable WifiNetworkManager wifi components!");
        if (Gameplay.instance != null && Gameplay.instance.selfPlayerNetwork != null)
            Destroy(Gameplay.instance.selfPlayerNetwork.gameObject);
        if(Gameplay.instance!=null && Gameplay.instance.opponentPlayerNetwork!=null)
            Destroy(Gameplay.instance.opponentPlayerNetwork.gameObject);

        StopHost();
        networkDiscovery.StopAllCoroutines();
        SearchingOpponentsWifi searchWifi = FindObjectOfType<SearchingOpponentsWifi>();
        if(searchWifi!=null ){
            searchWifi.CancelWifiSearchOpponents();
        }
    }
	

    public void SearchOpponents(float duration)
    {
        networkDiscovery.onFoundServerListening += OnFound;
        networkDiscovery.onListeningTimeOut += OnEndListeining;
        networkDiscovery.onBroadcastingTimeOut += OnEndSending;
     

        networkDiscovery.StartListening(duration);

    }
    
    public void LetOpponentsKnowMe(float duration)
    {
        Debug.Log("WIFI Send Broadcast");
		string address="";
		#if !UNITY_WEBGL
		address = Network.player.ipAddress;
		#endif

		ConnectWifiAsHost(address);
        networkDiscovery.StartBroadcasting(duration);
    }

   
   
    void ConnectWifiAsClient(string adress)
    {
       
        networkAddress = adress;
        Debug.Log("ConnectWifiAsClient networkAdress=" + networkAddress);
        NetworkClient client = this.StartClient();
        if (client == null)
        {
            Debug.Log("Errrourrrrrrrrrrr");
            Destroy(this.gameObject);
        }
    }

    void ConnectWifiAsHost(string adress)
    {
       
        
        
        NetworkServer.Reset();
        networkAddress = adress;
        Debug.Log("ConnectWifiAsHost networkAdress=" + networkAddress);
        NetworkClient client= this.StartHost();
        if(client==null){
            Debug.Log("Errrourrrrrrrrrrr");
            Destroy(this.gameObject);
        }
    }

    #region unet callbacks
  
   


   // public short playerControllerIdServer;
   /////
   // public short playerControllerIdClient;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("OnServerAddPlayer connection=" + conn.connectionId);

        base.OnServerAddPlayer(conn,playerControllerId);
        /*
        if (conn.connectionId == 0)
        {
            Debug.Log("connectionId=" + conn.connectionId + " playerControllerId=" + playerControllerId);
            playerControllerIdServer = playerControllerId;
        }
       
        if(conn.connectionId == 1)
        {
            Debug.Log("connectionId=" + conn.connectionId + " playerControllerId=" + playerControllerId);
            playerControllerIdClient = playerControllerId;
        }
        */
        if(numPlayers == 2 && SceneManager.GetActiveScene().name != "Game")
        {
            Debug.Log("2 players! start Game scene");
            onServerAddAllPlayers();
            
        }        
    }

    

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.Log("OnClientError conn=" + conn.connectionId + " errorCode=" + errorCode + " Networkerrorenum=" + (NetworkError)errorCode);
        if (conn.connectionId == 1)
        {
            Debug.Log("other connection error, calling I win game network");
            Gameplay.instance.selfPlayerNetwork.IWinGameNetwork();
        }
        else if (conn.connectionId == 0)
        {
            Debug.Log("my connection error, calling local opponent win");
            Gameplay.instance.WinGame(GameMechanics.instance.selfPlayer);
        }
    }
    
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconnect connectionId=" + conn.connectionId);
        if(conn.connectionId == 1){
            Debug.Log("other connection disconected, calling I win game network");
            Gameplay.instance.selfPlayerNetwork.IWinGameNetwork();
        }
        else if(conn.connectionId == 0 )
        {
            Debug.Log("my connection disconected, calling local opponent win");
            Gameplay.instance.WinGame(GameMechanics.instance.opponentPlayer);
        }
    }
   
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("OnClientConnect connectionId=" + conn.connectionId);
        

    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect connectionId=" + conn.connectionId );
        Destroy(this.gameObject);
    }
  
    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        base.OnServerError(conn, errorCode);
        Debug.Log("OnServerError connectionId=" + conn.connectionId + "NetworkError parsed=" + (NetworkError)errorCode +" error code=" + errorCode);
        Destroy(this.gameObject);
    }

    #endregion
    
}

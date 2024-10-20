using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SearchingOpponentsWifi : Menu {

    WifiNetworkManager _wifiNetworkManager;
    WifiNetworkManager wifiNetworkManager
    {
        get
        {
            if(_wifiNetworkManager==null){
                _wifiNetworkManager = FindObjectOfType<WifiNetworkManager>();
            }
            return _wifiNetworkManager;
        }
    }

    public float maxTimeWaiting = 60;
    public float maxTimeSearching = 5;

    void Start()
    {

        SceneManager.sceneLoaded += (Scene s, LoadSceneMode mode) => { if (s.name == "Game") OnEnterOnGameScene(); };
    }

    void OnEnterOnGameScene()
    {
        wifiNetworkManager.networkDiscovery.StopBroadcast();
        wifiNetworkManager.networkDiscovery.StopAllCoroutines();
        if(searchWifi!=null){
            StopCoroutine(searchWifi);
        }
    }
  
    public Sprite[] spritesToChangeOverTime;

    IEnumerator ChangeImageOverTime( float stepTime=0.5f){
        Image myimage = GetComponent<Image>();
        int currentIndex=0;
        for(;;){


            myimage.sprite =spritesToChangeOverTime[currentIndex];  
            yield return new WaitForSeconds(stepTime);
            currentIndex++;
            if(currentIndex == spritesToChangeOverTime.Length){
                currentIndex=0;
            }

        }
    }
    public void OnEnable()
    {
        
        StartWifiSearching();
        StartCoroutine(ChangeImageOverTime());
    }

    public void CancelWifiSearchOpponents()
    {
        
     
        Hide();
        if(wifiNetworkManager!=null)
            Destroy(wifiNetworkManager.gameObject);

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if(transform.parent.GetComponentInChildren<NoOpponentsMenu>()==null)
            transform.parent.GetComponentInChildren<BattleMenu>(true).Show();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }


 
    IEnumerator searchWifi;
    void StartWifiSearching()
    {
        Debug.Log("StartWifiSearching");
        
        //PlayerPrefs.SetString("Wifi", "false");//info for next scene
        searchWifi = SearchingOpponents();
        StartCoroutine(searchWifi);
        
    }
  
    IEnumerator SearchingOpponents()
    {
  
        if (wifiNetworkManager == null)
        {
            var wifiManager = Resources.Load<GameObject>("Prefabs/Menu/WifiNetworkManager").GetComponent<WifiNetworkManager>();
            _wifiNetworkManager = GameObject.Instantiate(wifiManager);
        }
        else
        {
            Debug.Log("Already exist wifi manager");
            wifiNetworkManager.StopClient();
            wifiNetworkManager.StopServer();
        }
       // wifiNetworkManager.onGotMaxTentativesBroadcastWifi += OnNoOpponentsFound;

        wifiNetworkManager.onServerAddAllPlayers += OnServerAddAllPlayers;
        wifiNetworkManager.networkDiscovery.onFailedToStartListening += () => { wifiNetworkManager.LetOpponentsKnowMe(maxTimeWaiting); };
        wifiNetworkManager.networkDiscovery.onListeningTimeOut += () => { wifiNetworkManager.LetOpponentsKnowMe(maxTimeWaiting); };
        wifiNetworkManager.networkDiscovery.onFailedToStartBroadcast += () => { CancelWifiSearchOpponents(); };
        wifiNetworkManager.networkDiscovery.onBroadcastingTimeOut+=()=>{
        
            if(!wifiNetworkManager.foundWifiPlayer && !allPlayersHere){
                Destroy(wifiNetworkManager.gameObject);
                Hide();
                this.transform.parent.GetComponentInChildren<NoOpponentsMenu>(true).Show();
            }
        };


        wifiNetworkManager.SearchOpponents(maxTimeSearching);
        yield return new WaitForSeconds(maxTimeSearching+maxTimeWaiting);

        
        

    }


    bool allPlayersHere = false;
    void OnServerAddAllPlayers()
    {
        allPlayersHere = true;
        Debug.Log("On Server Add All players");
        Debug.Log("Start Load Game Scene");
        wifiNetworkManager.ServerChangeScene("Game");
        LoadGame(false);
    }
    
    void ShowNoOnpponentsFound()
    {
        
        if(wifiNetworkManager.foundWifiPlayer || allPlayersHere){
            return;
        }
        
        wifiNetworkManager.networkDiscovery.StopAllCoroutines();
        wifiNetworkManager.networkDiscovery.StopBroadcast();

        Debug.Log("ShowNoOnpponentsFound");
        Hide();
        this.transform.parent.GetComponentInChildren<NoOpponentsMenu>(true).Show();
        Destroy(wifiNetworkManager.gameObject);
    }
    
}

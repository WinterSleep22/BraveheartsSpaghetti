using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum SearchMode
{
    none,
    LetOthersKnowMe,
    SearchForOthers
}
public class SearchingOpponents : Menu {

    public int maxSecondsWaitingOpponents = 60;

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
        ConnectToPhoton();
        StartCoroutine(ChangeImageOverTime());
    }

    public void ConnectToPhoton()
    {
        //on show menu

        if (PhotonNetwork.networkingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            Debug.Log("Photon ConnectUsingSettings");
            PhotonNetwork.ConnectUsingSettings(MenuManager.instance.gameVersion);

        }
        else if (PhotonNetwork.networkingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Connected)
        {
            if (!PhotonNetwork.insideLobby)
            {
                Debug.Log("join Lobby");
                PhotonNetwork.JoinLobby();
            }
            else
            {
                Debug.Log("Already inside lobby");  
            }
        }
        else
        {
            Debug.Log("unhandled photon network state=" + PhotonNetwork.connectionState);
        }
    }

    void ShowNoResults()
    {
        Hide();
        transform.parent.GetComponentInChildren<NoOpponentsMenu>(true).Show();
    }

    
    IEnumerator WaitingOthersJoinMyRoom()
    {
        float timeStamp = Time.realtimeSinceStartup;
        Debug.Log("WaitingOthersJoinMyRoom");
      
        for(;;){

            if(!PhotonNetwork.connected ){
                Debug.Log("WaitingOthersJoinMyRoom canceled, not conncted to photon");
                break;
            }
           
           
            if(PhotonNetwork.inRoom && PhotonNetwork.playerList.Length==2){
                Debug.Log("WaitingOthersJoinMyRoom canceled , already in a room with 2 players");
                break;
            }
          
            if( Time.realtimeSinceStartup - timeStamp > maxSecondsWaitingOpponents){
                Debug.Log("WaitingOthersJoinMyRoom reached max of " + maxSecondsWaitingOpponents + " secs");
                break;
            }

           
            yield return new WaitForSeconds(1f);
        }

        if (PhotonNetwork.playerList.Length != 2)
        {
            PhotonNetwork.Disconnect();
            ShowNoResults();
        }

     
    }



    public void Cancel()
    {
        PhotonNetwork.Disconnect();
        Hide();
        transform.parent.GetComponentInChildren<BattleMenu>(true).Show();

    }



    public void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }
 
    public void OnConnectedToPhoton()
    {
        Debug.Log("Connected to Photon region=" + PhotonNetwork.networkingPeer.CloudRegion);
      
    }
    
    public void OnPhotonPlayerConnected(PhotonPlayer newplayer)
    {
        if (PhotonNetwork.playerList.Length == 2)
        {
            Debug.Log("OnPhotonPlayerConnected with 2 players, Loading Game!");
            LoadGame();
        }
        else if (PhotonNetwork.playerList.Length == 1)
        {
            Debug.Log("OnPhotonPlayerConnected: alone, waiting for other Player");
        }
    }
  
    public void OnFailedToConnectToPhoton 	( 	DisconnectCause  	cause	)
    {
        Debug.Log("OnFailedToConnectToPhoton=" + cause);
        Cancel();
    }
   
    public void OnJoinedRoom()
    {
        if (PhotonNetwork.playerList.Length == 2)
        {
            Debug.Log("OnJoinedRoom with 2 players, Loading Game!");
            LoadGame();
        }
        else if (PhotonNetwork.playerList.Length == 1)
        {
            Debug.Log("OnJoinedRoom: alone, waiting for other Player");
        }
    }

    public void  OnPhotonRandomJoinFailed (object[] codeAndMsg)
    {
       // Debug.Log("OnPhotonRandomJoinFailed errorMsg=" + codeAndMsg[1] + " errorCode=" + codeAndMsg[0]);
        Debug.Log("OnPhotonRandomJoinFailed");

        Debug.Log("trying to create room");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(PhotonNetwork.countOfRooms.ToString(), roomOptions, TypedLobby.Default);
        StartCoroutine(WaitingOthersJoinMyRoom());
    }

   
}

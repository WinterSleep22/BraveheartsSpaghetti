using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
public class NetworkDiscoveryOverriden : NetworkDiscovery {


    public delegate void OnFoundServerListening(string address,string data);
    public OnFoundServerListening onFoundServerListening;
    
    public delegate void OnListeningTimeOut();
    public OnListeningTimeOut onListeningTimeOut;

    public delegate void OnBroadcastingTimeOut();
    public OnBroadcastingTimeOut onBroadcastingTimeOut;

    public delegate void OnFailedToStartBroadcast();
    public OnFailedToStartBroadcast onFailedToStartBroadcast;

    public delegate void OnFailedToStartListening();
    public OnFailedToStartListening onFailedToStartListening;
    public void Awake()
    {
        
        bool sucess= Initialize();
        if(!sucess){
            Debug.Log("____Error Broadcast local wifi failed, port probably not avaiable");
        }
    }

  

    public void StartListening(float duration)
    {
        StopAllCoroutines();
        
        StopBroadcast();
        StartCoroutine(Listening(duration));
    }

    public void StartBroadcasting(float duration)
    {

        StopAllCoroutines();
        StopBroadcast();
        StartCoroutine(Broadcasting(duration));
    }


  

    IEnumerator Listening(float duration)
    {
        
        bool sucesss = Initialize();
        if (!sucesss)
        {
            Debug.Log("____Error Broadcast local wifi failed, port probably not avaiable");
        }

        bool sucess = this.StartAsClient();
        if (sucess)
        {
            
            receivedBroadcast = false;
            Debug.Log("Start Listening Broadcast");
            Debug.Log("isrunning=" + running + " isclient=" + isClient + " isserver=" + isServer);
            yield return new WaitForSeconds(duration);
            Debug.Log("Stop Listening Broadcast");
            
            StopBroadcast();
            if (!receivedBroadcast)
            {             
                if (onListeningTimeOut != null)
                {
                    onListeningTimeOut();
                }
            }
        }
        else
        {
            Debug.Log("Failed toStart Listening Broadcast");
            if (onFailedToStartListening!=null)
            {
                onFailedToStartListening();
            }
        }
       
    }
    public bool IsBroadcasting = false;
    IEnumerator Broadcasting(float duration)
    {
        IsBroadcasting = true;
        bool sucesss = Initialize();
        if (!sucesss)
        {
            Debug.Log("____Error Broadcast local wifi failed, port probably not avaiable");
        }

        bool sucess = this.StartAsServer();
        if (sucess)
        {
            Debug.Log("Start Sending Broadcast");
            Debug.Log("isrunning=" + running + " isclient=" + isClient + " isserver=" + isServer);
            yield return new WaitForSeconds(duration);
            Debug.Log("End Sending Broadcast");
            StopBroadcast();
            if (onBroadcastingTimeOut != null)
            {
                onBroadcastingTimeOut();
            }
        }
        else
        {
            Debug.Log("Failed toStart Sending Broadcast");
            if (onFailedToStartBroadcast!=null)
            {
                onFailedToStartBroadcast();
            }
        }
        IsBroadcasting = true;
    }

    bool receivedBroadcast = false;
    
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        receivedBroadcast = true;
        if(onFoundServerListening != null ){
            onFoundServerListening(fromAddress,data);
        }
        else
        {
            Debug.Log("_________________________________");
        }
    }
}

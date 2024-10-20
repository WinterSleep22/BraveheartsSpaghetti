using UnityEngine;
using System.Collections;


/// <summary>
/// interface to hide the implementation of UNET or Photon
/// </summary>
public abstract class CardNetwork {


    public abstract bool IsConnected { get; }
    public abstract bool IsServer { get; }

    /// <summary>
    /// function to all clients execute
    /// </summary>
    /// <param name="behaviourNetwork"></param>
    /// <param name="functionName"></param>
    /// <param name="args"></param>
    public abstract void ExecuteRPC(MonoBehaviour behaviourNetwork,string functionName, object[] args );

    /// <summary>
    /// start network
    /// </summary>
    /// <param name="asServer"></param>
    public abstract void Connect(bool asServer=false);

    public abstract void Disconnect();


    public abstract PlayerNetwork CreateNetworkPlayer();


    public abstract void Instantiate(string resourcesPrefabPath, Vector3 position, Quaternion rotation);

}

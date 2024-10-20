using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PingText : MonoBehaviour {


    Text myText;
	// Use this for initialization
	void Start () {
        gameObject.SetActive(!PhotonNetwork.offlineMode && PhotonNetwork.connected);
        myText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        myText.text = "Ping " + PhotonNetwork.GetPing() + " ms";	
	}
}

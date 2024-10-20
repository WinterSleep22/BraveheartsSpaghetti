using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonManager : MonoBehaviour {

    // Use this for initialization
    private void Awake()
    {
        if (FindObjectsOfType<SingletonManager>().Length == 1) DontDestroyOnLoad(this.gameObject);
        else Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    static SoundManager _instance;
    public static SoundManager instance
    {
        get
        {
            if(_instance==null){
                _instance = FindObjectOfType<SoundManager>();
            }
            return _instance;
        }
    }

    public AudioSource cardSelect;
    public AudioSource cardDeselect;
    public AudioSource cardActivated;
    public AudioSource cardDrawFromDeck;
    public AudioSource selfSufferAttack;
    public AudioSource opponentSufferAttack;
   
    public AudioSource win;
    public AudioSource lose;
    public AudioSource endturn;
    public AudioSource selfStartTurn;
    public AudioSource clock;


}

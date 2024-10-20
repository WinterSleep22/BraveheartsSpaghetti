using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class Timer : MonoBehaviour {



    Text _myText;
    public Text myText
    {
        get
        {
            if (_myText == null)
            {
                _myText = GetComponent<Text>();
            }
            return _myText;
        }
    }


    Player _myPlayer;
    public Player myPlayer
    {
        get
        {
            if (_myPlayer==null)
            {
                _myPlayer = GetComponentInParent<Player>();
            }
            return _myPlayer;
        }
    }

    
    // Use this for initialization
	
    void Start () {
        
        GameMechanics.instance.turnManager.onBeginTurn += (p) => { if ( p == myPlayer) this.enabled = true; };
        GameMechanics.instance.turnManager.onEndTurn += (p) => { if (p == myPlayer) this.enabled = false; };
        this.enabled = false;

	}


	// Update is called once per frame
	IEnumerator UpdatingTimer () {

        while (true)
        {
            float maxTime = 0;
            if (GameMechanics.instance.turnManager.currentPlayer != null)
            {
                maxTime = GameMechanics.instance.turnManager.currentPlayer.status.maxSecsTurnTime + GameMechanics.instance.turnManager.currentPlayer.status.maxLastSecsTurnTime;
            }
            SetCurrentTimerTurn(maxTime - GameMechanics.instance.turnManager.currentSecondsTurn);
            yield return new WaitForSeconds(0.2f);
        }
	}

    void OnEnable()
    {
        myText.enabled = true;
        StartCoroutine(UpdatingTimer());
    }

    void OnDisable()
    {
        myText.enabled = false;
        myText.text = "00";
        StopAllCoroutines();
    }

    void SetCurrentTimerTurn(float secs)
    {
        myText.text = secs.ToString("00");
    }

}

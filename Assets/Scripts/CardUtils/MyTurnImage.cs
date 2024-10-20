using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MyTurnImage : MonoBehaviour {



    private Image _myImage;
    public Image myImage
    {
        get
        {
            if (_myImage == null)
            {
                _myImage = GetComponent<Image>();
            }
            return _myImage;
        }
    }

	// Use this for initialization
	void Start () {
        GameMechanics.instance.onPlayerRevealFlippedCardDueStartTurn += OnStartPlayerTurn;
	}
	

    void OnStartPlayerTurn(Player player)
    {
        if(player is SelfPlayer){
            StartCoroutine(ShowingMyTurnMessage());
        }
    }

    IEnumerator ShowingMyTurnMessage()
    {
        float transitionTime = 1;

        SoundManager.instance.selfStartTurn.Play();
        
        myImage.enabled = true;

        myImage.CrossFadeAlpha(1, transitionTime, true);

        yield return new WaitForSeconds(3f);

        myImage.CrossFadeAlpha(0, transitionTime, true);

        yield return new WaitForSeconds(transitionTime);

        myImage.enabled = false;
    }
}

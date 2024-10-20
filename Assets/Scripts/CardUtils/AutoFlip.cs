using UnityEngine;
using System.Collections;

public class AutoFlip : MonoBehaviour {

    private Card _myCard;
    public Card myCard
    {
        get
        {

            if (_myCard == null)
            {
                _myCard = GetComponent<Card>();
            }
            return _myCard;
        }

    }
	
    
    // Use this for initialization
	void Start () {
        StartCoroutine(AutoFlipCard());
	}
    void Update()
    {
        if (!myCard.myDrag.IsDragging )
        {
            Destroy(this);
        }
    }
    IEnumerator AutoFlipCard()
    {
        yield return new WaitForSeconds(1f);
        for (; ; )
        {
           
            myCard.myCardGraphics.FlipCard();
            yield return new WaitForSeconds(2f);
        }
        
    }
}

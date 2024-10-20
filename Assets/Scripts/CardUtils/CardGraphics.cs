using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// manages flip state, front,back,image,sprite stuff
/// </summary>
public class CardGraphics : MonoBehaviour {

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
    
    
    private CanvasGroup _myCanvasGroup;
    public CanvasGroup myCanvasGroup
    {
        get
        {
            if (_myCanvasGroup == null)
            {
                _myCanvasGroup = GetComponent<CanvasGroup>();
            }
            return _myCanvasGroup;
        }
    }
   
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

   



    public bool isShowingFront
    {
        get
        {
            return myImage.sprite == myCard.front;
        }
    }
    public bool isShowingBack
    {
        get
        {
            return myImage.sprite == CardManager.back;
        }
    }
    public bool isShowingFrontOnField
    {
        get
        {
            ObjectCard objCard = (myCard as ObjectCard);
            if (objCard != null)
            {
                return myImage.sprite == objCard.frontOnField;
            }
            else
            {
                return false;
            }
        }
    }

    public bool isFlipped { get { return myImage.sprite == CardManager.back; } }

    /// <summary>
    /// if not object card, return normal front
    /// </summary>
    public Sprite frontOnField
    {
        get
        {
            ObjectCard objCard = (myCard as ObjectCard);
            if (objCard != null)
            {
                return  objCard.frontOnField;
            }
            else
            {
                return myCard.front;
            }
        }
    }
    
    
    
    
    void Awake()
    {
        myImage.sprite = CardManager.back;
    }

   
  
   
    public void ShowFront(bool immediate=false)
    {
        if(myCard is ObjectCard){
            var me = myCard as ObjectCard;
            if(myImage.sprite == me.frontOnField){
                transform.localScale = transform.localScale / 0.8f;
            }
        }
        if(immediate){
            myImage.sprite = myCard.front;
            return;
        }

        //Debug.Log("ShowFront");
        if(myImage.sprite!=myCard.front){
           
            if (isShowingFrontOnField)
            {
                
                Debug.Log("____ ShowFront immediate due front is on field " + myCard.title);
                myImage.sprite = myCard.front;
            }
            else
            {
                //Debug.Log("flip to show front " + myCard.title);
                FlipCard();
            }
        }
        
    }

    public void ShowBack(bool immediate=false)
    {

        
        if (immediate)
        {
            myImage.sprite = CardManager.back;
            return;
        }
        
       
        if (myImage.sprite != CardManager.back)
        {
            if (isShowingFrontOnField)
            {
                myImage.sprite = CardManager.back;
                Debug.Log("____ ShowBack immediate due front is on field " + myCard.title);
            }
            else
            {
                //Debug.Log("flip to show back");
                FlipCard();
            }
        }
    }
    
    public bool ShowFrontOnField()
    {

        ObjectCard objCard = (myCard as ObjectCard);
        if(objCard!=null){
            
            myImage.sprite = objCard.frontOnField;
        //    Debug.Log("____ ShowFrontField "  + myCard.title );
            return true;
        }
        else
        {
            myImage.sprite = myCard.front;
          //  Debug.Log("Cant show on field, card issnt the object card, showing card front instead");
            return false;
            
        }
    }


    IEnumerator flippingCoroutine;
    public void FlipCard()
    {
        
        if(flippingCoroutine!=null){
            //Debug.Log("Aready flipping card, flip canceled");
            return;
        }

        Sprite newSprite = (isFlipped) ? myCard.front : CardManager.back;

        flippingCoroutine = Fliping(newSprite);
        StartCoroutine(flippingCoroutine);
        
        //myImage.sprite = ;
    }

    IEnumerator Fliping(Sprite newSprite, float transitionTime = 1f)
    {
        Vector3 originalScale = gameObject.transform.localScale; 
        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", new Vector3(0, originalScale.y, originalScale.z),
            "time", transitionTime / 2f,
            "easetype", iTween.EaseType.easeInCubic
        ));

        yield return new WaitForSeconds(transitionTime / 2f);
        myImage.sprite = newSprite;
        iTween.ScaleTo(gameObject, iTween.Hash(
           "scale", originalScale,
           "time", transitionTime / 2f,
           "easetype", iTween.EaseType.easeOutCubic
       ));

        flippingCoroutine = null;
    }

    public void FadeAlpha(float initial, float final, float time)
    {
        
        StartCoroutine(FadingAlpha(GetComponent<CanvasRenderer>(), initial, final, time));
    }

    IEnumerator FadingAlpha(CanvasRenderer canvasRenderer, float initial, float final, float time)
    {
        float currentTime = 0;
        float step = 0.1f;
        int antiInfinity = 0;
        for (; ; )
        {
            if (antiInfinity++ > 100000)
                break;
            if (currentTime > time)
                break;
            float newAlpha = Mathf.Lerp(initial, final, currentTime / time);
          //  canvasRenderer.SetAlpha(newAlpha);
            myCanvasGroup.alpha = newAlpha;
            currentTime += step;
            yield return new WaitForSeconds(step);
        }
    }


}

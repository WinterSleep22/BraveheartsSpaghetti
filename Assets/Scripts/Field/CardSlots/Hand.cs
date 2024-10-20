using UnityEngine;
using System.Collections.Generic;


public class Hand : CardSlot {

    public float offsetHeightMultiplier = 1;
    public float radiusMultiplier = 1;
    public float offesetAngle = 20f;
    public float anglePadding = 10f;
  
    public float anglePaddingMultiplier=1;
    public float offsetAngleMultiplier = 1;
    private float originalAnglePadding;
    private float originalOffsetAngle;
    public override void Awake()
    {
        originalOffsetAngle = offesetAngle;
        originalAnglePadding = anglePadding;
        base.Awake();
        myDropZone.max = 41;
    }

    public override void Start()
    {
        OrganizeCards();
        StartCoroutine(CheckingState());
    }
    /// <summary>
    /// check if card is showing front/back at hand and ajust when necessary 
    /// </summary>
    /// <returns></returns>
    System.Collections.IEnumerator CheckingState()
    {
        for (; ; )
        {
            if (myPlayer is SelfPlayer)
            {
                anglePadding = originalAnglePadding * 1.4f - cards.Count * anglePaddingMultiplier;
                offesetAngle = originalOffsetAngle * 1.4f - cards.Count * offsetAngleMultiplier;

                var card = cards.Find(c => c.myCardGraphics.isShowingBack && myPlayer.currentDraggingCard != c && !c.isActivating);
                if (card != null)
                {
                    card.myCardGraphics.ShowFront();
                }
            }
            else
            {
                var card = cards.Find(c => c.myCardGraphics.isShowingFront && !c.isActivating );
                if (card != null)
                {
                    card.myCardGraphics.ShowBack();
                }
            }

            yield return new WaitForSeconds(4f);
        }
    }
    
   
 

    public override void OnCardEndComeBackDrag(Card _card)
    {
        if (myPlayer is OpponentPlayer)
        {
            _card.myCardGraphics.ShowBack();
        }
        else
        {
            _card.myCardGraphics.ShowFront();
        }
       
        _card.myCardGraphics.FadeAlpha(1f, 1f, 0.1f);
//        Debug.Log("OnCardEndComeBackDrag");
       // OrganizeCard(cards.IndexOf(_card));
        _card.transform.localScale = Vector3.one;
        OrganizeCards();
        
    }
   
    public override void OnCardEnter(Card card)
    {
        if (myPlayer is OpponentPlayer)
        {
            card.myCardGraphics.ShowBack();
        }
        else
        {
            card.myCardGraphics.ShowFront();
        }
       // Debug.Log("OnCardComeBackToHand");
        OrganizeCards();
        card.myDrag.allowDrag = true;
        if (myPlayer is SelfPlayer)
        {
            card.myCardGraphics.myImage.raycastTarget = true;

        }
        else
        {
            card.myCardGraphics.myImage.raycastTarget = false;
        }
    }

    public override void OnCardExit(Card _card)
    {
        base.OnCardExit(_card);
        OrganizeCards(0.5f);
        iTween.ScaleTo(_card.gameObject, Vector3.one, 1f);
        _card.myDrag.allowDrag = false;
        _card.myCardGraphics.myImage.raycastTarget = false;
       // OrganizingIn(1.5f);
    }

    public override Vector3 GetMyPosition(Card card)
    {
        int index = cards.FindIndex(c=>c == card);
        return GetOrganizedPosition(index);
    }

    public override Vector3 GetMyRotation(Card card)
    {
        return GetOrganizedRotation(cards.IndexOf(card)).eulerAngles;
    }



    public System.Collections.IEnumerator OrganizeCardsIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OrganizeCards();
    }

    public void OrganizeCards(float timeTransition=1f)
    {
        //   Debug.Log("Hand.OrganizeCards");
        foreach (var card in cards)
        {
            card.transform.SetParent(transform);
            OrganizeCard(cards.IndexOf(card), timeTransition);
        }
    }

    private void OrganizeCard(int indice,float timeTransition=1f)
    {
        if (GetComponent<PlayerHandCardSelector>()!=null)
        if (GetComponent<PlayerHandCardSelector>().currentSelectedCard == cards[indice])
        {
            Debug.Log("Hand:ignores card organization=" +cards[indice].title+" cause card is current selection");
            return;
        }
        
        if(cards[indice].isActivating ){
            Debug.Log("Hand: ignores card organize to " + cards[indice].title+" cause card is being activating");
            return;
        }
        
        iTween.MoveTo(cards[indice].gameObject, transform.position + GetOrganizedPosition(indice), timeTransition);
        iTween.RotateTo(cards[indice].gameObject, transform.rotation.eulerAngles + GetOrganizedRotation(indice).eulerAngles, timeTransition);
        iTween.ScaleTo(cards[indice].gameObject, iTween.Hash("scale", Vector3.one , "time", 1f));

        cards[indice].transform.SetSiblingIndex(indice);
        if (myPlayer is OpponentPlayer)
        {
            cards[indice].myCardGraphics.ShowBack();
        }
        else
        {
            cards[indice].myCardGraphics.ShowFront();
        }

    }


    public Vector3 GetNextOrganizedPosition()
    {
        return GetOrganizedPosition(cards.Count);
    }

    //TODO : create new layout for this
    public Vector3 GetOrganizedPosition(int i)
    {
        //TODO treat pair number of cards!


        float myAngle = GetOrganizedAngle(i,cards.Count);
        float radiusCircle = myRectTransform.GetWidth()/ 2f* radiusMultiplier;
        float cardPosX =  radiusCircle* Mathf.Cos(Mathf.Deg2Rad * myAngle);
        float cardPosY = radiusCircle* Mathf.Sin(Mathf.Deg2Rad * myAngle);
        Vector2 offesetCards = new Vector2(0, -radiusCircle * offsetHeightMultiplier);

        Vector3 positionCard = new Vector3(offesetCards.x, offesetCards.y, 0) + new Vector3(cardPosX, cardPosY, 0);
       // Debug.Log("organizing i=" + i + " angle=" + myAngle  + " position=" + positionCard);
        return positionCard;
    }
    private float GetOrganizedAngle(int i, int total)
    {


        float indiceFromCenter = i - ((float)total) / 2f ;

        float myAngle = 90 + anglePadding * indiceFromCenter + offesetAngle + myRectTransform.rotation.eulerAngles.z;
        return myAngle;
    }


    public Quaternion GetOrganizedRotation(int i)
    {

        //TODO treat pair number of cards!

        float myAngle = GetOrganizedAngle(i,cards.Count);
        myAngle -= 90;
       // Debug.Log("organizing i=" + i + " rotation=" + myAngle );
     
        Quaternion rotationCard = Quaternion.Euler(0, 0, myAngle );
        return rotationCard;
    }

}

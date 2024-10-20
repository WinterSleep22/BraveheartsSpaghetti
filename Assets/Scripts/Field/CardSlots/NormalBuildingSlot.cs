using UnityEngine;
using System.Collections;

public abstract class NormalBuildingSlot : BuildingCardSlot
{
    
    public override bool OnAllowCardAdd(Card card)
    {
        return card is NormalBuildingCard;
    }

    public override void OnCardEndComeBackDrag(Card _card)
    {
        if (myPlayer is OpponentPlayer)
        {
           AdjustRotationToSlot(_card);
        }
        AdjustSizeToSlot(_card);
        AdjustPositionToSlot(_card);
        _card.myCardGraphics.ShowFrontOnField();
    }
 
    public override void OnCardExit(Card _card)
    {
        AdjustRotationToNormal(_card);
        AdjustSizeToNormal(_card);
    }

    public override void OnCardEnter(Card _card)
    {
       // AdjustSizeToSlot(_card);
       // AdjustPositionToSlot(_card);
        _card.myCardGraphics.ShowFrontOnField();

    }



    void AdjustRotationToSlot(Card _card)
    {
        iTween.RotateTo(_card.gameObject, new Vector3(0,0,1)*180 , 0.5f);
    }
    
    void AdjustPositionToSlot(Card _card)
    {
        Vector3 adjust = (myPlayer is SelfPlayer) ? Vector3.down * 65 : Vector3.up * 65;
 
        iTween.MoveTo(_card.gameObject, transform.position + adjust, 0.5f);

    }

    void AdjustSizeToSlot(Card _card)
    {
       
        iTween.ScaleTo(_card.gameObject, new Vector3(1.5f, 1.5f, 1), 0.5f);
    }
    void AdjustSizeToNormal(Card _card)
    {
        iTween.ScaleTo(_card.gameObject, new Vector3(1, 1, 1), 0.5f);
    }
    void AdjustRotationToNormal(Card _card)
    {
        iTween.RotateTo(_card.gameObject, Vector3.zero, 0.5f);
    }
}

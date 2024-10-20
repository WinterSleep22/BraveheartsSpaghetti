using UnityEngine;
using System.Collections;

public class SpecialBuildingSlot : BuildingCardSlot {

    public override void OnCardEnter(Card _card)
    {
        base.OnCardEnter(_card);
       
        iTween.ScaleTo(_card.gameObject,Vector3.one*0.8f,1f);
    }
    public override void OnCardExit(Card _card)
    {
        base.OnCardExit(_card);
        iTween.ScaleTo(_card.gameObject, Vector3.one * 1f, 1f);
    }
    
    public override bool OnAllowCardAdd(Card _card)
    {
        bool isAllowed = _card is SpecialBuildingCard;

        return isAllowed;
    }
}

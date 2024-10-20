using UnityEngine;
using System.Collections;


public abstract class DefenseCard : ActionCard {


    public override bool IsAllowedToActivate()
    {
        if(myCardGraphics.isShowingFront){
            Debug.Log("Can't use Defense card Showing Front");
            return false;
        }
        return base.IsAllowedToActivate();
    }
}

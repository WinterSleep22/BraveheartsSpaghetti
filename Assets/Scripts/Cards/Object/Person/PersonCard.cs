using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class PersonCard : ObjectCard {


    public override bool IsAllowedToActivate()
    {
       // Debug.Log("PersonCard.IsAllowedToActivate");
        if(myPlayer.status.isFirstTurn ){
            if(myPlayer is SelfPlayer)
                LogTextUI.Log("Can’t present in first turn");
            Debug.Log("Can't use helper cards on firt turns");
            return false;
        }
        if (myPlayer.status.personCardSetInThisTurn  )
        {
            Debug.Log("Max person Card Per Turn");
            if (myPlayer is SelfPlayer)
            {
                LogTextUI.Log("You can’t present more");
            }
            return false;
        }
        return base.IsAllowedToActivate();
    }
    
}

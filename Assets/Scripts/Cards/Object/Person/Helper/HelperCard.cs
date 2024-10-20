using UnityEngine;
using System.Collections;

public abstract class HelperCard : PersonCard {

    public override bool IsAllowedToActivate()
    {
        if (myPlayer.myBoard.helperCardSlotLeft.cards.Count > 0)
            if (myPlayer.myBoard.helperCardSlotLeft.cards[0].title == this.title)
                return false;
        if (myPlayer.myBoard.helperCardSlotMiddle.cards.Count > 0)
            if (myPlayer.myBoard.helperCardSlotMiddle.cards[0].title == this.title)
                return false;
        if (myPlayer.myBoard.helperCardSlotRight.cards.Count > 0)
            if (myPlayer.myBoard.helperCardSlotRight.cards[0].title == this.title)
                return false;

        return base.IsAllowedToActivate();
    }


}

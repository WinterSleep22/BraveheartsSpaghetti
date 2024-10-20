using UnityEngine;
using System.Collections;

public class BasicDefense : DefenseCard {

    protected override IEnumerator Activating()
    {
        myCardGraphics.ShowBack(true);
        yield return new WaitForSeconds(0.5f);

        if (IsAllowedToActivate())
        {
            myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
            OnActivatingEnd(new Action(instance_id, true));
        }
        else OnActivatingEnd();
    }

}

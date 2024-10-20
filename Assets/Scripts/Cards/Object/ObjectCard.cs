using UnityEngine;
using System.Collections;

public abstract class ObjectCard : Card {

    public Sprite frontOnField;
    [HideInInspector]
    public CardSlot targetSlot = null;


    public override bool IsAllowedToActivate()
    {
        if (myPlayer.myBoard.GetFreeCardSlot(this)==null)
        {
            Debug.Log("No avaiable slot in field");
            return false;
        }
        return true;
    }
    protected override IEnumerator Activating()
    {
  
        if (IsAllowedToActivate())
        {
            if (targetSlot == null)
            {
                targetSlot = myPlayer.myBoard.GetFreeCardSlot(this);
            }
            Debug.Log("Placing " + title + " on field");
            myDrag.MoveToNewDropZone(targetSlot.myDropZone, 2f);
            OnActivatingEnd(new Action(instance_id,myCardGraphics.isFlipped));
            yield return new WaitForSeconds(2f);
        }
        else
        {

            OnActivatingEnd();
        }

    }  
}

using UnityEngine;
using System.Collections;

public class FullScaleAttack : BasicAttack {

    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        base.OnEnterCardSlot(cardSlot);
        if (cardSlot is AttackWaitListSlot)
            StartCoroutine(ActivatingFullScaleAttack());
    }


    IEnumerator ActivatingFullScaleAttack()
    {
        Debug.Log("Full Scale Effect Activated");
        myPlayer.status.effectFullScaleAttack = true;
        yield return new WaitForSeconds(1);
    }

    public override bool IsAllowedToActivate()
    {
        if (myPlayer.otherPlayer.myBoard.attackWaitListSlot.FindCardOfType<Charge>())
        {
            if (myPlayer is SelfPlayer)
                LogTextUI.Log("Can't use Full Scale Attack with Charge");
            return false;
        }
        return base.IsAllowedToActivate();
    }

    protected override IEnumerator Activating()
    {
        yield return new WaitForSeconds(0.6f);
        if (IsAllowedToActivate())
            yield return base.Activating();
        else
            OnActivatingEnd();
    }


}

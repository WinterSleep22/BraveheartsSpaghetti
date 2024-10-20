using UnityEngine;
using System.Collections;


public abstract class EffectCard : ActionCard
{
    public virtual int cost { get; private set; }
    public virtual bool SelectCardsToPay(Card[] c) { return false; }
    public virtual bool isPayingFromFlipped { get; protected set; }
}

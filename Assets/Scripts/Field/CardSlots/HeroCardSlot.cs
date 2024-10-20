using UnityEngine;
using System.Collections;

public class HeroCardSlot : CardSlot {

  

    public override bool OnAllowCardAdd(Card card)
    {

        return (card is HeroCard);
    }
   
   

  
   
}

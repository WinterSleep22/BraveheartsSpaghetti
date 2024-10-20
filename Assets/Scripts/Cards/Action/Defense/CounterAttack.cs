using UnityEngine;
using System.Collections;

public class CounterAttack : DefenseCard
{

    public delegate void OnCounterAttackSucessfulyActivated(Player player);
    public OnCounterAttackSucessfulyActivated onCounterAttackSucessfulyActivated;
    public override bool IsAllowedToActivate()
    {
        Fighter fighter= myPlayer.myBoard.TryGetCardOnField<Fighter>();
        if(fighter==null){
            
            Debug.Log("Can't use counter attack: No fighter on field");
            
            if(myPlayer is SelfPlayer){
              LogTextUI.Log("Need a "+CardManager.GetCardOfType<Fighter>().title+" card");
            }

            return false;
        }
      
      
        return base.IsAllowedToActivate();
    }

    protected override IEnumerator Activating()
    {
        yield return new WaitForSeconds(0.1f);
        myCardGraphics.ShowBack(true);
        if (IsAllowedToActivate())
        {
            myDrag.MoveToNewDropZone(myPlayer.myBoard.flippedCardSlot.myDropZone);
            OnActivatingEnd(new Action(instance_id, true));

        }
        else
        {

            OnActivatingEnd();
        }
        
    }


    public override IEnumerator OnCardRevealedStartTurnPreAttack()
    {
        yield return StartCoroutine( ActivatingCounterAttack() );
    }

    public IEnumerator ActivatingCounterAttack()
    {
        yield return new WaitForSeconds(1f);

        int attackNumber=myPlayer.myBoard.attackWaitListSlot.cards.Count;
        if (attackNumber > 0)
        {
            Debug.Log("Apply Counter-Attack Effect=" + attackNumber);

            //collect attack cards
            Card[] basicAttackCards = myPlayer.myBoard.attackWaitListSlot.cards.ToArray();

            //move to center attack cards
            for (int i = 0; i < attackNumber; i++)
            {
                basicAttackCards[i].MoveToCenter(2f,1f +i/2f);
            }      

            //move to center 
            MoveToCenter();

            yield return new WaitForSeconds(2f);
            
            //change owner
           
            for (int i = 0; i < attackNumber ; i++)
            {
                basicAttackCards[i].myDrag.MoveToNewDropZone(myPlayer.otherPlayer.myBoard.attackWaitListSlot.myDropZone);
            }       
           
        }
        else
        {
            Debug.Log("Counter-attack not activated, no attacks detected");
        }
        if(attackNumber > 0){
            if (onCounterAttackSucessfulyActivated!=null)
            {
                onCounterAttackSucessfulyActivated(myPlayer);
            }
        }
        Debug.Log("Counter-Attack to Used Deck");
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
        yield return new WaitForSeconds(1f);
        
    }


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Action
{
    public int card_instance;
    public int[] targetCards;
    public int[] payCards;
    public bool UseFlipped;

    public Action(int id, bool flipped = false, int[] targets = null, int[] cardstoPay = null)
    {
        card_instance = id;
        UseFlipped = flipped;
        targetCards = targets;
        payCards = cardstoPay;
    }
    public override string ToString()
    {
      return "card=" + CardManager.GetNameCardById(card_instance) + " UseFlipped=" + UseFlipped + " payCards=" + CardManager.GetNameCardsByIds(payCards) + " targetCards=" + CardManager.GetNameCardsByIds( targetCards);
    }

    public static void UseCard(Action action)
    {
        Card card = CardManager.GetCardById(action.card_instance);
       
        if (card == null)
        {
            Debug.Log("______________ PlayerInputControl.UseCard instance_id=" + action.card_instance + " not found!");
            return;
        }

        Debug.Log("Action.UseCard action " + action.ToString());
       
        if (card is BuildingCard)
        {

            PlayerInputControl.instance.UseBuildingCard(card as BuildingCard, CardManager.GetCardsById(action.payCards).ToList());

			return;
        }


        if (card is Insight)
        {

            PlayerInputControl.instance.TryUseCard(card, flipped: action.UseFlipped);

            return;
        }
        if (card is Satisfaction)
        {
            Satisfaction satisfactionCard = card as Satisfaction;
            if (action.UseFlipped)
            {
                PlayerInputControl.instance.TryUseCard(satisfactionCard, flipped: true);
            }
            else
            {
                if (action.targetCards != null && action.targetCards.Length == 2)
                {
                    HelperCard cardChoosedOpponent = CardManager.GetCardById(action.targetCards[1]) as HelperCard;

                    HelperCard cardChoosedToBePaid = null;

                    if (action.payCards != null && action.payCards.Length == 1)
                    {
                        cardChoosedToBePaid = CardManager.GetCardById(action.payCards[0]) as HelperCard;
                        PlayerInputControl.instance.UseDuel(satisfactionCard, cardChoosedOpponent, cardChoosedToBePaid);
                    }
                    else
                    {
                        Debug.Log("____ Error PlayerInputControl.UseCard trying use satisfaction without card to pay");
                    }


                }
                else
                {
                    Debug.Log("____ Error PlayerInputControl.UseCard trying use satisfaction without target card");
                }


            }
            return;
        }
        if (card is Supply)
        {
            PlayerInputControl.instance.TryUseCard(card, action.UseFlipped);
            return;
        }

        if (card is CounterAttack)
        {
            PlayerInputControl.instance.TryUseCard(card, action.UseFlipped);
            return;
        }

        if (card is BasicDefense)
        {
            PlayerInputControl.instance.TryUseCard(card, action.UseFlipped);
            return;
        }

        if (card is FullScaleAttack)
        {

            PlayerInputControl.instance.TryUseCard(card, action.UseFlipped);
            return;
        }

        if (card is Raid)
        {
            if (!action.UseFlipped)
            {
                if (action.targetCards != null && action.targetCards.Length == 1 && CardManager.GetCardById(action.targetCards[1]) is WatchTower)
                {
                    Debug.Log("Raid, remote use with target watch tower");
                    PlayerInputControl.instance.TargetWatchTowerForAttack();
                }
            }

            PlayerInputControl.instance.TryUseCard(card, action.UseFlipped);
            return;
        }

        if (card is BasicAttack)
        {
            PlayerInputControl.instance.TryUseCard(card, action.UseFlipped);
            return;
        }

        if (card is HelperCard)
        {
            PlayerInputControl.instance.TryUseCard(card, action.UseFlipped);
            return;
        }


        Debug.Log("_________Error: CArd not did not enter in any case " + card.title);
    }

}

[System.Serializable]
public class ActionsTurn
{
    //record of action taken in this turn
    public List<Action> actionsTaken = new List<Action>();
}

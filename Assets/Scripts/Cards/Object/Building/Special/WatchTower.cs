using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class WatchTower :  SpecialBuildingCard
{

    public Text stackAttackText;

    private int _stackAttack = 0;
    public int stackAttack
    {
        get
        {
            return _stackAttack;
        }
        set
        {
            if (_stackAttack!=value)
            {
                _stackAttack = value;
                stackAttackText.text = _stackAttack.ToString();
                SetTargetEffect(false);
                if(_stackAttack >= 3){
                    OnWatchTowerDestroyed();
                }
            }
        }
    }
    void OnWatchTowerDestroyed()
    {
        LogTextUI.Log(title+ " Destroyed");
        myCardGraphics.ShowFront();
        myDrag.MoveToNewDropZone(myPlayer.myBoard.usedCardsSlot.myDropZone);
    }

    public void SetTargetEffect(bool active)
    {
        if(active){
           myCardGraphics.myImage.color = Color.red;
        }
        else
        {
            myCardGraphics.myImage.color = Color.white;
        }
    }

   
    public override void OnEnterCardSlot(CardSlot cardSlot)
    {
        if (cardSlot is SpecialBuildingSlot)
        {
            _stackAttack = 0;
            SetTargetEffect(false);
            myPlayer.otherPlayer.myBoard.flippedCardSlot.RevealCard();
            stackAttackText.enabled = true;
        }
        else
        {
            stackAttackText.enabled = false;
        }

       

    }

    public override void OnExitCardSlot(CardSlot cardSlot)
    {

            stackAttackText.enabled = false;
    }

  
}

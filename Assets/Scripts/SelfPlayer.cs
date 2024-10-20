
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SelfPlayer : Player {

    BlockAllInput _blockAllGameInput;
    public BlockAllInput blockAllGameInput
    {
        get
        {
            if (_blockAllGameInput == null)
            {
                _blockAllGameInput = GetComponentInChildren<BlockAllInput>(true);
            }
            return _blockAllGameInput;
        }
    }


    BlockSelfFieldInput _blockSelfGameInput;
    public BlockSelfFieldInput blockSelfGameInput
    {
        get
        {
            if (_blockSelfGameInput == null)
            {
                _blockSelfGameInput = GetComponentInChildren<BlockSelfFieldInput>(true);
            }
            return _blockSelfGameInput;
        }
    }

    public Button endTurnButton;
    public Button exitGameButton;

    public void SetBlockAllInput(bool block)
    {
        blockAllGameInput.gameObject.SetActive(block);
    }

    public void SetBlockSelfInput(bool block)
    {
        blockSelfGameInput.gameObject.SetActive(block);
    }

    public void SetPlayerInput(bool active)
    {
        endTurnButton.interactable = active;
        SetBlockAllInput(!active);
        SetBlockSelfInput(!active);
        myBoard.myHand.GetComponent<PlayerHandCardSelector>().enabled = active;
        SetAllowDrag(active);
        if (active)
        {
        }
        else
        {
            myBoard.myHand.GetComponent<PlayerHandCardPaySelector>().enabled = false;
            myBoard.myHand.GetComponent<CardSlotSelectorForAttack>().enabled = false;
            myBoard.myHand.GetComponent<CardSlotSelectorForSatisfaction>().enabled = false;
            myBoard.myHand.GetComponent<PlayerHandCardEffectPaySelector>().enabled = false;
        }
    }

    public void ExitGame()
    {
        GameMechanics.instance.myCanvas.transform.GetComponentInChildren<MenuDuringGame>(true).Show();
    }
}

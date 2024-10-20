using UnityEngine;
using System.Collections;

public class OpponentPlayer : Player{

    BlockInputOpponent _blockOpponentGameInput;
    public BlockInputOpponent blockOpponentGameInput
    {
        get
        {
            if (_blockOpponentGameInput == null)
            {
                _blockOpponentGameInput = GetComponentInChildren<BlockInputOpponent>(true);
            }
            return _blockOpponentGameInput;
        }
    }
    public void SetBlockOpponentInput(bool block)
    {
        blockOpponentGameInput.gameObject.SetActive(block);
    }
}

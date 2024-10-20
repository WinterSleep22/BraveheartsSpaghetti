using UnityEngine;
using System.Collections;


[System.Serializable]
public class PlayerStatus
{
    public PenaltyLateTurn currentPenalty = PenaltyLateTurn.Level0;
    public int maxSecsTurnTime = 50;
    public int maxLastSecsTurnTime = 7;

    public bool effectFullScaleAttack = false;
    public bool personCardSetInThisTurn = false;
    public bool normalBuildingCardSetInThisTurn = false;
    public bool specialBuildingCardSetInThisturn = false;
    public bool isFirstTurn = true;
    public void Reset()
    {
        
        effectFullScaleAttack = false;
        personCardSetInThisTurn = false;
        normalBuildingCardSetInThisTurn = false;
        specialBuildingCardSetInThisturn = false;
    }   

    public static PlayerStatus FromJson(string json)
    {
        return JsonUtility.FromJson<PlayerStatus>(json);
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}
using UnityEngine;
using System.Collections;

public class NoOpponentsMenu : Menu {

    public void Back()
    {
        Hide();
        transform.parent.GetComponentInChildren<BattleMenu>(true).Show();
    }
}

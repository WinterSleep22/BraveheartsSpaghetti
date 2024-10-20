using UnityEngine;
using System.Collections;

public class BattleMenu : Menu
{
    public override void Show()
    {
        base.Show();
        ShowBanner();
    }

    public void MultiPlay()
    {
        Hide();
        transform.parent.GetComponentInChildren<SearchingOpponents>(true).Show();
    }

    public void WiFi()
    {
        Hide();
        this.transform.parent.GetComponentInChildren<SearchingOpponentsWifi>(true).Show();
    }

    public void SinglePlayer()
    {
        Hide();
        transform.parent.GetComponentInChildren<SinglePlayMenu>(true).Show();
    }

    public void Back()
    {
        Hide();
        transform.parent.GetComponentInChildren<MainMenu>(true).Show();
    }

}

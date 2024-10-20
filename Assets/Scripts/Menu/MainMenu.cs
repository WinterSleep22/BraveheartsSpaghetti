using UnityEngine;
using System.Collections;

public class MainMenu : Menu
{
    void Start()
    {
        // Show();
    }

    public override void Show()
    {
        base.Show();
        ShowBanner();
    }

    public void Battle()
    {
        Hide();
        transform.parent.GetComponentInChildren<BattleMenu>(true).Show();
    }

    public void MyInfo()
    {
        Hide();
        transform.parent.GetComponentInChildren<MyInfoPanelMenu>(true).Show();
    }
    public void Shop()
    {
        Hide();
        transform.parent.GetComponentInChildren<ShopMenu>(true).Show();
    }
    public void Exit()
    {
        Hide();
        transform.parent.GetComponentInChildren<ExitMainMenu>(true).Show();
    }

}

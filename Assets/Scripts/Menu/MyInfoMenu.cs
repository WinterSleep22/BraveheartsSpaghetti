using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MyInfoMenu : Menu
{
    public Text TotalWinLose;
    public Text MultiplayerWin;
    public Text WiFiWin;
    public Text SingleplayerWin;
    public Text TotalPoints;

    public override void Show()
    {
        base.Show();
        TotalWinLose.text = PersistentCardGame.GetTotalWins() + "/" + PersistentCardGame.GetTotalLoses();
        MultiplayerWin.text = PersistentCardGame.GetMultiplayerTotalWins() + "/" + PersistentCardGame.GetMultiplayerTotalLoses();
        WiFiWin.text = PersistentCardGame.GetWifiTotalWins() + "/" + PersistentCardGame.GetWifiTotalLoses();
        SingleplayerWin.text = PersistentCardGame.GetSingleplayerTotalWins() + "/" + PersistentCardGame.GetSingleplayerTotalLoses();
        TotalPoints.text = PersistentCardGame.GetBalance().ToString();
    }
    public void Ok()
    {
        Hide();
        transform.parent.GetComponentInChildren<MyInfoPanelMenu>(true).Show();
    }
}

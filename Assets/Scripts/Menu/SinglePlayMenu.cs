using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SinglePlayMenu : Menu
{
    public void SinglePlay()
    {
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.CreateRoom("MyRoom");
        Debug.Log("Photon Offline mode");
        LoadGame();
    }

    public override void Show()
    {
        base.Show();
        ShowBanner();
    }

    public void Tutorial()
    {
        Hide();
        transform.parent.GetComponentInChildren<TutorialMenu>(true).Show();
    }

    public void Back()
    {
        Hide();
        transform.parent.GetComponentInChildren<BattleMenu>(true).Show();
    }
}

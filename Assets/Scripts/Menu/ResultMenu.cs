using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class ResultMenu : Menu
{


    public override void Show()
    {
        base.Show();

        transform.SetSiblingIndex(LogTextUI.instance.transform.GetSiblingIndex() - 1);

        LoadInterstitial();
    }

    public void PlayAgain()
    {
        ShowInterstitial();
        switch (Gameplay.initialGameplayMode)
        {
            case GameplayMode.Wifi:

                PlayerPrefs.SetInt(MenuManager.instance.SearchOpponentsWifiMenuKey, 1);
                SceneManager.LoadScene("MainMenu");
                break;
            case GameplayMode.Multiplayer:
                PlayerPrefs.SetInt(MenuManager.instance.SearchOpponentsMenuKey, 1);
                SceneManager.LoadScene("MainMenu");
                break;
            case GameplayMode.SinglePlayer:
                SceneManager.LoadScene("Game");
                break;
            default:
                break;
        }


    }

    public void Exit()
    {
        ShowInterstitial();
        PlayerPrefs.SetInt("Initial", 1);
        SceneManager.LoadScene("MainMenu");
    }

}

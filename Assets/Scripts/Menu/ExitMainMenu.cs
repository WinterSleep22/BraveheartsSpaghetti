using UnityEngine;
using System.Collections;

public class ExitMainMenu : Menu {

    public void ExitGame()
    {
        PlayerPrefs.SetInt("Initial", 0);
        Application.Quit();
    }

    public void Cancel()
    {
        Hide();
        transform.parent.GetComponentInChildren<MainMenu>(true).Show();
    }
}

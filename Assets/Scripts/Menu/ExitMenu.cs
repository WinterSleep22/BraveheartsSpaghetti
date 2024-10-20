using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class ExitMenu : Menu
{
    public override void Show()
    {
        base.Show();
        ShowBanner();
    }

    public void ExitGame()
    {
        Hide();
        this.transform.parent.GetComponentInChildren<MenuDuringGame>(true).Show();

    }

    public void Cancel()
    {
        Hide();
    }
}

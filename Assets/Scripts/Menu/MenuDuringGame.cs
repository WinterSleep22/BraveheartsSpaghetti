using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuDuringGame : Menu {

    public void Concede()
    {
        Hide();
        Gameplay.instance.selfPlayerNetwork.IConcedeNetwork();
    }

    public override void Show()
    {
        LoadingScreen ls = GameObject.FindObjectOfType<LoadingScreen>();
        if (ls != null)
            if (ls.gameObject.activeSelf)
            {
                Debug.Log("Can't pause (during loading)");
                return;
            }
        GameObject.FindObjectOfType<TurnManager>().Pause();
    }

    public IEnumerator _Show()
    {
        base.Show();
        yield return new WaitForSeconds(0.5f);
        transform.SetAsLastSibling();
    }

    public void Back()
    {
        // this.transform.parent.GetComponentInChildren<ExitMenu>(true).Show();
        Hide();
        GameObject.FindObjectOfType<TurnManager>().Unpause();
    }
}

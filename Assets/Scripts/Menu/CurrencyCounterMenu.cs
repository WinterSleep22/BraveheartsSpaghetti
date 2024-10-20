using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyCounterMenu : Menu
{

    public Menu[] MenusToBeActiveOn;
    public Text text;
    void Update()
    {
        #region debugging
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            PersistentCardGame.CreditBalance(100);
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            PersistentCardGame.DebitBalance(100);
        #endregion
    }

    void Start()
    {
        PersistentCardGame.balanceChanged += OnBalanceChanged;
        PersistentCardGame.SetBalance(100);
        Menu.OnMenuShow += Initialize;
        this.gameObject.SetActive(false);
        foreach (Menu m in MenusToBeActiveOn)
            if (m.isShown)
            {
                Show();
                break;
            }
    }


    void Initialize()
    {
        OnBalanceChanged();
        for (int i = 0; i < MenusToBeActiveOn.Length; i++)
        {
            if (MenusToBeActiveOn[i].isShown)
            {
                Show();
                return;
            }
        }
        Hide();
    }

    public override void Show()
    {
        this.isShown = true;
        transform.GetComponentInParent<MenuManager>().SetActiveIn(transitionTime, true, myMenu.gameObject);
        myMenu.Fade((this.isShown) ? 1f : 0, 1f, transitionTime, transform.GetComponentInParent<MenuManager>());
    }

    void OnBalanceChanged()
    {
        text.text = PersistentCardGame.GetBalance().ToString();
    }

    void OnDestroy()
    {
        PersistentCardGame.balanceChanged -= OnBalanceChanged;
        Menu.OnMenuShow -= Initialize;
    }
}

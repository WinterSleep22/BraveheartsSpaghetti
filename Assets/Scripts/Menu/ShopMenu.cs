using UnityEngine;
using System.Collections;

public class ShopMenu : Menu
{

    public const int boxPrice = 80;
    public Menu enoughMoney;
    public Menu noEnoughMoney;
    public Menu alreadyHave;
    public Menu overBuy;

    public void Back()
    {
        Hide();
        transform.parent.GetComponentInChildren<MainMenu>(true).Show();
    }

    public override void Show()
    {
        base.Show();
        ShowBanner();
    }

    public void BuyWithCoinsButton()
    {
        print((MenuCardsOpening.dropType()));
        if (PersistentCardGame.GetBalance() >= boxPrice)
        {
            switch (MenuCardsOpening.dropType())
            {
                case MenuCardsOpening.DropType.Normal:
                    enoughMoney.Show();
                    break;
                case MenuCardsOpening.DropType.AleadyHave:
                    alreadyHave.Show();
                    break;
                case MenuCardsOpening.DropType.Overbuy:
                    overBuy.Show();
                    break;
            }
        }
        else
            noEnoughMoney.Show();

    }

    public void DebitBalance()
    {
        PersistentCardGame.DebitBalance(boxPrice);
        PersistentCardGame.SetChestAmount(PersistentCardGame.GetChestAmount() + 1);
    }

    public void BuyWithDollarsButton()
    {

    }
}

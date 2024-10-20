using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInfoPanelMenu : Menu {

	public void MyInfo()
	{
		Hide();
		transform.parent.GetComponentInChildren<MyInfoMenu>(true).Show();
	}

	public void MyDeck()
	{
		Hide();
		this.transform.parent.GetComponentInChildren<MyDeckMenu>(true).Show();
	}
		

	public void Back()
	{
		Hide();
		transform.parent.GetComponentInChildren<MainMenu>(true).Show();
	}
}

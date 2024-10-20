using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialMenu : Menu {

    public Sprite[] tutorialSprites;
    public Image tutorial;
    int currentTutorial = 0;
   
    public void Ok()
    {
        Hide();
        transform.parent.GetComponentInChildren<SinglePlayMenu>(true).Show();
    }
    public override void Show()
    {
        base.Show();
        tutorial.sprite = tutorialSprites[0];
        currentTutorial = 0;

    }
    
    public void Back()
    {
        if(currentTutorial==0){
            return;
        }
        currentTutorial--;
        tutorial.sprite = tutorialSprites[currentTutorial];
    }

    public void Forward()
    {
        if (currentTutorial == tutorialSprites.Length-1)
        {
            return;
        }
        currentTutorial++;
        tutorial.sprite = tutorialSprites[currentTutorial];
    }

}

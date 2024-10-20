using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TouchAnywhere : MonoBehaviour, IPointerClickHandler
{


    bool initialized = false;

    public Image BackgroundImageTouch;
    public Image BackgroundImage;



    // Use this for initialization
    void Start()
    {

        initialized = (PlayerPrefs.HasKey("Initial")) ? PlayerPrefs.GetInt("Initial") != 0 : false;
        if (initialized)
        {
            BackgroundImageTouch.CrossFadeAlpha(0, 0.01f, true);
            GetComponentInChildren<MainMenu>(true).Show();
            PlayerPrefs.SetInt("Initial", 0);
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideBackground()
    {
        BackgroundImageTouch.CrossFadeAlpha(0, 0.3f, true);
    }
    public void InitialBackground()
    {
        initialized = (PlayerPrefs.HasKey("Initial")) ? PlayerPrefs.GetInt("Initial") != 0 : false;
        if (initialized)
        {
            return;
        }

        initialized = true;
        HideBackground();

        if (GetComponentsInChildren<Menu>().Length == 0)
        {
            SoundManagerMenu.instance.onFirstBackgroundHides.Play();
            BackgroundMusicManager.instance.menuMusic.Play();
            GetComponentInChildren<MainMenu>(true).Show();
        }

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        InitialBackground();
    }
}

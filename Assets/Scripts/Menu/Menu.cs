using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    public delegate void OnMenuChange();
    public static event OnMenuChange OnMenuShow;
    public bool isShown;
    public float transitionTime = 0.3f;
    public CanvasGroup myMenu
    {
        get
        {
            if (_myMenu == null)
            {
                _myMenu = GetComponent<CanvasGroup>();
            }
            return _myMenu;
        }
    }

    private CanvasGroup _myMenu;

    public virtual void Hide()
    {
        this.isShown = false;
        MenuManager.instance.SetActiveIn(transitionTime, false, myMenu.gameObject);
        myMenu.Fade(1f, 0f, transitionTime, MenuManager.instance);
    }

    public virtual void Show()
    {
        this.isShown = true;
        if (OnMenuShow != null)
            OnMenuShow.Invoke();
        transform.GetComponentInParent<MenuManager>().SetActiveIn(transitionTime, true, myMenu.gameObject);
        //   myMenu.gameObject.SetActive(true);

        myMenu.Fade(0.5f, 1f, transitionTime, transform.GetComponentInParent<MenuManager>());
    }

    public void ShowBanner()
    {
        //Banners off
        /*try
        {
            if (!TappxManagerUnity.instance.isBannerVisible())
                TappxManagerUnity.instance.show(TappxSDK.TappxSettings.POSITION_BANNER.BOTTOM, false);
        }
        catch
        {
            Debug.LogWarning("Can't show banner; Not on android device!  " + this);
        }*/
    }

    public void ShowInterstitial()
    {
        try
        {
            /*if (TappxManagerUnity.instance.isInterstitialReady())
                TappxManagerUnity.instance.interstitialShow();
            else
                TappxManagerUnity.instance.loadInterstitial(true);*/
        }
        catch
        {
            Debug.LogWarning("Can't display interstitial; Not on android device!  " + this);
        }
    }

    public void LoadInterstitial()
    {
        try
        {
            /*if (!TappxManagerUnity.instance.isInterstitialReady())
                TappxManagerUnity.instance.loadInterstitial(false);*/
        }
        catch
        {
            Debug.LogWarning("Can't load interstitial; Not on android device!  " + this);
        }
    }

    /// <summary>
    /// load Game scene async showing game loading bar
    /// </summary>
    public virtual void LoadGame(bool loadScene = true)
    {
        StartCoroutine(LoadingGame(loadScene));
    }

    protected AsyncOperation myAsyncOperation;
    public virtual AsyncOperation GetLoadingGameAsyncOp
    {
        get
        {
            return myAsyncOperation;
        }
    }

    float simulationLoadingTime = 2f;

    IEnumerator LoadingGame(bool LoadGameScene = true)
    {
        GameObject loadingGameBar = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Menu/LoadingGameBar"));
        loadingGameBar.transform.SetParent(FindObjectOfType<Canvas>().transform);
        //   loadingGameBar.transform.localPosition = new Vector3(0,- Screen.height/2f + loadingGameBar.GetComponent<RectTransform>().GetHeight() - 40 ,0);

        RectTransform loadingRect = loadingGameBar.transform.GetChild(0).GetComponent<RectTransform>();
        float originalLoadingBarWidth = loadingRect.GetWidth();
        loadingRect.SetWidth(0);


        if (LoadGameScene)
        {
            myAsyncOperation = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        }


        float currentTime = 0;
        for (; ; )
        {
            float progress = 0;
            if (GetLoadingGameAsyncOp != null)
            {
                progress = GetLoadingGameAsyncOp.progress;
            }
            else
            {
                progress = currentTime / simulationLoadingTime;
            }

            if (progress > 1)
            {
                break;
            }
            float newWidth = originalLoadingBarWidth * progress;
            //loadingRect.localPosition = loadingRect.position + Vector3.left * (loadingRect.GetWidth() - newWidth);
            loadingRect.SetWidth(newWidth);


            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        //Debug.Log("___________Loading Bar should be destroyed");
        Destroy(loadingGameBar);
    }
}

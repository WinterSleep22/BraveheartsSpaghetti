using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
    }

    private CanvasGroup _canvasGroup;

    public string currentMessage = "Loading";

    Text _loadingText;
    public Text loadingText
    {
        get
        {
            if (_loadingText == null)
            {
                _loadingText = GetComponentInChildren<Text>();
            }
            return _loadingText;
        }
    }

    // Use this for initialization
    void Start()
    {
        loadingText.enabled = true;
        GetComponent<Image>().enabled = true;
        Gameplay.instance.onBothDeckIitialized += OnFinishLoading;
        StartCoroutine(LoadingTextAnimation());
    }

    // Update is called once per frame
    void OnFinishLoading()
    {

        canvasGroup.Fade(1f, 0, 1f, this);
        this.SetActiveIn(1.1f, false, this.gameObject);
        loadingText.CrossFadeAlpha(0, 1f, true);
        if (BackgroundMusicManager.instance != null)
            BackgroundMusicManager.instance.gameMusic.Play();
    }

    IEnumerator LoadingTextAnimation()
    {
        for (;;)
        {
            yield return StartCoroutine(UpdatingLoadingText(currentMessage));
        }
    }

    IEnumerator UpdatingLoadingText(string message)
    {
        float timing = 0.2f;
        loadingText.text = message + "   ";
        yield return new WaitForSeconds(timing);
        loadingText.text = message + ".  ";
        yield return new WaitForSeconds(timing);
        loadingText.text = message + ".. ";
        yield return new WaitForSeconds(timing);
        loadingText.text = message + "...";
        yield return new WaitForSeconds(timing);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    static MenuManager _instance;
    public static MenuManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MenuManager>();
            }
            return _instance;
        }
    }


    public string gameVersion = "v01";

    [HideInInspector]
    public string SearchOpponentsMenuKey = "BattleSearchOpponents";
    [HideInInspector]
    public string SearchOpponentsWifiMenuKey = "BattleSearchOpponentsWifi";

    public Canvas canvas;

    void Start()
    {

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            FirstTimeAppCheck();
            CheckForPreSelectedMenus();
        }

    }

    /// <summary>
    /// this is used by gameplay menu when player wants play again he is redirected to apropriate menu
    /// </summary>
    void CheckForPreSelectedMenus()
    {
        if (PlayerPrefs.HasKey(SearchOpponentsMenuKey))
        {
            if (PlayerPrefs.GetInt(SearchOpponentsMenuKey) == 1)
            {
                SelectSearchOpponents();
            }
        }
        if (PlayerPrefs.HasKey(SearchOpponentsWifiMenuKey))
        {
            if (PlayerPrefs.GetInt(SearchOpponentsWifiMenuKey) == 1)
            {
                SelectSearchOpponentsWifi();
            }

        }

    }

    void SelectSearchOpponents()
    {
        GetComponent<TouchAnywhere>().HideBackground();
        PlayerPrefs.SetInt(SearchOpponentsMenuKey, 0);
        GetComponentInChildren<BattleMenu>(true).MultiPlay();
    }

    void SelectSearchOpponentsWifi()
    {
        GetComponent<TouchAnywhere>().HideBackground();
        PlayerPrefs.SetInt(SearchOpponentsWifiMenuKey, 0);
        GetComponentInChildren<BattleMenu>(true).WiFi();
    }




    void FirstTimeAppCheck()
    {
        if (PlayerPrefs.HasKey(gameVersion))
        {
            if (PlayerPrefs.GetInt(gameVersion) == 0)
            {
                OnFirstTimeApp();
            }

        }

    }

    void OnFirstTimeApp()
    {
        Debug.Log("First time app verison=" + gameVersion);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(gameVersion, 1);
    }







}
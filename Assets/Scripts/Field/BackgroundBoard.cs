using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class BackgroundBoard : MonoBehaviour , IPointerDownHandler{

    private static BackgroundBoard _instance;
    public static BackgroundBoard instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BackgroundBoard>();
            }
            return _instance;
        }
    }

    public delegate void OnClickedBackground();
    public event OnClickedBackground onClickedBackground;
    public void OnPointerDown(PointerEventData eventData)
    {
      //  Debug.Log("Clicked on BackgroundBoard");
        if (onClickedBackground!=null)
        {
            onClickedBackground();
        }
    }
}

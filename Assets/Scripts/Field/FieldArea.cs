using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FieldArea : MonoBehaviour {

    private Image _myImage;
    public Image myImage
    {
        get
        {
            if (_myImage == null)
            {
                _myImage = GetComponent<Image>();
            }
            return _myImage;
        }
    }

    Player _myplayer;

    public Player myPlayer
    {
        get
        {
            if(_myplayer==null){
                _myplayer = GetComponentInParent<Player>();
            }

            return _myplayer;
        }
    }
 
    
    
}

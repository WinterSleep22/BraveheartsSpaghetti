using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeckGauge : MonoBehaviour {


    private Slider _mySlider;
    public Slider mySlider
    {
        get
        {
            if(_mySlider==null){
                _mySlider = GetComponentInChildren<Slider>();
            }
            return _mySlider;
        }
    }

    public void SetValue(float value){
        mySlider.value = value;
    }
}

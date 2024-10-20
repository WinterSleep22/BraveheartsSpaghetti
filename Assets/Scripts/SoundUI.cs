using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SoundUI : MonoBehaviour {


    public Image soundIcon;

    public AudioSource soundTest;

	public void OnValueSliderChanged (float value) {
        soundIcon.CrossFadeAlpha(value + 0.1f,0.1f,true);
        AudioListener.volume = value;
        if(!soundTest.isPlaying)
            soundTest.Play();
	}


  
}

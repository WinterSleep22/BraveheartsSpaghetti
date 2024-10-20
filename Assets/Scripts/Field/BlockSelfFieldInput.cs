﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlockSelfFieldInput : MonoBehaviour {

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


	
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ModelHeader : MonoBehaviour {

    public Text modelName;
    public Text modelStatus;

    // set model name
    public void SetName(string text) {
        modelName.text = "NAME: " + text;
    }

    // set model status
    public void SetStatus(string text)
    {
        modelStatus.text =text;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (modelStatus.text == "Normal" || modelStatus.text == "normal") {
            modelStatus.color = Color.green;
        }
        else
        {
            modelStatus.color = Color.red;
        }
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWriter : MonoBehaviour {
    GameObject canvas;

    public string toggleChild(string childName) {
        GameObject child = returnChild(childName);
        Text text = child.GetComponent<Text>();
        text.enabled = !text.enabled;
        return "Text is set to " + text.enabled;
    }

    public string writeToName(string childName) {
        return "null";
    }

    private GameObject returnChild(string childName) {
        GameObject child = canvas.gameObject.transform.Find(childName).gameObject;
        return child;
    }

    void Start() {
        canvas = GameObject.Find("Canvas");
    }

    void Update() {

    }
}

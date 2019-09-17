﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour {

    public static UISystem Instance { get; private set; }
    List<GameObject> popUps = new List<GameObject>();
    GameObject blankPoint;
    GameObject cameraRig;

    public UISystem() {
    }

    void Start() {
        blankPoint = transform.Find("Blank").gameObject;
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
    }


    public void deleteChild(GameObject popupObject) {
        popUps.Remove(popupObject);
    }

    public void CreatePopup(int point, string message) {
        GameObject copy = Instantiate(blankPoint);
        copy.transform.position = blankPoint.transform.position;
        copy.AddComponent<PointPopup>();
        copy.GetComponent<PointPopup>().setPointAndText(point, message);
        popUps.Add(copy);
    }


}
﻿using UnityEngine;


public class MenuInterface : MonoBehaviour {

    [SerializeField]
    private GameObject menuContainer;
    private Hand hand;

    public bool Visible => menuContainer.activeSelf;

    public void Close() {
        menuContainer.SetActive(!Visible);
    }

    private void Start() {
        hand = GameObject.FindGameObjectWithTag("Controller (Left)")?.GetComponent<Hand>();
    }

    private void Update() {
        if (hand != null && VRInput.GetControlDown(hand.HandType, ControlType.Menu)) {
            Close();
        } else if (hand == null) {
            Logger.Warning("Hand is Null!");
        }
    }
}
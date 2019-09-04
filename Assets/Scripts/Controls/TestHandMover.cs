﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHandMover : MonoBehaviour {

    private bool usingRight;

    Vector3 movement;

    private float handSpeed = 1;

    private Transform right, left;

    // Start is called before the first frame update
    void Start() {
        right = transform.GetChild(0);
        left = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update() {

        UpdateHandStatus();
        CheckInput();
        MoveHand();
    }

    private void UpdateHandStatus() {
        if (Input.GetKeyDown(KeyCode.Mouse0) && usingRight) {
            usingRight = false;
        } else if (Input.GetKeyDown(KeyCode.Mouse1) && !usingRight) {
            usingRight = true;
        }
    }

    private void CheckInput() {

        movement = Vector3.zero;

        if (Pressing(KeyCode.W)) {
            movement.z++;
        }
        if (Pressing(KeyCode.S)) {
            movement.z--;
        }

        if (Pressing(KeyCode.D)) {
            movement.x++;
        }
        if (Pressing(KeyCode.A)) {
            movement.x--;
        }

        if (Pressing(KeyCode.E)) {
            movement.y++;
        }
        if (Pressing(KeyCode.Q)) {
            movement.y--;
        }
    }

    private void MoveHand() {

        Transform hand = usingRight ? right : left;

        hand.Translate(movement * handSpeed * Time.deltaTime);
    }

    private bool Pressing(KeyCode c) {
        return Input.GetKey(c);
    }
}

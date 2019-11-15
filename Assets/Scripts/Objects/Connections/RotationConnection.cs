﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationConnection: ItemConnection {

    #region Fields
    protected override ItemConnector Connector { get; set; }

    private Transform target;

    private Rigidbody rb;

    private float maxDistance = 0.3f;
    #endregion

    private void Start() {
        rb = GetComponent<Rigidbody>();

        if (rb == null) {
            throw new System.Exception("Rigidbody was null");
        }
    }

    protected override void Update() {
        return;
        CheckBreakCondition();
        Rotate();
    }

    private void CheckBreakCondition() {
        if (rb.transform == null) {
            Logger.Print("Missing transform");
        }

        float distance = Vector3.Distance(rb.transform.position, target.position);
        if (distance > maxDistance) {
            OnRemoveConnection();
        }
    }

    private void Rotate() {
        rb.MoveRotation(target.rotation);
        //rb.MoveRotation(Quaternion.Lerp(transform.rotation, target.rotation, 0.2f));
    }

    public static RotationConnection Configuration(ItemConnector connector, Transform target, Interactable addTo) {
        RotationConnection conn = addTo.gameObject.AddComponent<RotationConnection>();

        conn.Connector = connector;
        conn.target = target;

        return conn;
    }
}

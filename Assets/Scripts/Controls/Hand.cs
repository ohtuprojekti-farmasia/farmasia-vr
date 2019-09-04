using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    #region fields

    private bool grab;

    private Vector3 velocity;

    private Vector3 lastPos;

    private HandCollider coll;

    private Rigidbody grabbedObject;
    #endregion

    private void Start() {
    }

    public void Grab() {

    }

    private Rigidbody GrabObject() {

        grabbedObject = coll.GetGrabObjet();

        return null;
    }

    private void Update() {
        UpdateVelocity();
    }


    private void UpdateVelocity() {
        Vector3 diff = transform.position - lastPos;
        velocity = diff / Time.deltaTime;
        lastPos = transform.position;
    }
}

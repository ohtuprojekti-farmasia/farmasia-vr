﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuerlockConnector : ItemConnector {

    #region fields
    public LuerlockAdapter Luerlock { get; private set; }
    #endregion

    public LuerlockConnector(Transform obj) : base(obj) {
        Luerlock = Object.GetComponent<LuerlockAdapter>();
    }

    #region Attaching
    public override void ConnectItem(Interactable interactable, int side) {

        if (Luerlock.State == InteractState.Grabbed) {
            Hand.GrabbingHand(Luerlock.Rigidbody).Connector.ReleaseItem(Luerlock, 0);
        }

        ReplaceObject(side, interactable?.gameObject);
    }

    private void ReplaceObject(int side, GameObject newObject) {

        GameObject colliderT = Luerlock.Colliders[side];

        LuerlockAdapter.AttachedObject obj = Luerlock.Objects[side];

        Logger.Print("ReplaceObject");
        if (obj.GameObject != null) {

            if (obj.GameObject == newObject) {
                return;
            }

            IgnoreCollisions(Luerlock.transform, obj.GameObject.transform, false);

            // attachedObject.GameObject.AddComponent<Rigidbody>();
            obj.Rigidbody.isKinematic = false;
            // attachedObject.Rigidbody.WakeUp();
            obj.GameObject.transform.parent = null;
            obj.GameObject.transform.localScale = obj.Scale;
        }

        if (newObject == null) {
            obj.Interactable = null;
            Luerlock.Objects[side] = obj;
            return;
        }

        obj.Interactable = newObject.GetComponent<Interactable>();
        obj.Scale = newObject.transform.localScale;

        IgnoreCollisions(Luerlock.transform, obj.GameObject.transform, true);

        Vector3 newScale = new Vector3(
            obj.Scale.x / Luerlock.transform.lossyScale.x,
            obj.Scale.y / Luerlock.transform.lossyScale.y,
            obj.Scale.z / Luerlock.transform.lossyScale.z);

        // Destroy(attachedObject.Rigidbody);
        obj.Rigidbody.isKinematic = true;
        //attachedObject.Rigidbody.Sleep();

        obj.GameObject.transform.parent = Luerlock.transform;
        obj.GameObject.transform.localScale = newScale;
        obj.GameObject.transform.up = colliderT.transform.up;
        SetLuerlockPosition(colliderT, obj.GameObject.transform);

        Luerlock.Objects[side] = obj;
    }

    private static void IgnoreCollisions(Transform a, Transform b, bool ignore) {

        Collider coll = a.GetComponent<Collider>();

        if (coll != null) {
            IgnoreCollisionsCollider(coll, b, ignore);
        }

        foreach (Transform child in a) {
            IgnoreCollisions(child, b, ignore);
        }
    }
    private static void IgnoreCollisionsCollider(Collider a, Transform b, bool ignore) {

        Collider coll = b.GetComponent<Collider>();

        if (coll != null) {
            Physics.IgnoreCollision(a, coll, ignore);
            foreach (Transform child in b) {
                IgnoreCollisionsCollider(a, child, ignore);
            }
        }
    }

    private void SetLuerlockPosition(GameObject collObject, Transform t) {

        Transform target = LuerlockAdapter.LuerlockPosition(t);

        if (target == null) {
            throw new System.Exception("Luerlock position not found");
        }

        Vector3 offset = collObject.transform.position - target.position;
        t.position += offset;
    }
    #endregion

    #region Releasing
    public override void ReleaseItem(Interactable Interactable, int side) {

        //if (luerlock.Interactable.State != InteractState.Grabbed) {
        //    throw new System.Exception("Trying to release ungrabbed item");
        //}


    }
    #endregion
}

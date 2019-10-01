﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughCabinet : MonoBehaviour {
    List<GameObject> objectsInsideArea;
    // Start is called before the first frame update
    void Start() {
        objectsInsideArea = new List<GameObject>();
        
    }

    private void OnTriggerEnter(Collider other) {
        objectsInsideArea.Add(other.transform.gameObject);
        UISystem.Instance.CreatePopup("Added object to Area!", MessageType.Notify);
    }

    private void OnTriggerExit(Collider other) {
        objectsInsideArea.Remove(other.transform.gameObject);
        UISystem.Instance.CreatePopup("Removed Object From Area!", MessageType.Notify);
    }

    // Update is called once per frame
    void Update() {
        
    }
}

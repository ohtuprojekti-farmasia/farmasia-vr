﻿using UnityEngine;

public class UISystem : MonoBehaviour {
    #region Fields
    public static UISystem Instance { get; private set; }

    private GameObject handuiInScene;
    [SerializeField]
    [Tooltip("Drag Popup prefab here!")]
    private GameObject popupPrefab;

    public GameObject player { get; private set; }
    private Hand hand;
    private GameObject currentPopup;

    private string descript = "";
    public string Descript { get => descript; set => descript = value; }
    #endregion



    #region Initialisation
    /// <summary>
    /// Used to instantiate Singleton of UISystem.
    /// </summary>
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        handuiInScene = GameObject.FindGameObjectWithTag("HandUI");
        hand = GameObject.FindGameObjectWithTag("Controller (Left)").GetComponent<Hand>();
    }

    /// <summary>
    /// Initiates UIComponent into players hand.
    /// </summary>
    /// <returns>Reference to the instantiated GameObject</returns>
    private GameObject InitUIComponent(GameObject gobj) {
        GameObject uiComponent = Instantiate(gobj, handuiInScene.transform);
        uiComponent.transform.SetParent(handuiInScene.transform, false);
        return uiComponent;
    }
    #endregion

    #region Popup Methods
    private void SetCurrentPopup(GameObject newPopup) {
        if (currentPopup != null) {
            Destroy(currentPopup);
        }
        currentPopup = newPopup;
    }

    /// <summary>
    /// Sets current Popup as null.
    /// </summary>
    public void DeleteCurrent() {
        currentPopup = null;
    }

    /// <summary>
    /// Used for creating a popup.
    /// </summary>
    /// <param name="point">Amount of points for the task. Some tasks do not use this.</param>
    /// <param name="message">Message to be displayed for the player.</param>
    /// <param name="type">Type of message. Different types have different colours.</param>
    public void CreatePopup(int point, string message, MsgType type) {
        GameObject popupMessage = InitUIComponent(popupPrefab);
        popupMessage.GetComponent<PointPopup>().SetPopup(point, message, type);
        SetCurrentPopup(popupMessage);
    }

    public void CreatePopup(string message, MsgType type) {
        GameObject popupMessage = InitUIComponent(popupPrefab);
        popupMessage.GetComponent<PointPopup>().SetPopup(message, type);
        SetCurrentPopup(popupMessage);
    }
    #endregion

    #region Test Functions
    private void KeyListener() {
        if (Input.GetKeyDown(KeyCode.C)) {
            CreatePopup("THIS IS A TEST", MsgType.Mistake);
        }
    }
    #endregion
}
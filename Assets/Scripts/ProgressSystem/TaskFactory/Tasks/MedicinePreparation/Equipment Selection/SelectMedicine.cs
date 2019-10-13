﻿using UnityEngine;
public class SelectMedicine : TaskBase {
    #region Fields
    private string[] conditions = { "BottlePickup" };
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor for SelectMedicine task. 
    ///  Is removed when finished and doesn't require previous task completion.
    /// </summary>
    public SelectMedicine() : base(TaskType.SelectMedicine, true, false) {
        Subscribe();
        AddConditions(conditions);
    }
    #endregion

    #region Event Subscriptions
    /// <summary>
    /// Subscribes to required Events.
    /// </summary>
    public override void Subscribe() {
        base.SubscribeEvent(PickupObject, EventType.PickupObject);
    }

    /// <summary>
    /// Once fired by an event, checks if the item is bottle and sets the corresponding condition to be true.
    /// </summary>
    /// <param name="data">Refers to the data returned by the trigger.</param>
    private void PickupObject(CallbackData data) {
        GameObject g = data.DataObject as GameObject;
        GeneralItem item = g.GetComponent<GeneralItem>();
        if (item == null) {
            return;
        }
        ObjectType type = item.ObjectType;
        if (type == ObjectType.Bottle) {
            EnableCondition("BottlePickup");
        }
        CheckClearConditions(true);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Once all conditions are true, this method is called.
    /// </summary>
    public override void FinishTask() {
        UISystem.Instance.CreatePopup("Medicine selected", MessageType.Done);
        base.FinishTask();
    }

    /// <summary>
    /// Used for getting the task's description.
    /// </summary>
    /// <returns>Returns a String presentation of the description.</returns>
    public override string GetDescription() {
        return "Valitse sopiva lääkepullo.";
    }
    
    /// <summary>
    /// Used for getting the hint for this task.
    /// </summary>
    /// <returns>Returns a String presentation of the hint.</returns>
    public override string GetHint() {
        return "Jääkaapissa on erikokoisia lääkepulloja. Valitse näistä yksi.";
    }
    #endregion
}
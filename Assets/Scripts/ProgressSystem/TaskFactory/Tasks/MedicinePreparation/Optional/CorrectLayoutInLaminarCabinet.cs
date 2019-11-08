using System;
using System.Collections.Generic;
using UnityEngine;

public class CorrectLayoutInLaminarCabinet : TaskBase {
    #region Fields
    private CabinetBase laminarCabinet;
    #endregion

    #region Constructor
    ///  <summary>
    ///  Constructor for CorrectLayoutInLaminarCabinet task.
    ///  Is moved to manager when finished and doesn't require previous task completion.
    ///  </summary>
    public CorrectLayoutInLaminarCabinet() : base(TaskType.CorrectLayoutInLaminarCabinet, false, false) {
        base.unsubscribeAllEvents = false;
        Subscribe();
        points = 1;
    }
    #endregion

    #region Event Subscriptions
    /// <summary>
    /// Subscribes to required Events.
    /// </summary>
    public override void Subscribe() {
        SubscribeEvent(SetCabinetReference, EventType.ItemPlacedInCabinet);
        SubscribeEvent(VentilationBlocked, EventType.VentilationBlocked);
        SubscribeEvent(ArrangedItems, EventType.CorrectLayoutInLaminarCabinet);
    }

    private void SetCabinetReference(CallbackData data) {
        CabinetBase cabinet = (CabinetBase)data.DataObject;
        if (cabinet.type == CabinetBase.CabinetType.Laminar) {
            laminarCabinet = cabinet;
            base.UnsubscribeEvent(SetCabinetReference, EventType.ItemPlacedInCabinet);
        }
    }

    /// <summary>
    /// Checks if items have been arranged inside Laminar Cabinet.
    /// </summary>
    /// <param name="data"></param>
    private void ArrangedItems(CallbackData data) {
        if (laminarCabinet == null) {
            return;
        }
        if (!ItemsArranged()) {
            UISystem.Instance.CreatePopup(0, "Työvälineitä ei ryhmitelty.", MsgType.Mistake);
            G.Instance.Progress.Calculator.Subtract(TaskType.CorrectLayoutInLaminarCabinet);
        }
    }

    private void VentilationBlocked(CallbackData data) {
        UISystem.Instance.CreatePopup(0, "Ilmanvaihto estynyt.", MsgType.Mistake);
        G.Instance.Progress.Calculator.Subtract(TaskType.CorrectLayoutInLaminarCabinet);
    }

    /// <summary>
    /// Checks that the items are arranged according to rules.
    /// </summary>
    /// <returns>"Returns true if the items are arranged."</returns>
    private bool ItemsArranged() {
        //code missing
        return true;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Once all conditions are true, this method is called.
    /// </summary>
    public override void FinishTask() {
        base.FinishTask();
    }

    /// <summary>
    /// Used for getting the task's description.
    /// </summary>
    /// <returns>"Returns a String presentation of the description."</returns>
    public override string GetDescription() {
        return "";
    }

    /// <summary>
    /// Used for getting the hint for this task.
    /// </summary>
    /// <returns>"Returns a String presentation of the hint."</returns>
    public override string GetHint() {
        return "";
    }
    #endregion
}
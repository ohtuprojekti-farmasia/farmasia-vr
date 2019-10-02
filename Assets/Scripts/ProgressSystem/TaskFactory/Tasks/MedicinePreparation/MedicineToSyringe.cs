using System.Collections.Generic;
using UnityEngine;
public class MedicineToSyringe : TaskBase {
    #region Fields
    private string[] conditions = { "RightAmountInSyringe", "PreviousTasksCompleted" };
    private List<TaskType> requiredTasks = new List<TaskType> {TaskType.SelectTools, TaskType.SelectMedicine};
    // COMMENTED FOR DEMO private string[] conditions = { "RightSizeSyringe", "NeedleToSyringe", "NeedleThroughBottleCap", "PreviousTasksCompleted" };
    // COMMENTED FOR DEMO private List<TaskType> requiredTasks = new List<TaskType> {TaskType.DisinfectBottles};
    #endregion

    #region Constructor
    ///  <summary>
    ///  Constructor for MedicineToSyringe task.
    ///  Is removed when finished and requires previous task completion.
    ///  </summary>
    public MedicineToSyringe() : base(TaskType.MedicineToSyringe, true, true) {
        Subscribe();
        AddConditions(conditions);
    }
    #endregion

    #region Event Subscriptions
    /// <summary>
    /// Subscribes to required Events.
    /// </summary>
    public override void Subscribe() {
        base.SubscribeEvent(ToSyringe, EventType.MedicineToSyringe);
    }
    /// <summary>
    /// Once fired by an event, checks which step of the MedicineToSyringe process has been taken and if required previous tasks are completed.
    /// Sets corresponding conditions to be true.
    /// </summary>
    /// <param name="data">"Refers to the data returned by the trigger."</param>
    private void ToSyringe(CallbackData data) {
        GameObject g = data.DataObject as GameObject;
        GeneralItem item = g.GetComponent<GeneralItem>();
        if (item == null) {
            return;
        }
        ObjectType type = item.ObjectType;
        
        if (type == ObjectType.Syringe) {
            Syringe syringe = item as Syringe;
            if (syringe.GetContainer().Amount == 20) {
                    EnableCondition("RightAmountInSyringe");
            }
        }

        if (CheckPreviousTaskCompletion(requiredTasks)) {
            EnableCondition("PreviousTasksCompleted");
        }

        bool check = CheckClearConditions(true);
        if (!check && base.clearConditions["PreviousTasksCompleted"]) {
            UISystem.Instance.CreatePopup(-1, "Wrong amount of medicine", MessageType.Mistake);
            G.Instance.Progress.calculator.Subtract(TaskType.MedicineToSyringe);
            base.FinishTask();
        }
    }
    /* COMMENTED FOR DEMO private void ToSyringe(CallbackData data) {
        GameObject g = data.DataObject as GameObject;
        GeneralItem item = g.GetComponent<GeneralItem>();
        if (item == null) {
            return;
        }
        ObjectType type = item.ObjectType;
        switch (type) {
            case ObjectType.Syringe:
                Syringe syringe = item as Syringe;
                if (syringe.GetContainer().Capacity == 20) {
                    EnableCondition("RightSizeSyringe");
                }
                break;
            case ObjectType.Needle:
                EnableCondition("NeedleToSyringe");
                break;
            case ObjectType.BottleCap:
                //check if the needle goes through the bottlecap
                EnableCondition("NeedleThroughBottleCap");
                break;
        }

        if (CheckPreviousTaskCompletion(requiredTasks)) {
            EnableCondition("PreviousTasksCompleted");
        }

        bool check = CheckClearConditions(true);
        if (!check && base.clearConditions["NeedleToSyringe"] && base.clearConditions["NeedleThroughBottleCap"] 
                && base.clearConditions["PreviousTasksCompleted"]) {
            UISystem.Instance.CreatePopup(-1, "Medicine was not successfully taken", MessageType.Mistake);
            G.Instance.Progress.calculator.Subtract(TaskType.MedicineToSyringe);
            base.FinishTask();
        }
    } */
    #endregion

    #region Public Methods
    /// <summary>
    /// Once all conditions are true, this method is called.
    /// </summary>
    public override void FinishTask() {
        UISystem.Instance.CreatePopup(1, "Medicine was successfully taken", MessageType.Notify);
        G.Instance.Progress.calculator.Add(TaskType.MedicineToSyringe);
        base.FinishTask();
    }
    
    /// <summary>
    /// Used for getting the task's description.
    /// </summary>
    /// <returns>"Returns a String presentation of the description."</returns>
    public override string GetDescription() {
        return "Ota ruiskulla lääkettä lääkeainepullosta.";
    }

    /// <summary>
    /// Used for getting the hint for this task.
    /// </summary>
    /// <returns>"Returns a String presentation of the hint."</returns>
    public override string GetHint() {
        return "Valitse oikeankokoinen ruisku, jolla otat lääkettä lääkeainepullosta. Varmista, että ruiskuun on kiinnitetty neula.";
    }
    #endregion
}
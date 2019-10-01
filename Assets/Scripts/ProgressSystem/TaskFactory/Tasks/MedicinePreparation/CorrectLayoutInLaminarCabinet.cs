using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CorrectLayoutInLaminarCabinet : TaskBase {
    #region Fields
    private string[] conditions = {"AllItems", "ItemsArranged"};
    #endregion

    #region Constructor
    ///  <summary>
    ///  Constructor for Layout2 task.
    ///  Is removed when finished and doesn't require previous task completion.
    ///  </summary>
    public CorrectLayoutInLaminarCabinet() : base(TaskType.CorrectLayoutInLaminarCabinet, true, false) {
        Subscribe();
        AddConditions(conditions);
    }
    #endregion

    #region Event Subscriptions
    /// <summary>
    /// Subscribes to required Events.
    /// </summary>
    public override void Subscribe() {
        base.SubscribeEvent(FinalArrangeItems, EventType.FinalArrangeItems);
    }
    /// <summary>
    /// Once fired by an event, checks if the tasks dealing with the amount of items have been completed and if the items are arranged.
    /// Sets the corresponding conditions to be true.
    /// </summary>
    /// <param name="data">"Refers to the data returned by the trigger."</param>
    private void FinalArrangeItems(CallbackData data) {
        GameObject g = data.DataObject as GameObject;
        if (G.Instance.Progress.doneTypes.Contains(TaskType.CorrectItemsInLaminarCabinet)) {
            List<ITask> list = G.Instance.Progress.activeTasks;
            int exists = 0;
            exists = (from n in list
                    where n.GetTaskType().Equals(TaskType.MissingItems)
                    select n).Count();
            if (exists == 0) {
                EnableCondition("AllItems"); 
                if (ItemsArranged()) {
                    EnableCondition("ItemsArranged");
                }
            } 
        }
        
        bool check = CheckClearConditions(true);
        if (!check && base.clearConditions["AllItems"]) {
            UISystem.Instance.CreatePopup(-1, "Items not arranged", MessageType.Mistake);
            G.Instance.Progress.calculator.Subtract(TaskType.CorrectLayoutInLaminarCabinet); 
            base.FinishTask();
            FinishLayout1();
        }
    }
    /// <summary>
    /// Checks that the items are arranged according to rules.
    /// </summary>
    /// <returns>"Returns true if the items are arranged."</returns>
    private bool ItemsArranged() {
        //code missing
        return false;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Once all conditions are true, this method is called.
    /// </summary>
    public override void FinishTask() {
        UISystem.Instance.CreatePopup(1, "Items in order", MessageType.Notify);
        G.Instance.Progress.calculator.Add(TaskType.CorrectLayoutInLaminarCabinet);
        base.FinishTask();
        FinishLayout1();
    }

    /// <summary>
    /// Removes Layout1 task if not removed before.
    /// </summary>
    public void FinishLayout1() {
        if (!G.Instance.Progress.doneTypes.Contains(TaskType.CorrectLayoutInThroughput)) {
            CorrectLayoutInThroughput layoutInstance = G.Instance.Progress.activeTasks.Find(x => x.GetTaskType().Equals(TaskType.CorrectLayoutInThroughput)) as CorrectLayoutInThroughput;
            layoutInstance.RemoveTaskFromOutside();
        }
    }

    /// <summary>
    /// Used for getting the task's description.
    /// </summary>
    /// <returns>"Returns a String presentation of the description."</returns>
    public override string GetDescription() {
        return "Siirrä välineet läpiantokaapista kaappiin.";
    }
    
    /// <summary>
    /// Used for getting the hint for this task.
    /// </summary>
    /// <returns>"Returns a String presentation of the hint."</returns>
    public override string GetHint() {
        return "Vie ja asettele valitsemasi työvälineet sekä lääkepullo läpiantokaapista kaappiin."; 
    }
    #endregion
}
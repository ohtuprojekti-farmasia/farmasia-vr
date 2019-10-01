﻿using System.Collections.Generic;

/// <summary>
/// Version 2 of current task.
/// </summary>
public class TaskBase : ITask {
    #region Fields
    protected ProgressManager manager;
    protected TaskType taskType;
    protected bool isFinished = false;
    protected bool removeWhenFinished = false;
    protected bool requiresPreviousTaskCompletion = false;
    protected bool previousTasksCompleted = false;
    protected Dictionary<string, bool> clearConditions = new Dictionary<string, bool>();
    protected Dictionary<Events.EventDataCallback, EventType> subscribedEvents = new Dictionary<Events.EventDataCallback, EventType>();
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor for Task Base.
    /// </summary>
    /// <param name="remove">Task removed when finished from list.</param>
    /// <param name="previous">Task requires previous tasks completion linearly.</param>
    public TaskBase(TaskType type, bool remove, bool previous) {
        taskType = type;
        removeWhenFinished = remove;
        requiresPreviousTaskCompletion = previous;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Removes current task if the task has been set to be removed.
    /// </summary>
    private void Remove() {
        if (removeWhenFinished) {
            manager.RemoveTask(this);
        }
    }
    #endregion

    #region Virtual Methods
    /// <summary>
    /// Used for finishing current task. Task is either removed or preserved.
    /// </summary>
    public virtual void FinishTask() {
        UnsubscribeAllEvents();
        Remove();
    }

    /// <summary>
    /// Used for getting task's description to show on UI.
    /// </summary>
    /// <returns>
    /// Returns string presentation of description.
    /// </returns>
    public virtual string GetDescription() {
        return "No Description";
    }

    /// <summary>
    /// Used for getting task's hint when hint trigger is triggered.
    /// </summary>
    /// <returns>
    /// Return string presentation of hint.
    /// </returns>
    public virtual string GetHint() {
        return "No Hints";
    }

    /// <summary>
    /// Used for defining custom Subscribtions per task. (Override)
    /// </summary>
    public virtual void Subscribe() {
    }
    #endregion

    #region Public Methods
    public void SetReferredManager(ProgressManager manager) {
        this.manager = manager;
    }
    /// <summary>
    /// Return the type of current task.
    /// </summary>
    /// <returns>returns TaskType enum.</returns>
    public TaskType GetTaskType() {
        return taskType;
    }

    /// <summary>
    /// Enables condition with given string.
    /// </summary>
    /// <param name="condition">String representation of condition.</param>
    public void EnableCondition(string condition) {
        if (clearConditions.ContainsKey(condition)) {
            clearConditions[condition] = true;
        }
    }

    /// <summary>
    /// Disables all conditions.
    /// </summary>
    public void DisableConditions() {
        foreach (KeyValuePair<string, bool> condition in clearConditions) {
            clearConditions[condition.Key] = false;
        }
    }

    /// <summary>
    /// Adds conditions with list of string conditions.
    /// </summary>
    /// <param name="conditions">List of string conditions</param>
    public void AddConditions(string[] conditions) {
        foreach (string condition in conditions) {
            clearConditions.Add(condition, false);
        }
    }

    /// <summary>
    /// Subscribes to Events and adds them to a Dictionary.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="Event"></param>
    public void SubscribeEvent(Events.EventDataCallback action, EventType Event) {
        Events.SubscribeToEvent(action, Event);
        subscribedEvents.Add(action, Event);
    }

    /// <summary>
    /// Unsubscribes from all events inside Dictionary.
    /// </summary>
    public void UnsubscribeAllEvents() {
        foreach (Events.EventDataCallback action in subscribedEvents.Keys) {
            Events.UnsubscribeFromEvent(action, subscribedEvents[action]);
        }
    }
    #endregion

    #region Protected Methods
    protected bool CheckIsCurrent() {
        if (manager.activeTasks.IndexOf(this) > 0) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Used for checking if certain previous tasks before current task are completed.
    /// </summary>
    /// <returns>
    /// Returns true if previous tasks are completed, otherwise false.
    /// </returns>
    protected bool CheckPreviousTaskCompletion(List<TaskType> tasks) {
        List<TaskType> completed = manager.doneTypes;
        foreach (TaskType type in tasks) {
            if (!completed.Contains(type)) {
                return false;
            }
        }
        previousTasksCompleted = true;
        return true;
    }

    /// <summary>
    /// Checks if conditions have been cleared.
    /// </summary>
    /// <param name="checkAll"></param>
    /// <returns></returns>
    protected bool CheckClearConditions(bool checkAll) {
        if (checkAll) {
            if (!clearConditions.ContainsValue(false)) {
                FinishTask();
                return true;
            }
            return false;
        }
        if (clearConditions.ContainsValue(true)) {
            FinishTask();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns an array of the conditions that have not yet been cleared.
    /// </summary>
    /// <returns>An array of the names of the conditions (strings)</returns>
    protected string[] GetNonClearedConditions() {
        List<string> nonCleared = new List<string>();
        foreach (KeyValuePair<string, bool> condition in clearConditions) {
            if (!condition.Value) {
                nonCleared.Add(condition.Key);
            }
        }
        return nonCleared.ToArray();
    }
    #endregion
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package {

    #region Fields
    private ProgressManager manager;
    public PackageName name { get; private set; }
    public bool packageCompleted { get; private set; }
    public List<ITask> activeTasks { get; private set; }
    public List<TaskType> doneTypes { get; private set; }

    public ITask CurrentTask {
        get {
            if (activeTasks.Count == 0) {
                return null;
            }
            return activeTasks[0];
        }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initiates all fields.
    /// </summary>
    /// <param name="name">Name of the package.</param>
    /// <param name="manager">Reference to manager that created this package.</param>
    public Package(PackageName name, ProgressManager manager) {
        this.name = name;
        this.manager = manager;
        packageCompleted = false;
        activeTasks = new List<ITask>();
        doneTypes = new List<TaskType>();
    }
    #endregion

    #region Task Addition
    /// <summary>
    /// Adds a task and updates description.
    /// </summary>
    /// <param name="task">Given task to add.</param>
    public void AddTask(ITask task) {
        task.SetPackage(this);
        activeTasks.Add(task);
        manager.UpdateDescription();
    }

    /// <summary>
    /// Adds a task before certain index.
    /// </summary>
    /// <param name="task">New task to add before index.</param>
    /// <param name="previousTask">Refers to the task that called this method.</param>
    public void AddNewTaskBeforeTask(ITask task, ITask previousTask) {
        if (activeTasks.Contains(previousTask)) {
            task.SetPackage(this);
            activeTasks.Insert(activeTasks.IndexOf(previousTask), task);
            manager.UpdateDescription();
        }
    }
    #endregion

    #region Task Removal
    /// <summary>
    /// Removes given task, when empty changes package in manager.
    /// </summary>
    /// <param name="task">Reference to given task.</param>
    public void RemoveTask(ITask task) {
        if (activeTasks.Contains(task)) {
            doneTypes.Add(task.GetTaskType());
            activeTasks.Remove(task);
            G.Instance.Audio.Play(AudioClipType.TaskCompletedBeep);
            CheckChangePackage();
            manager.UpdateDescription();
            StartTask();
        }
    }
    #endregion

    private void CheckChangePackage() {
        if (activeTasks.Count == 0) {
            packageCompleted = true;
            manager.ChangePackage();
        } else {
            Debug.Log("Still " + activeTasks.Count + " left in package: " + name);
        }
    }

    #region Task Movement
    /// <summary>
    /// Moves task back to ProgressManager
    /// </summary>
    /// <param name="task">Reference to the task that will be moved</param>
    public void MoveTaskToManager(ITask task) {
        if (activeTasks.Contains(task)) {
            Logger.Print("Moving task back to manager.");
            manager.AddTask(task);
            activeTasks.Remove(task);
            CheckChangePackage();
            manager.UpdateDescription();
            StartTask();
        }
    }

    /// <summary>
    /// Moves task from ProgressManager
    /// </summary>
    /// <param name="type">Given type of task to move.</param>
    public void MoveTaskFromManager(TaskType type) {
        manager.MoveToPackage(this, type);
    }

    /// <summary>
    /// Moves task from ProgressManager before given task
    /// </summary>
    /// <param name="type">Given type of task to move.</param>
    /// <param name="previousTask">Task point where given task will be moved</param>
    public void MoveTaskFromManagerBeforeTask(TaskType type, ITask previousTask) {
        manager.MoveToPackageBeforeTask(this, type, previousTask);
    }
    #endregion

    #region Helpful Methods
    public void PrintAllTasks() {
        Logger.Print("Package name: " + name + "\nThese tasks are inside currently\n");
        foreach (ITask task in activeTasks) {
            Logger.Print(task.GetType());
        }
    }

    public void StartTask() {
        if (CurrentTask != null) {
            CurrentTask.StartTask();
        }
    }

    #endregion
}
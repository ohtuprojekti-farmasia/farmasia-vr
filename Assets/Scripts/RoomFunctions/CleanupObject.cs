﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanupObject : MonoBehaviour {

    [SerializeField]
    private Collider coll;

    [SerializeField]
    private CabinetBase laminarCabinet, secondPassThroughCabinet;

    private TriggerInteractableContainer roomItems;

    private bool startedCleanup;
    private bool finished;

    private void Start() {
        
        roomItems = coll.gameObject.AddComponent<TriggerInteractableContainer>();
        Events.SubscribeToEvent(ItemLiftedFromFloor, this, EventType.ItemLiftedOffFloor);
        Events.SubscribeToEvent(ItemDroppedInTrash, this, EventType.ItemDroppedInTrash);
    }

    public static CleanupObject GetCleanup() {
        return GameObject.FindObjectOfType<CleanupObject>();
    }

    private void ItemLiftedFromFloor(CallbackData data) {
        GeneralItem item = (GeneralItem)data.DataObject;

        if (G.Instance.Progress.CurrentPackage.name == PackageName.EquipmentSelection) {
            return;
        }

        if (!startedCleanup && !item.IsClean) {
            UISystem.Instance.CreatePopup(-1, "Siivoa lattialla olevat esineet vasta lopuksi", MsgType.Mistake);
            G.Instance.Progress.Calculator.AddTaskMistake("Siivoa lattialla olevat esineet vasta lopuksi");
            G.Instance.Progress.Calculator.SubtractWithScore(TaskType.ScenarioOneCleanUp, 1);
        }
    }
    private void ItemDroppedInTrash(CallbackData data) {
        GeneralItem g = (GeneralItem)data.DataObject;

        if (G.Instance.Progress.CurrentPackage.name == PackageName.EquipmentSelection) {
            return;
        }

        if (g.ObjectType == ObjectType.Bottle) {
            UISystem.Instance.CreatePopup(-1, "Pulloa ei saa heittää roskikseen", MsgType.Mistake);
            G.Instance.Progress.Calculator.AddTaskMistake("Pulloa ei saa heittää roskikseen");
            G.Instance.Progress.Calculator.SubtractWithScore(TaskType.ScenarioOneCleanUp, 1);
        }
        if (g.ObjectType == ObjectType.SterileBag) {
            UISystem.Instance.CreatePopup(-1, "Steriilipussia ei saa heittää roskikseen", MsgType.Mistake);
            G.Instance.Progress.Calculator.AddTaskMistake("Steriilipussia ei saa heittää roskikseen");
            G.Instance.Progress.Calculator.SubtractWithScore(TaskType.ScenarioOneCleanUp, 1);
        }
    }

    private void Update() {
        if (startedCleanup && !finished) {
            if (RoomGeneralItemCount() <= AcceptedCount()) {
                finished = true;
                Logger.Warning("Finishing cleanup");
                G.Instance.Progress.ForceCloseTask(TaskType.ScenarioOneCleanUp, false);
            }
        }
    }

    private int RoomGeneralItemCount() {
        int count = 0;
        foreach (Interactable i in roomItems.Objects) {
            if (i as GeneralItem is var g && g != null) {
                if (g.ObjectType == ObjectType.Bottle || g.ObjectType == ObjectType.SterileBag) {
                    continue;
                }
                if (g.Rigidbody == null || g.Rigidbody.isKinematic) {
                    continue;
                }
                //Logger.Print("Clean up item: " + count + ", " + i.name);
                count++;
            }
        }

        return count;
    }

    private int AcceptedCount() {
        int acc = 1 + secondPassThroughCabinet.GetContainedItems().Count;
        return acc;
    }

    public void EnableCleanup() {

        Logger.Print("enable cleanup");

        ObjectFactory.DestroyAllFactories(true);

        foreach (Interactable i in secondPassThroughCabinet.GetContainedItems()) {
            i.DestroyInteractable();
        }
        //foreach (ItemSpawner i in GameObject.FindObjectsOfType<ItemSpawner>()) {
        //    Destroy(i);
        //}

        startedCleanup = true;
    }
}
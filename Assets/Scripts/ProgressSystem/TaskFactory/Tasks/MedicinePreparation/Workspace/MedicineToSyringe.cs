using System;
using System.Collections.Generic;
using UnityEngine;
public class MedicineToSyringe : TaskBase {

    #region Constants
    private const int RIGHT_SYRINGE_CAPACITY = 20000;
    private const int MINIMUM_AMOUNT_OF_MEDICINE_IN_BIG_SYRINGE = 900;

    private const string DESCRIPTION = "Valmistele välineet ja ota ruiskulla ja neulalla lääkettä lääkeainepullosta.";
    private const string HINT = "Valitse oikeankokoinen ruisku (20ml), jolla otat lääkettä lääkeainepullosta. Varmista, että ruiskuun on kiinnitetty neula.";

    private Syringe syringe;
    #endregion

    #region Fields
    // private Dictionary<int, int> syringes = new Dictionary<int, int>();
    public enum Conditions { }
    private List<TaskType> requiredTasks = new List<TaskType> { TaskType.CorrectItemsInLaminarCabinet };
    private CabinetBase laminarCabinet;
    #endregion

    #region States
    private bool takenBeforeTime = false;
    #endregion

    #region Constructor
    public MedicineToSyringe() : base(TaskType.MedicineToSyringe, true, true) {
        SetCheckAll(true);
        Subscribe();
        AddConditions((int[])Enum.GetValues(typeof(Conditions)));
        points = 2;
    }
    #endregion

    #region Event Subscriptions
    public override void Subscribe() {
        SubscribeEvent(SetCabinetReference, EventType.ItemPlacedForReference);
        SubscribeEvent(NeedleWithSyringeInsertedIntoBottle, EventType.SyringeWithNeedleEntersBottle);
        SubscribeEvent(FinishedTakingMedicineToSyringe, EventType.FinishedTakingMedicineToSyringe);
        SubscribeEvent(TakingMedicineFromBottle, EventType.TakingMedicineFromBottle);
    }

    private void SetCabinetReference(CallbackData data) {
        CabinetBase cabinet = (CabinetBase)data.DataObject;
        if (cabinet.type == CabinetBase.CabinetType.Laminar) {
            laminarCabinet = cabinet;
            base.UnsubscribeEvent(SetCabinetReference, EventType.ItemPlacedForReference);
        }
    }

    private void NeedleWithSyringeInsertedIntoBottle(CallbackData data) {
        Syringe s = data.DataObject as Syringe;
        if (!IsPreviousTasksCompleted(requiredTasks) && G.Instance.Progress.CurrentPackage.name == PackageName.Workspace) {
            Popup("Siirrä kaikki tarvittavat työvälineet ensin laminaarikaappiin.", MsgType.Notify);
            G.Instance.Progress.ForceCloseTask(TaskType.DisinfectBottles, true);
            G.Instance.Progress.ForceCloseTask(TaskType.CorrectItemsInLaminarCabinet, false);
        }
    }

    private void TakingMedicineFromBottle(CallbackData data) {
        Syringe s = data.DataObject as Syringe;
        if (s.BottleContainer.Capacity == 100000) {
            CreateTaskMistake("Ruiskulla otettiin väärää lääkettä", 5);
        }
    }

    private void FinishedTakingMedicineToSyringe(CallbackData data) {
        syringe = (Syringe)data.DataObject;

        if (laminarCabinet == null) {
            if (!takenBeforeTime) {
                CreateTaskMistake("Lääkettä yritettiin ottaa liian aikaisin", 1);
                takenBeforeTime = true;
            } else {
                Popup("Lääkettä yritettiin ottaa liian aikaisin.", MsgType.Mistake);
            }
        } else if (!laminarCabinet.GetContainedItems().Contains(syringe)) {
            CreateTaskMistake("Lääkettä yritettiin ottaa laminaarikaapin ulkopuolella", 1);
        } else {
            if (!IsPreviousTasksCompleted(requiredTasks)) {
                CreateTaskMistake("Tarvittavia työvälineitä ei siirretty laminaarikaappiin", 1);
            }
            //CheckConditions(s);
        }
        CompleteTask();
    }
    #endregion

    #region Public Methods

    public override string GetDescription() {
        return DESCRIPTION;
    }

    public override string GetHint() {
        return HINT;
    }

    protected override void OnTaskComplete() {
        (G.Instance.Scene as MedicinePreparationScene).NeedleUsed = true;

        if (syringe == null) {
            return;
        }

        bool fail = false;
        if (syringe.Container.Capacity != RIGHT_SYRINGE_CAPACITY) {
            CreateTaskMistake("Väärän kokoinen ruisku", 1);
            fail = true;
        }
        if (syringe.Container.Amount < MINIMUM_AMOUNT_OF_MEDICINE_IN_BIG_SYRINGE) {
            CreateTaskMistake("Liian vähän lääkettä", 1);
            fail = true;
        }
        if (!syringe.IsClean) {
            CreateTaskMistake("Ruisku tai pullo oli likainen", 1);
            fail = true;
        }

        if (!fail) {
            Popup("Lääkkeen ottaminen onnistui.", MsgType.Done, 2);
        }
    }
    #endregion
}
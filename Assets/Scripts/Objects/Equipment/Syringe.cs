﻿using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class Syringe : GeneralItem {

    #region Constants
    private const float SWIPE_DEFAULT_TIME = 0.75f;
    private const float LIQUID_TRANSFER_SPEED = 15;
    #endregion

    #region fields
    public LiquidContainer Container { get; private set; }

    [SerializeField]
    private int LiquidTransferStep = 50;

    [SerializeField]
    private float defaultPosition, maxPosition;

    [SerializeField]
    private Transform handle;

    private GameObject syringeCap;
    public bool HasSyringeCap { get { return syringeCap.activeInHierarchy; } }

    public LiquidContainer BottleContainer { get; set; }

    public bool hasBeenInBottle;


    private GameObject liquidDisplay;
    private GameObject currentDisplay;
    private bool displayState;
    #endregion

    protected override void Start() {
        base.Start();

        Container = LiquidContainer.FindLiquidContainer(transform);
        Assert.IsNotNull(Container);
        ObjectType = ObjectType.Syringe;

        Type.On(InteractableType.Attachable, InteractableType.HasLiquid, InteractableType.Interactable, InteractableType.SmallObject);

        Container.OnAmountChange += SetSyringeHandlePosition;
        SetSyringeHandlePosition();

        hasBeenInBottle = false;

        syringeCap = transform.Find("syringe_cap").gameObject;
        NullCheck.Check(syringeCap);

        syringeCap.SetActive(false);

        liquidDisplay = Resources.Load<GameObject>("Prefabs/LiquidDisplay");
        displayState = false;
    }

    public void EnableDisplay() {
        if (displayState) {
            return;
        }

        displayState = true;
        currentDisplay = Instantiate(liquidDisplay);
        SyringeDisplay display = currentDisplay.GetComponent<SyringeDisplay>();
        display.SetFollowedObject(gameObject);

        EnableForOtherSyringeDisplay();
    }

    public void DisableDisplay() {
        if (State != InteractState.LuerlockAttached && State != InteractState.Grabbed) {
            DestroyDisplay();
        }
    }

    public void DestroyDisplay() {
        if (currentDisplay != null) {
            Destroy(currentDisplay);
        }

        displayState = false;
    }

    private void EnableForOtherSyringeDisplay() {
        if (State == InteractState.LuerlockAttached && (Interactors.LuerlockPair.Value.ObjectCount == 2)) {
            Syringe other = (Syringe)Interactors.LuerlockPair.Value.GetOtherInteractable(this);
            other.EnableDisplay();
        }
    }

    public override void OnGrabStart(Hand hand) {
        base.OnGrabStart(hand);

        EnableDisplay();
    }

    public override void OnGrabEnd(Hand hand) {
        base.OnGrabEnd(hand);

        DisableDisplay();
    }

    public override void OnGrab(Hand hand) {
        base.OnGrab(hand);

        bool takeMedicine = VRInput.GetControlDown(hand.HandType, Controls.TakeMedicine);
        bool sendMedicine = VRInput.GetControlDown(hand.HandType, Controls.EjectMedicine);

        int liquidAmount = 0;

        if (takeMedicine) liquidAmount -= LiquidTransferStep;
        if (sendMedicine) liquidAmount += LiquidTransferStep;
        if (liquidAmount == 0) return;

        if (this.HasSyringeCap) {
            Logger.Warning("Cannot change liquid amount of syringe with a cap");
            return;
        }

        if (State == InteractState.LuerlockAttached && Interactors.LuerlockPair.Value.ObjectCount == 2) {
            TransferToLuerlock(liquidAmount);
        } else if (State == InteractState.InBottle) {
            TransferToBottle(liquidAmount);
            Events.FireEvent(EventType.TakingMedicineFromBottle, CallbackData.Object(this));
        } else {
            Eject(liquidAmount);
        }
    }

    private void Eject(int amount) {
        if (amount > 0) Container.SetAmount(Container.Amount - amount);
    }

    private void TransferToLuerlock(int amount) {
        bool pushing = amount > 0;

        var pair = Interactors.LuerlockPair;

        if (pair.Key < 0 || pair.Value == null) {
            return;
        }

        Syringe other = (Syringe)pair.Value.LeftConnector.AttachedInteractable != this ?
            (Syringe)pair.Value.LeftConnector.AttachedInteractable :
            (Syringe)pair.Value.RightConnector.AttachedInteractable;

        if (pushing) {
            if (other.Container.Capacity < Container.Capacity) {
                Events.FireEvent(EventType.PushingToSmallerSyringe);
            }
        }

        Container.TransferTo(other.Container, amount);
    }

    private void TransferToBottle(int amount) {
        if (BottleContainer == null) return;
        if (Vector3.Angle(-BottleContainer.transform.up, transform.up) > 25) return;

        Container.TransferTo(BottleContainer, amount);
    }

    public void SetSyringeHandlePosition() {
        Vector3 pos = handle.localPosition;
        pos.y = SyringePos();
        handle.localPosition = pos;
    }

    public void ShowSyringeCap(bool show) {
        syringeCap.SetActive(show);
    }

    private float SyringePos() {
        return Factor * (maxPosition - defaultPosition);
    }

    private float Factor {
        get {
            return 1.0f * Container.Amount / Container.Capacity;
        }
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class LiquidContainer : MonoBehaviour {

    #region fields
    [SerializeField]
    private LiquidObject liquid;

    public delegate void AmountChange();
    public AmountChange OnAmountChange { get; set; }

    [SerializeField]
    private int amount;

    private GeneralItem item;

    public int Amount {
        get { return amount; }
    }

    public void SetAmountPercentage(float percentage) {
        int amount = (int)(percentage * Capacity);
        SetAmount(amount);
    }
    public void SetAmount(int value) {
        if (Capacity == 0) {
            amount = 0;
            liquid?.SetFillPercentage(0);
        } else {
            amount = Math.Max(Math.Min(value, Capacity), 0);
            // liquid is null when OnValidate is called twice before Awake
            // when playing in Editor Mode
            // See: https://forum.unity.com/threads/onvalidate-called-twice-when-pressing-play-in-the-editor.430250/
            float percentage = (float)amount / capacity;
            liquid?.SetFillPercentage(percentage);
        }
        OnAmountChange?.Invoke();
    }

    [SerializeField]
    private int capacity;
    public int Capacity {
        get { return capacity; }
        private set { capacity = Math.Max(value, 0); }
    }
    #endregion

    private void Awake() {
        Assert.IsNotNull(liquid);
    }


    private void Start() {
        GetComponent<MeshRenderer>().enabled = false;

        StartCoroutine(SearchInteractable());

        IEnumerator SearchInteractable() {

            yield return null;

            Interactable interactable = Interactable.GetInteractable(transform);

            Logger.Print("interactable found: " + interactable.name);

            item = (GeneralItem)interactable;

            if (item == null) {
                throw new Exception("Liquid container attached to non GeneralItem object");
            }
        }
    }

    private void OnValidate() {
        Capacity = capacity;
        SetAmount(amount);
    }

    public int GetReceiveCapacity() {
        return Capacity - Amount;
    }

    public void TransferTo(LiquidContainer target, int amount) {
        if (amount < 0) {
            throw new ArgumentException("value must be non-negative", "amount");
        }

        // If target == null, it means you're emptying the source
        // container.
        int receiveCapacity = target?.GetReceiveCapacity() ?? int.MaxValue;
        int canSend = Math.Min(Amount, amount);
        int toTransfer = Math.Min(canSend, receiveCapacity);

        SetAmount(Amount - toTransfer);
        if (target != null) {
            target.SetAmount(target.Amount + toTransfer);
        }
    }

    public static LiquidContainer FindLiquidContainer(Transform t) {

        LiquidContainer c = t.GetComponent<LiquidContainer>();

        if (c != null) {
            return c;
        }

        return t.Find("Liquid")?.GetComponent<LiquidContainer>();
    }

    private void OnTriggerEnter(Collider c) {

        Logger.Print("Liquid container enter: " + c.gameObject.name);

        Syringe syringe = Interactable.GetInteractable(c.transform) as Syringe;

        if (syringe == null) {
            Logger.Print("No syringe");
            return;
        }

        if (item.ObjectType == ObjectType.Bottle) {
            syringe.State.On(InteractState.InBottle);
        }

        Logger.Print("In syringe");

        syringe.BottleContainer = this;
    }
    private void OnTriggerExit(Collider c) {

        Syringe syringe = Syringe.GetInteractable(c.transform) as Syringe;

        if (syringe == null) {
            return;
        }

        if (item.ObjectType == ObjectType.Bottle) {
            syringe.State.Off(InteractState.InBottle);
        }

        syringe.BottleContainer = null;
    }
}

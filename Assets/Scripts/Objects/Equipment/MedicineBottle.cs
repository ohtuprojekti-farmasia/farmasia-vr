﻿using UnityEngine;
using UnityEngine.Assertions;

public class MedicineBottle : GeneralItem {

    #region fields
    public LiquidContainer Container { get; private set; } 
    #endregion

    protected override void Start_GeneralItem() {
        Container = LiquidContainer.FindLiquidContainer(transform);
        Assert.IsNotNull(Container);
        ObjectType = ObjectType.Bottle;
    }
}

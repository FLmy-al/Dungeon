using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveItem : Item
{
    protected new void Awake()
    {
        base.Awake();
        InitOccupy();
    }

    //被动效果
    public abstract void PassiveAbility();
    //初始化物品大小
    public abstract void InitOccupy();
}

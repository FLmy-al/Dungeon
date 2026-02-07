using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine;

public abstract class ActiveItem : Item,IPointerDownHandler
{
    protected int currentCD;
    protected int CD;
    protected new void Awake()
    {
        base.Awake();
        InitOccupy();
    }

    protected new void Start()
    {
        base.Start();
        InitCD();
        currentCD = CD;
    }
    //主动效果
    public abstract void ActiveAbility();
    //设置cd
    public bool UseItem()
    {
        if(currentCD >= CD)
        {
            currentCD = 0;
            return true;
        }else
        {
            UnityEngine.Debug.Log("冷却中");
            return false;
        }
    }

    public void ChangeCD(int change)
    {
        currentCD += change;
    }
    //鼠标点击
    public abstract void OnPointerDown(PointerEventData eventData);
    //初始化物品大小
    public abstract void InitOccupy();
    //初始化CD
    public abstract void InitCD();

}

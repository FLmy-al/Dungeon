using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine;
using Unity.VisualScripting;

public abstract class ActiveItem : Item,IPointerDownHandler
{
    protected int currentCD;
    protected abstract int CD { get; set; }

    protected void Start()
    {
        currentCD = CD;
    }
    //主动效果
    public abstract void ActiveAbility();
    //使用物品
    public bool CheckItemCD()
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
    //鼠标点击
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (AttackController.Instance.isFighting)
        {
            if (CheckItemCD())
            {
                ActiveAbility();
                SetAlpha(currentCD, CD);
                EventManager.Instance.TriggerEvent(EventTypes.UseItemEvent, new UseItemEvent(this));
            }
        }
    }

    public void SetItemCurrentCD(int num)
    {
        currentCD = num;
        SetAlpha(currentCD, CD);
    }

    public int GetItemCurrentCD()
    {
        return currentCD;
    }

    public int GetItemCD()
    {
        return CD;
    }

    public void SetItemCD(int num)
    {
        CD = num;
        SetAlpha(currentCD, CD);
    }
}

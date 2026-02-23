using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine;

public abstract class ActiveItem : Item,IPointerDownHandler
{
    protected int currentCD;
    protected abstract int CD { get; }

    protected void Start()
    {
        currentCD = CD;
    }
    //主动效果
    public abstract void ActiveAbility();
    //使用物品
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
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (AttackController.Instance.isFighting)
        {
            if (UseItem())
            {
                ActiveAbility();
                SetAlpha(currentCD, CD);
            }
        }
    }

}

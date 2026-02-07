using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageItems : MonoBehaviour
{
    public static PackageItems Instance { get; private set; }

    public List<Item> items = new List<Item>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    //遍历背包内物品
    public void UseItem()
    {
        do{
            foreach (Item item in items)
            {
                if (item.isActiveItem)
                {                
                    item.currentCD++;
                    item.SetAlpha();
                } 
            }
            if (!CheckItemCanUse())//如果没有可使用的物品
            {
                EnemyController.Instance.ReduceEnemyAttackTime();
                EnemyController.Instance.ReduceEnemyAttackTime();
            }
            else
            {
                EnemyController.Instance.ReduceEnemyAttackTime();
            }
        }
        while (!CheckItemCanUse());
        
    }

    //检查是否有可用物品
    public bool CheckItemCanUse()
    {
        Debug.Log("检查是否有可用物品");
        foreach (Item item in items)
        {
            if (item.isActiveItem)
            {
                if(item.currentCD >= item.CD)
                {
                    Debug.Log("有可用物品" + item.name);
                    return true;
                }
            }
        }
        Debug.Log("没有可用物品");
        return false;
    }


}

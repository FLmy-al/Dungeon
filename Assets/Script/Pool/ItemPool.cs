using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPool", menuName = "ItemPool")]
public class ItemPool : ScriptableObject
{
    public static ItemPool Instance {  get; private set; }
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        Instance = Resources.Load<ItemPool>("ScriptableObject/ItemPool");
        Debug.Log("初始化物品池");
    }

    public List<Item> items = new List<Item>();

}

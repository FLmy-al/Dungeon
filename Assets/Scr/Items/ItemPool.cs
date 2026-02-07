using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    public static ItemPool Instance {  get; private set; }

    public List<Item> items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}

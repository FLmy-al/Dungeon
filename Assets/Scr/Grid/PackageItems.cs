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
}

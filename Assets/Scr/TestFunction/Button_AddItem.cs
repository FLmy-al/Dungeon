using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_AddItem : MonoBehaviour
{
    public Item item;

    private Image image;
    private TMP_Text itemName;

    private void Start()
    {
        image = GetComponent<Image>();
        itemName = GetComponentInChildren<TMP_Text>();

        image = item.image;
        itemName.text = item.name;
    }
    public void PutDown()
    {
        PackageManager package = FindAnyObjectByType<PackageManager>();

        package.AddItemToGrid(item);
    }
}

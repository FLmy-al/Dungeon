using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_AddItem : MonoBehaviour
{
    private Item item;
    private Image image;
    private TMP_Text itemName;

    private void Awake()
    {
        image = GetComponent<Image>();
        itemName = GetComponentInChildren<TMP_Text>();
    }
    public void PutDown()
    {
        PackageManager package = FindAnyObjectByType<PackageManager>();

        package.AddItemToGrid(item);
    }

    public void InitItemUI(Item targetItem)
    {
        item = targetItem;
        // 重新执行赋值逻辑
        if (image != null && item.GetImage() != null)
        {
            image.sprite = item.GetImage().sprite;
        }
        if (itemName != null)
        {
            itemName.text = item.itemName;
        }
    }
}

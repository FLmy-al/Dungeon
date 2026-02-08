using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddItemToPackage : MonoBehaviour
{
    public Item item;

    private Image image;
    private TMP_Text itemName;

    private void Start()
    {
        image = GetComponent<Image>();
        itemName = GetComponent<TMP_Text>();

        image = item.image;
        itemName.text = item.name;
    }
    public void PutDown()
    {
        
    }
}

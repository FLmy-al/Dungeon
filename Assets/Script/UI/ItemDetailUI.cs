using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 创建通用UI预制体
public class ItemDetailUI : MonoBehaviour
{
    public static ItemDetailUI Instance {  get; private set; }

    public CanvasGroup canvasGroup;
    public TMP_Text itemName;
    public TMP_Text description;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowItemDetail(Item item)
    {
        itemName.text = item.itemName;
        description.text = item.itemDescription;

        // 自适应高度
        StartCoroutine(AdjustSize());
    }

    IEnumerator AdjustSize()
    {
        yield return null; // 等待一帧让布局更新
    }
}

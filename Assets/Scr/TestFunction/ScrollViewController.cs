using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    // 拖拽赋值：Scroll View的Content对象
    public RectTransform content;
    // 拖拽赋值：列表项预制体
    public Button_AddItem itemPrefab;

    // 模拟数据（比如背包物品列表）
    private List<Item> itemDataList;

    private void Start()
    {
        itemDataList = ItemPool.Instance.items;
        // 初始化滚动列表
        InitScrollView();
    }

    /// <summary>
    /// 初始化滚动列表（清空旧内容 + 生成新内容）
    /// </summary>
    private void InitScrollView()
    {
        // 第一步：清空Content下的旧子物体（避免重复生成）
        ClearAllItems();

        // 第二步：遍历数据，生成列表项
        foreach (var itemData in itemDataList)
        {
            // 实例化预制体，作为Content的子对象
            Button_AddItem item_AddButton = Instantiate(itemPrefab, content);

            item_AddButton.name = "Item_AddButton" + itemData.name;
            item_AddButton.InitItemUI(itemData);
        }

        // 第三步：强制刷新Content大小（解决动态生成后大小未更新的问题）
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    /// <summary>
    /// 清空所有列表项
    /// </summary>
    private void ClearAllItems()
    {
        // 遍历Content的子物体，逐个销毁
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(content.GetChild(i).gameObject);
        }
    }

}
